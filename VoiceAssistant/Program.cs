/*
Used resources:
- VOSK voice recognition module: https://alphacephei.com/vosk/
  language models: https://alphacephei.com/vosk/models
- RHVoice Lab voices for Windows SAPI5: https://rhvoice.su/voices/
- NAudio project: https://github.com/naudio/NAudio
- Weighting string comparison alghoritm: https://github.com/JakeBayer/FuzzySharp

ToDo:
- move core messages into resources
- command validation all over plugins to avoid similar commands
- allow command injection from plugin (enable by config parameter)
- console commands like "help", "commands", "log on/off"
- core logging and corresponding settings parameter

Plugins list planned:
1. Hello (done)
2. Timer (done)
3. Open web-site in browser (done)
4. Run program (done)
5. Currency rates (done)
6. Application control using key code injection (done)
7. Weather check openweathermap.org (done)

8. Google/Yandex calendar tasks check/add (https://developers.google.com/calendar/api/quickstart/dotnet , https://stackoverflow.com/questions/55103032/how-to-create-an-event-in-google-calendar-using-c-sharp-and-google-api )
9. Play music from folder by name/artist (foobar - https://www.foobar2000.org/components/view/foo_beefweb , https://hyperblast.org/beefweb/api/)
10. Message broadcast/announce to selected/all instances in the network (websocket + mqtt)
11. Voice connection (interphone/speakerphone) between instances (websocket + mqtt)
12. Suburban trains (yandex)
*/

using FuzzySharp;

using NAudio.Wave;

using Newtonsoft.Json;

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Speech.Synthesis;
using System.Threading.Tasks;
using System.Timers;
using PluginInterface;
using Vosk;
using NAudio.CoreAudioApi;

namespace VoiceAssistant
{
    internal static class Program
    {
        private static readonly string _appConfigFile = "appsettings.json";
        private static VoiceAssistantSettings _appConfig;
        private static GenericPluginLoader<PluginBase> pluginLoader = new GenericPluginLoader<PluginBase>();
        private static List<PluginBase> _plugins = new List<PluginBase>();
        private static List<ProcessingCommand> currentCommand = new List<ProcessingCommand>();
        private static bool collectingIntent = false;
        private static AudioOutSingleton audioOut;
        private static WaveOut waveOut;
        private static float _savedVolume = 0;
        private static VoskRecognizer voiceRecognition;
        private static Timer commandAwaitTimer;
        private static readonly object SyncRoot = new object();

        private static void Main(string[] args)
        {
            _appConfig = LoadConfig();

            Console.WriteLine("Voice Assistant");

            // Init audio output device
            waveOut = InitAudioOutput();

            // Init audio input device
            var waveIn = InitAudioInput();

            // Init Speech recognition
            voiceRecognition = InitSpeechToText();
            if (voiceRecognition == null)
                return;

            // init TTS
            audioOut = InitTextToSpeech(waveOut);

            // load plugins
            _plugins = LoadPlugins();

            // Init timer to cancel command waiting
            commandAwaitTimer = InitCommandAwaitTimer();
            commandAwaitTimer.Elapsed += CommandAwaitTimerElapsed;

            // print out assistant name
            Console.WriteLine("\r\nAssistant names:");
            foreach (var name in _appConfig.CallSign)
            {
                Console.Write($"  {name}");
            }

            Console.WriteLine();

            // start listening
            waveIn.DataAvailable += ProcessAudioInput;
            waveIn.RecordingStopped += (s, a) => { waveIn.Dispose(); };

            try
            {
                waveIn.StartRecording();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Can't open audio input: {ex.Message}");
            }

            audioOut.PlayFile(_appConfig.StartSound);
            Console.WriteLine("\r\nAssistant started");

            // wait for the program to stop
            var command = "";
            while (!command.Equals("exit", StringComparison.OrdinalIgnoreCase))
            {
                command = Console.ReadLine();
            }

            waveIn.StopRecording();

            waveOut.Stop();
            waveOut.Dispose();

            //unload plugins
            pluginLoader.UnloadAll();
        }

        private static VoiceAssistantSettings LoadConfig()
        {
            var configBuilder = new Config<VoiceAssistantSettings>(_appConfigFile);

            if (!File.Exists(_appConfigFile))
                configBuilder.SaveConfig();

            return configBuilder.ConfigStorage;
        }

        private static WaveOut InitAudioOutput()
        {
            var audioDeviceNumber = WaveOut.DeviceCount;
            var recordOutputs = new Dictionary<int, string>(audioDeviceNumber + 1);
            var selectedDevice = -1;
            Console.WriteLine("\r\nAvailable output devices:");

            for (var n = -1; n < audioDeviceNumber; n++)
            {
                var caps = WaveOut.GetCapabilities(n);
                recordOutputs.Add(n, caps.ProductName);
                Console.WriteLine($"- {n}: {caps.ProductName}");

                if (!string.IsNullOrEmpty(_appConfig.SelectedAudioOutDevice) &&
                    caps.ProductName.StartsWith(_appConfig.SelectedAudioOutDevice))
                {
                    selectedDevice = n;
                }
            }

            recordOutputs.TryGetValue(selectedDevice, out var outDevice);

            var waveOut = new WaveOut
            {
                DeviceNumber = selectedDevice
            };

            Console.WriteLine($"Selected output device: {outDevice}");

            return waveOut;
        }

        private static WaveInEvent InitAudioInput()
        {
            var recordDeviceNumber = WaveIn.DeviceCount;
            var recordInputs = new Dictionary<int, string>(recordDeviceNumber + 1);
            Console.WriteLine("\r\nAvailable input devices:");
            var selectedDevice = -1;

            for (var n = -1; n < recordDeviceNumber; n++)
            {
                var caps = WaveIn.GetCapabilities(n);
                recordInputs.Add(n, caps.ProductName);
                Console.WriteLine($"- {n}: {caps.ProductName}");

                if (!string.IsNullOrEmpty(_appConfig.SelectedAudioInDevice) &&
                    caps.ProductName.StartsWith(_appConfig.SelectedAudioInDevice))
                {
                    selectedDevice = n;
                }
            }

            recordInputs.TryGetValue(selectedDevice, out var inDevice);

            var waveIn = new WaveInEvent
            {
                DeviceNumber = selectedDevice,
                WaveFormat = new WaveFormat(_appConfig.AudioInSampleRate, 1)
            };

            Console.WriteLine($"Selected input device: {inDevice}");
            Console.WriteLine($"Stream settings: {waveIn.WaveFormat}");

            return waveIn;
        }

        private static AudioOutSingleton InitTextToSpeech(WaveOut waveOut)
        {
            var synthesizer = new SpeechSynthesizer();
            Console.WriteLine("\r\nAvailable voices:");

            var voiceSelected = false;
            foreach (var voice in synthesizer.GetInstalledVoices())
            {
                var info = voice.VoiceInfo;
                Console.WriteLine(
                    $"- Id: {info.Id} | Name: {info.Name} | Age: {info.Age} | Gender: {info.Gender} | Culture: {info.Culture} ");

                if (!string.IsNullOrEmpty(_appConfig.VoiceName))
                {
                    if (info.Name == _appConfig.VoiceName)
                    {
                        synthesizer.SelectVoice(_appConfig.VoiceName);
                        voiceSelected = true;
                    }
                }

                if (!voiceSelected
                    && !string.IsNullOrEmpty(_appConfig.SpeakerCulture)
                    && info.Culture.Name.StartsWith(_appConfig.SpeakerCulture))
                {
                    synthesizer.SelectVoice(info.Name);
                }
            }

            var builder = new PromptBuilder();
            Console.WriteLine($"Selected voice: {synthesizer.Voice.Name}");

            //create audio output interface singleton
            var audioOut = AudioOutSingleton.GetInstance(_appConfig.SpeakerCulture, synthesizer, builder, waveOut,
                _appConfig.AudioOutSampleRate);

            return audioOut;
        }

        private static VoskRecognizer InitSpeechToText()
        {
            // set -1 to disable logging messages
            Vosk.Vosk.SetLogLevel(_appConfig.VoskLogLevel);
            VoskRecognizer rec;

            if (!Directory.Exists(_appConfig.ModelFolder))
            {
                Console.WriteLine($"Voice recognition model folder missing: {_appConfig.ModelFolder}");

                return null;
            }

            try
            {
                var model = new Model(_appConfig.ModelFolder);
                rec = new VoskRecognizer(model, _appConfig.AudioInSampleRate);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Can't load model (could it be missing?): {ex.Message}");

                return null;
            }

            rec.SetMaxAlternatives(0);
            rec.SetWords(true);

            return rec;
        }

        private static List<PluginBase> LoadPlugins()
        {
            pluginLoader = new GenericPluginLoader<PluginBase>();
            var pluginFolder = AppDomain.CurrentDomain.BaseDirectory + _appConfig.PluginsFolder;

            //Load all plugins from a folder
            var plugins = pluginLoader.LoadAll(pluginFolder, _appConfig.PluginFileMask, audioOut, _appConfig.SpeakerCulture);
            Console.WriteLine($"\r\n{plugins.Count} plugin(s) loaded");

            foreach (var plugin in plugins)
            {
                Console.WriteLine($"\r\nPlugin: {plugin.PluginName}");

                foreach (var com in plugin.Commands)
                {
                    Console.WriteLine($" - Command name: {com.Name}");
                    Console.WriteLine($"     Phrase: {com}");
                }
            }

            return plugins;
        }

        private static Timer InitCommandAwaitTimer()
        {
            var commandAwaitTimer = new Timer
            {
                AutoReset = false,
                Interval = _appConfig.CommandAwaitTime * 1000
            };

            return commandAwaitTimer;
        }

        private static void ProcessAudioInput(object s, WaveInEventArgs waveEventArgs)
        {
            lock (SyncRoot)
            {
                var recognitionResult = voiceRecognition.AcceptWaveform(waveEventArgs.Buffer, waveEventArgs.BytesRecorded);

                // copy audio data to plugin's buffer if allowed anf if any plugin wants
                if (_appConfig.AllowPluginsToListenToSound)
                {
                    var recordingPlugins = _plugins.Where(n => n.AcceptsSound);

                    if (recordingPlugins.Any())
                    {
                        var len = waveEventArgs.BytesRecorded;
                        byte[] dataCopy = new byte[len];
                        Array.Copy(waveEventArgs.Buffer, dataCopy, len);

                        foreach (var plugin in recordingPlugins)
                        {
                            plugin.AddSound(dataCopy);
                        }
                    }
                }

                if (!recognitionResult)
                {
                    return;
                }

                var jsonResult = voiceRecognition.Result();
                var newWords = JsonConvert.DeserializeObject<VoskResult>(jsonResult);

                if (newWords == null || string.IsNullOrEmpty(newWords.text))
                {
                    return;
                }

                Console.WriteLine($"Recognized words: {newWords.text}");

                // copy recognized words to plugin's buffer if allowed anf if any plugin wants
                if (_appConfig.AllowPluginsToListenToWords)
                {
                    foreach (var plugin in _plugins)
                    {
                        if (plugin.AcceptsWords) plugin.AddWords(newWords.text);
                    }
                }

                // process new words
                ProcessNewWords(newWords.result);
            }
        }

        private static void ProcessNewWords(List<Result> newWords)
        {
            foreach (var word in newWords)
            {
                if (string.IsNullOrEmpty(word.word))
                {
                    continue;
                }

                word.word = word.word.ToLower();

                // looking for an assistant name (keyword)
                if (!collectingIntent)
                {
                    var recognize = FuzzyEqual(_appConfig.CallSign, word.word, _appConfig.DefaultSuccessRate);

                    if (recognize)
                    {
                        _savedVolume = GetMasterVolume();
                        SetMasterVolume(0.1f);

                        // add all plugins command to command pool (to exclude unmatched later)
                        currentCommand.Clear();

                        foreach (var plugin in _plugins)
                        {
                            foreach (var command in plugin.Commands)
                            {
                                var newCmdItem = new ProcessingCommand
                                {
                                    PluginName = plugin.PluginName,
                                    ExpectedCommand = command
                                };

                                currentCommand.Add(newCmdItem);
                            }
                        }

                        //run timer to wait for the command phrase (stop waiting if no command received)
                        commandAwaitTimer.Interval = _appConfig.CommandAwaitTime * 1000;
                        commandAwaitTimer.Start();
                        collectingIntent = true;
                    }
                }
                // add new words to a command phrase
                else
                {
                    //stop timer in any case and restart later if phrase is not completed
                    commandAwaitTimer.Stop();

                    // check word to comply with any command in command pool
                    ProcessNextToken(word.word, ref currentCommand);

                    // no commands left similar to the input phrase
                    if (currentCommand.Count < 1)
                    {
                        collectingIntent = false;
                        SetMasterVolume(_savedVolume);
                        audioOut.Speak(_appConfig.CommandNotRecognizedMessage);
                        Console.WriteLine(_appConfig.CommandNotRecognizedMessage);
                        currentCommand.Clear();
                    }
                    // the only command found
                    else if (currentCommand.Count == 1)
                    {
                        var command = currentCommand.FirstOrDefault();

                        // if a command is of the same length and the last token is not of Parameter type
                        // Parameter could be appended with new words within word-to-word timer
                        if (command.CommandTokens.Count == command.ExpectedCommand.Tokens.Count()
                            && command.CommandTokens.LastOrDefault().Type != TokenType.Parameter)
                        {
                            collectingIntent = false;
                            SetMasterVolume(_savedVolume);
                            ExecuteCommand(command);
                            currentCommand.Clear();
                        }
                    }
                    // still there are multiple commands possible
                    else
                    {
                        commandAwaitTimer.Interval = _appConfig.NextWordAwaitTime * 1000;
                        commandAwaitTimer.Start();
                    }
                }
            }
        }

        private static void CommandAwaitTimerElapsed(object obj, ElapsedEventArgs eventArgs)
        {
            lock (SyncRoot)
            {
                collectingIntent = false;
                SetMasterVolume(_savedVolume);

                // filter out incomplete commands
                var possibleCommands = currentCommand
                    .Where(n => n.CommandTokens.Count == n.ExpectedCommand.Tokens.Count()).ToArray();

                if (possibleCommands.Count() < 1)
                {
                    audioOut.PlayFile(_appConfig.MisrecognitionSound);
                    Console.WriteLine(_appConfig.CommandNotFoundMessage);
                }
                else
                {
                    // multiple commands found
                    if (possibleCommands.Count() > 1)
                    {
                        Console.WriteLine("Multiple similar command found. Check command definitions:");

                        foreach (var pcommand in possibleCommands)
                        {
                            Console.WriteLine("----");
                            Console.WriteLine(pcommand.ToString());
                            Console.WriteLine("----");
                        }
                    }

                    foreach (var command in possibleCommands)
                    {
                        ExecuteCommand(command);
                    }
                }

                currentCommand.Clear();
            }
        }

        private static void ExecuteCommand(ProcessingCommand command)
        {
            if (command == null)
                return;

            var foundPlugin = _plugins.FirstOrDefault(n => n.PluginName == command.PluginName);

            if (foundPlugin != null)
            {
                Console.WriteLine("Executing command:");
                Console.WriteLine(command.ToString());

                var task = Task.Run(() =>
                {
                    var commandName = command.ExpectedCommand.Name;
                    var commandTokens = command.CommandTokens;

                    try
                    {
                        foundPlugin.Execute(commandName, commandTokens);
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"Failed to execute plugin command: {command.ToString()}");
                        Console.WriteLine($"Exception details: {ex.Message}");
                    }
                }).ConfigureAwait(true);
            }
            // shouldn't be but just in case
            else
            {
                Console.WriteLine($"Plugin {command.PluginName} not found for command:");
                Console.WriteLine(command.ToString());
            }
        }

        private static bool FuzzyEqual(string[] expectedWords, string newWord, int successRate)
        {
            foreach (var word in expectedWords)
            {
                var result = Fuzz.WeightedRatio(word.ToLower(), newWord.ToLower());

                // some words are considered almost equal even if they are obviously not (like "Вася" and "я")
                // so better check if the length difference is less than 2 times.
                var lengthDifference = (float)word.Length / (float)newWord.Length;

                if (lengthDifference <= 0.5 || lengthDifference >= 2)
                {
                    result /= 2;
                }

                if (result >= successRate)
                {
                    return true;
                }
            }

            return false;
        }

        private static void ProcessNextToken(string newTokenString, ref List<ProcessingCommand> collectedCommands)
        {
            for (var i = 0; i < collectedCommands.Count; i++)
            {
                var command = collectedCommands[i];
                var curentPosition = command.CommandTokens.Count;

                // if command phrase is over it's only possible to add words to the previous parameter if any
                if (command.ExpectedCommand.Tokens.Length <= curentPosition)
                {
                    // if last token in the expected phrase is parameter
                    if (command.ExpectedCommand.Tokens.LastOrDefault()?.Type == TokenType.Parameter)
                    {
                        var lastParam = command.CommandTokens.LastOrDefault();

                        // add new word to the parameter
                        if (lastParam != null)
                        {
                            lastParam.Value[0] += " " + newTokenString;
                        }
                    }
                    // if last token in the expected phrase is command
                    else
                    {
                        //phrase is over and new words are coming. Remove it from a pool
                        collectedCommands.RemoveAt(i);
                        i--;
                    }

                    continue;
                }

                var currentExpectedCommandWord = command.ExpectedCommand.Tokens[curentPosition];

                // if it's a parameter than add new word as a parameter
                if (currentExpectedCommandWord.Type == TokenType.Parameter)
                {
                    var newToken = new Token
                    {
                        Value = new[] { newTokenString },
                        Type = TokenType.Parameter,
                        SuccessRate = currentExpectedCommandWord.SuccessRate
                    };

                    command.CommandTokens.Add(newToken);
                }
                // if it's a command than process word
                else if (currentExpectedCommandWord.Type == TokenType.Command)
                {
                    // if cmpliance rate id more than desired than it's a next phrase part
                    if (FuzzyEqual(currentExpectedCommandWord.Value, newTokenString,
                        currentExpectedCommandWord.SuccessRate))
                    {
                        var newToken = new Token
                        {
                            Value = new[] { newTokenString },
                            Type = TokenType.Command,
                            SuccessRate = currentExpectedCommandWord.SuccessRate
                        };

                        command.CommandTokens.Add(newToken);
                    }
                    // if it doesn't look like a command - try to add it to the previous parameter if any
                    else if (command.CommandTokens.LastOrDefault()?.Type == TokenType.Parameter)
                    {
                        var lastParam = command.CommandTokens.LastOrDefault();

                        if (lastParam != null)
                        {
                            lastParam.Value[0] += " " + newTokenString;
                        }
                    }
                    // or it's the wrng command. Remove it from a pool
                    else
                    {
                        collectedCommands.RemoveAt(i);
                        i--;
                    }
                }
            }
        }

        private static void SetMasterVolume(float level)
        {
            MMDeviceEnumerator devEnum = new MMDeviceEnumerator();
            MMDevice defaultDevice = devEnum.GetDefaultAudioEndpoint(DataFlow.Render, Role.Multimedia);
            //string currVolume = "MasterPeakVolume : " + defaultDevice.AudioMeterInformation.MasterPeakValue.ToString();
            var range = defaultDevice.AudioEndpointVolume.VolumeRange;
            defaultDevice.AudioEndpointVolume.MasterVolumeLevelScalar = level;
        }

        private static float GetMasterVolume()
        {
            MMDeviceEnumerator devEnum = new MMDeviceEnumerator();
            MMDevice defaultDevice = devEnum.GetDefaultAudioEndpoint(DataFlow.Render, Role.Multimedia);

            return defaultDevice.AudioEndpointVolume.MasterVolumeLevelScalar;
        }
    }
}
