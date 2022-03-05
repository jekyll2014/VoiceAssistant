/*
ToDo:
- move core messages into resources
- command validation all over plugins to avoid similar commands
- check if audio capture can be used by plugin
- use MQTT connection for instance-to-instance command exchange (manage by plugin)

Plugins list planned:
1. Hello
2. Timer
3. MPC-HC (VLC?) control with web interface
4. Google/Yandex calendar task add
5. Weather check
6. Suburban trains (yandex)
7. Currency rates
8. Open web-site in browser
9. Run program
10. Message broadcast/announce to selected/all instances in the network (websocket or json + mqtt)
11. Voice connection (interphone/speakerphone) between instances
12. Play music from folder by name/artist
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

namespace VoiceAssistant
{
    internal static class Program
    {
        private static readonly string _appConfigFile = "appsettings.json";
        private static VoiceAssistantSettings _appConfig;
        private static List<PluginBase> _plugins = new List<PluginBase>();

        private static void Main(string[] args)
        {
            var configBuilder = new Config<VoiceAssistantSettings>(_appConfigFile);

            if (!File.Exists(_appConfigFile))
                configBuilder.SaveConfig();

            _appConfig = configBuilder.ConfigStorage;
            var currentCommand = new List<ProcessingCommand>();

            Console.WriteLine("Voice Assistant");

            // Init audio output device
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
                    selectedDevice = n;
            }

            recordOutputs.TryGetValue(selectedDevice, out var outDevice);

            var waveOut = new WaveOut
            {
                DeviceNumber = selectedDevice
            };

            Console.WriteLine($"Selected output device: {outDevice}");

            // Init audio input device
            var recordDeviceNumber = WaveIn.DeviceCount;
            var recordInputs = new Dictionary<int, string>(recordDeviceNumber + 1);
            Console.WriteLine("\r\nAvailable input devices:");
            selectedDevice = -1;

            for (var n = -1; n < recordDeviceNumber; n++)
            {
                var caps = WaveIn.GetCapabilities(n);
                recordInputs.Add(n, caps.ProductName);
                Console.WriteLine($"- {n}: {caps.ProductName}");

                if (!string.IsNullOrEmpty(_appConfig.SelectedAudioInDevice) &&
                    caps.ProductName.StartsWith(_appConfig.SelectedAudioInDevice))
                    selectedDevice = n;
            }

            recordInputs.TryGetValue(selectedDevice, out var inDevice);

            var waveIn = new WaveInEvent
            {
                DeviceNumber = selectedDevice,
                WaveFormat = new WaveFormat(_appConfig.AudioInSampleRate, 1)
            };

            Console.WriteLine($"Selected input device: {inDevice}");
            Console.WriteLine($"Stream settings: {waveIn.WaveFormat}");

            // init TTS
            var synthesizer = new SpeechSynthesizer();
            Console.WriteLine("\r\nAvailable voices:");

            foreach (var voice in synthesizer.GetInstalledVoices())
            {
                var info = voice.VoiceInfo;
                Console.WriteLine(
                    $"- Id: {info.Id} | Name: {info.Name} | Age: {info.Age} | Gender: {info.Gender} | Culture: {info.Culture} ");

                if (!string.IsNullOrEmpty(_appConfig.VoiceName))
                {
                    if (info.Name == _appConfig.VoiceName)
                        synthesizer.SelectVoice(_appConfig.VoiceName);
                }
                else if (!string.IsNullOrEmpty(_appConfig.SpeakerCulture) &&
                         info.Culture.Name.StartsWith(_appConfig.SpeakerCulture))
                {
                    synthesizer.SelectVoice(info.Name);
                }
            }

            var builder = new PromptBuilder();
            Console.WriteLine($"Selected voice: {synthesizer.Voice.Name}");

            //create audio output interface singleton
            var audioOut = AudioOutSingleton.GetInstance(_appConfig.SpeakerCulture, synthesizer, builder, waveOut,
                _appConfig.AudioOutSampleRate);

            // Init Speech recognition
            // You can set to -1 to disable logging messages
            Vosk.Vosk.SetLogLevel(_appConfig.VoskLogLevel);
            VoskRecognizer rec;

            if (!Directory.Exists(_appConfig.ModelFolder))
            {
                Console.WriteLine("Voice recognition model folder missing: " + _appConfig.ModelFolder);

                return;
            }

            try
            {
                var model = new Model(_appConfig.ModelFolder);
                rec = new VoskRecognizer(model, _appConfig.AudioInSampleRate);
            }
            catch (Exception ex)
            {
                Console.WriteLine("Can't load model (could it be missing?): " + ex.Message);

                return;
            }

            rec.SetMaxAlternatives(0);
            rec.SetWords(true);

            var collectingIntent = false;

            // Init timer to cancel command waiting
            var commandAwaitTimer = new Timer
            {
                AutoReset = false,
                Interval = _appConfig.CommandAwaitTime * 1000
            };

            void CommandAwaitTimerElapsed(object obj, ElapsedEventArgs eventArgs)
            {
                // нашел несколько команд
                if (currentCommand.Count > 1)
                {
                    var possibleCommands = currentCommand
                        .Where(n => n.CommandTokens.Count == n.ExpectedCommand.Tokens.Count()).ToArray();
                    if (possibleCommands.Count() > 1)
                    {
                        audioOut.PlayFile(_appConfig.MisrecognitionSound);
                        Console.WriteLine("Multiple command definitions");
                    }

                    // ToDo: do I need "OrderByDescending()" ?
                    var command = possibleCommands.OrderByDescending(n => n.CommandTokens.Count).FirstOrDefault();

                    if (command != null)
                    {
                        // запускаем найденную команду
                        var foundPlugin = _plugins.FirstOrDefault(n => n.PluginName == command.PluginName);
                        if (foundPlugin != null)
                        {
                            var task = Task.Run(() =>
                                foundPlugin.Execute(command.ExpectedCommand.Name, command.CommandTokens));
                            task.Wait();
                        }
                        else
                        {
                            Console.WriteLine("Plugin not found");
                        }
                    }
                    else
                    {
                        audioOut.Speak("Команда не распознана");
                        Console.WriteLine("Command not recognized");
                    }
                }
                else
                {
                    audioOut.PlayFile(_appConfig.MisrecognitionSound);
                    Console.WriteLine("Command not complete");
                }

                currentCommand = new List<ProcessingCommand>();
                collectingIntent = false;
            }

            commandAwaitTimer.Elapsed += CommandAwaitTimerElapsed;

            // load plugins
            var pluginLoader = new GenericPluginLoader<PluginBase>();

            //Load all plugins from a folder and tell to use the "o" (ISO-8601) date format
            _plugins = pluginLoader.LoadAll(AppDomain.CurrentDomain.BaseDirectory + _appConfig.PluginsFolder,
                _appConfig.PluginFileMask, audioOut);
            Console.WriteLine($"\r\nLoaded {_plugins.Count} plugin(s)");

            // inform assistant name
            Console.WriteLine($"\r\nAssistant name: {_appConfig.CallSign}");

            foreach (var plugin in _plugins)
            {
                Console.WriteLine($"\r\nPlugin: {plugin.PluginName}");

                foreach (var com in plugin.Commands)
                    Console.WriteLine($"- Command: {com}");
            }

            void ProcessNewWords(object s, WaveInEventArgs waveEventArgs)
            {
                if (!rec.AcceptWaveform(waveEventArgs.Buffer, waveEventArgs.BytesRecorded))
                    return;

                var jsonResult = rec.Result();
                var result = JsonConvert.DeserializeObject<VoskResult>(jsonResult);

                if (result == null || string.IsNullOrEmpty(result.text))
                    return;

                // Debug
                Console.WriteLine("Recognized words: " + result.text);

                foreach (var wordResult in result.result)
                {
                    if (string.IsNullOrEmpty(wordResult.word))
                        continue;

                    wordResult.word = wordResult.word.ToLower();

                    // ждем ключевое слово
                    if (!collectingIntent)
                    {
                        commandAwaitTimer.Stop();
                        var recognize = FuzzyEqual(_appConfig.CallSign, wordResult.word, _appConfig.DefaultSuccessRate);

                        if (recognize)
                        {
                            collectingIntent = true;
                            //запустить таймер на сброс ожидания тела команды
                            commandAwaitTimer.Interval = _appConfig.CommandAwaitTime * 1000;
                            commandAwaitTimer.Start();
                        }
                    }
                    // ищем слова команды
                    else
                    {
                        //сбросить таймер на сброс ожидания тела команды
                        commandAwaitTimer.Stop();

                        // если это только начатая команда, то добавляем в нее варианты команд из плагинов
                        if (currentCommand.Count == 0)
                        {
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
                        }

                        // и проверять на совпадение с списком ожидаемых команд
                        ProcessNextToken(wordResult.word, ref currentCommand);

                        if (currentCommand.Count < 1) // не нашел ничего
                        {
                            collectingIntent = false;
                            audioOut.Speak("Команда не найдена");
                            currentCommand = new List<ProcessingCommand>();
                            commandAwaitTimer.Stop();
                        }
                        else if (currentCommand.Count == 1) // нашел команду
                        {
                            collectingIntent = false;
                            // запускаем найденную команду
                            var command = currentCommand.FirstOrDefault();
                            if (command != null)
                            {
                                var foundPlugin = _plugins.FirstOrDefault(n => n.PluginName == command.PluginName);

                                if (foundPlugin != null)
                                {
                                    var task = Task.Run(() => foundPlugin.Execute(command.ExpectedCommand.Name,
                                        command.CommandTokens));
                                }
                            }

                            currentCommand = new List<ProcessingCommand>();
                            commandAwaitTimer.Stop();
                        }
                        else
                        {
                            commandAwaitTimer.Interval = _appConfig.NextWordAwaitTime * 1000;
                            commandAwaitTimer.Start();
                        }
                    }
                }
            }

            waveIn.DataAvailable += ProcessNewWords;

            waveIn.RecordingStopped += (s, a) => { waveIn.Dispose(); };

            waveIn.StartRecording();

            audioOut.PlayFile(_appConfig.StartSound);
            Console.WriteLine("\r\nAssistant started");

            // wait for the program to stop
            var command = "";

            while (!command.Equals("exit", StringComparison.OrdinalIgnoreCase))
                command = Console.ReadLine();

            waveIn.StopRecording();

            //unload plugins
            pluginLoader.UnloadAll();

            configBuilder.SaveConfig();
        }

        private static bool FuzzyEqual(string[] expectedWords, string newWord, int successRate)
        {
            foreach (var word in expectedWords)
            {
                var result = Fuzz.WeightedRatio(word.ToLower(), newWord.ToLower());

                if (result >= successRate)
                    return true;
            }

            return false;
        }

        private static void ProcessNextToken(string newTokenString, ref List<ProcessingCommand> collectedCommands)
        {
            for (var i = 0; i < collectedCommands.Count; i++)
            {
                var command = collectedCommands[i];
                var curentPosition = command.CommandTokens.Count;

                // если команда уже закончилась, то можно лишь добивать слова в последний параметр
                if (command.ExpectedCommand.Tokens.Length == curentPosition - 1)
                {
                    // если последним идет параметр
                    if (command.ExpectedCommand.Tokens.LastOrDefault().Type == TokenType.Parameter)
                    {
                        var lastParam = command.CommandTokens.LastOrDefault();
                        // добавляем новое слово к параметру
                        lastParam.Value[0] += " " + newTokenString;
                    }
                    // если последней идет команда
                    else
                    {
                        //команда закончилась, а новые слова идут - что-то не то. убрать команду из списка.
                        collectedCommands.RemoveAt(i);
                        i--;
                    }

                    continue;
                }

                var currentExpectedCommandWord = command.ExpectedCommand.Tokens[curentPosition];

                // если это параметр, то добавить слово к текущему списку как параметр.                
                if (currentExpectedCommandWord.Type == TokenType.Parameter)
                {
                    var newToken = new Token
                    {
                        Value = new[] {newTokenString},
                        Type = TokenType.Parameter,
                        SuccessRate = currentExpectedCommandWord.SuccessRate
                    };

                    command.CommandTokens.Add(newToken);
                }
                // если это команда, то обработать
                else if (currentExpectedCommandWord.Type == TokenType.Command)
                {
                    // если средний коэфф. совпадения больше заданного, то отмечаем как очередную часть команды (CommandToken)
                    if (FuzzyEqual(currentExpectedCommandWord.Value, newTokenString,
                        currentExpectedCommandWord.SuccessRate))
                    {
                        var newToken = new Token
                        {
                            Value = new[] {newTokenString},
                            Type = TokenType.Command,
                            SuccessRate = currentExpectedCommandWord.SuccessRate
                        };

                        command.CommandTokens.Add(newToken);
                    }
                    // если это не похоже на команду и перед этим шел параметр, то прибавляем к параметру
                    else if (command.CommandTokens.LastOrDefault()?.Type == TokenType.Parameter)
                    {
                        var lastParam = command.CommandTokens.LastOrDefault();
                        lastParam.Value[0] += " " + newTokenString;
                    }
                    // иначе это слово не вписывается в текущую команду и команду надо убирать из списка потенциальных
                    else
                    {
                        collectedCommands.RemoveAt(i);
                        i--;
                    }
                }
            }
        }
    }
}
