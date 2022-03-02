/*
Конфигурация команды должна отдаваться из класса команды
После загрузки всех плагинов команд надо проверить, что ни одна команда не скрывает другую

ToDo:
- move string into resources
- command waiting end signal (bip sound)
- MQTT connection for instance-to-instance command exchange
- command validation all over plugins to avoid similar commands
- check audio capture can be used by plugin

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
*/

using FuzzySharp;

using NAudio.Wave;

using Newtonsoft.Json;

using PluginInterface;

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Speech.Synthesis;
using System.Threading.Tasks;
using System.Timers;

using Vosk;

namespace VoiceAssistant
{
    internal static class Program
    {
        private static string _appConfigFile = "appsettings.json";
        private static VoiceAssistantSettings _appConfig;
        private static List<IPluginInterface> _plugins = new List<IPluginInterface>();

        static void Main(string[] args)
        {
            var configBuilder = new Config<VoiceAssistantSettings>(_appConfigFile);

            if (!File.Exists(_appConfigFile))
            {
                configBuilder.SaveConfig();
            }

            _appConfig = configBuilder.ConfigStorage;
            var currentCommand = new List<ProcessingCommand>();

            Console.WriteLine("Voice Assistant");

            // Init audio output device
            var audioDeviceNumber = WaveOut.DeviceCount;
            var recordOutputs = new Dictionary<int, string>(audioDeviceNumber + 1);
            var selectedDevice = -1;
            Console.WriteLine("\r\nAvailable output devices:");

            for (int n = -1; n < audioDeviceNumber; n++)
            {
                var caps = WaveOut.GetCapabilities(n);
                recordOutputs.Add(n, caps.ProductName);
                Console.WriteLine($"- {n}: {caps.ProductName}");

                if (!string.IsNullOrEmpty(_appConfig.SelectedAudioOutDevice) && caps.ProductName == _appConfig.SelectedAudioOutDevice)
                    selectedDevice = n;
            }

            recordOutputs.TryGetValue(selectedDevice, out var outDevice);

            var waveOut = new WaveOut()
            {
                DeviceNumber = selectedDevice
            };

            Console.WriteLine($"Selected output device: {outDevice}");

            // Init audio input device
            var recordDeviceNumber = WaveIn.DeviceCount;
            var recordInputs = new Dictionary<int, string>(recordDeviceNumber + 1);
            Console.WriteLine("\r\nAvailable input devices:");
            selectedDevice = -1;

            for (int n = -1; n < recordDeviceNumber; n++)
            {
                var caps = WaveIn.GetCapabilities(n);
                recordInputs.Add(n, caps.ProductName);
                Console.WriteLine($"- {n}: {caps.ProductName}");

                if (!string.IsNullOrEmpty(_appConfig.SelectedAudioInDevice) && caps.ProductName == _appConfig.SelectedAudioInDevice)
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
                Console.WriteLine($"- Id: {info.Id} | Name: {info.Name} | Age: { info.Age} | Gender: { info.Gender} | Culture: { info.Culture} ");

                if (!string.IsNullOrEmpty(_appConfig.VoiceName))
                {
                    if (info.Name == _appConfig.VoiceName)
                        synthesizer.SelectVoice(_appConfig.VoiceName);
                }
                else if (!string.IsNullOrEmpty(_appConfig.SpeakerCulture) && info.Culture.Name == _appConfig.SpeakerCulture)
                    synthesizer.SelectVoice(info.Name);
            }

            var builder = new PromptBuilder();
            Console.WriteLine($"Selected voice: {synthesizer.Voice.Name}");

            //create audio output interface singleton
            var audioOut = AudioOutSingleton.GetInstance(_appConfig.SpeakerCulture, synthesizer, builder, waveOut, _appConfig.AudioOutSampleRate);

            // Init Speech recognition
            // You can set to -1 to disable logging messages
            Vosk.Vosk.SetLogLevel(-1);
            Model model;
            VoskRecognizer rec;

            if (!Directory.Exists(_appConfig.ModelFolder))
            {
                Console.WriteLine("Voice recognition model folder missing: " + _appConfig.ModelFolder);
                return;
            }

            try
            {
                model = new Model(_appConfig.ModelFolder);
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
            var commandAwaitTimert = new System.Timers.Timer()
            {
                AutoReset = false,
                Interval = _appConfig.CommandAwaitTime * 1000,
            };

            ElapsedEventHandler CommandAwaitTimerElapsed = (obj, args) =>
                           {
                               if (currentCommand.Count > 1) // нашел несколько команд
                               {
                                   var possiblecommands = currentCommand.Where(n => n.CommandTokens.Count == n.ExpectedCommand.Tokens.Count());
                                   if (possiblecommands.Count() > 1)
                                   {
                                       audioOut.PlayFile(_appConfig.MisrecognitionSound);
                                       Console.WriteLine("Несколько вариантов команды");
                                   }

                                   var command = possiblecommands.OrderByDescending(n => n.CommandTokens.Count).FirstOrDefault();
                                   if (command != null)
                                   {
                                       // запускаем найденную команду
                                       var foundPlugin = _plugins.FirstOrDefault(n => n.PluginName == currentCommand.FirstOrDefault().PluginName);
                                       var res = Task.Run(() => { return foundPlugin.Execute(command.ExpectedCommand.Name, command.CommandTokens); });
                                       res.Wait();
                                       audioOut.Speak(res.Result);
                                   }
                                   else
                                   {
                                       audioOut.Speak("Команда не распознана");
                                       Console.WriteLine("Команда не распознана");
                                   }
                               }
                               else
                               {
                                   audioOut.PlayFile(_appConfig.MisrecognitionSound);
                                   Console.WriteLine("Команда не закончена");
                               }

                               currentCommand = new List<ProcessingCommand>();
                               collectingIntent = false;
                           };

            commandAwaitTimert.Elapsed += new ElapsedEventHandler(CommandAwaitTimerElapsed);

            // load plugins
            var pluginLoader = new GenericPluginLoader<IPluginInterface>();

            //Load all plugins from a folder and tell to use the "o" (ISO-8601) date format
            _plugins = pluginLoader.LoadAll(pluginPath: AppDomain.CurrentDomain.BaseDirectory, constructorArgs: audioOut);
            Console.WriteLine($"\r\nLoaded {_plugins.Count} plugin(s)");

            // inform assistant name
            Console.WriteLine($"\r\nAssistant name: {_appConfig.CallSign}");

            // prepare and print available commands
            _appConfig.CallSign = _appConfig.CallSign.ToLower();

            foreach (var plugin in _plugins)
            {
                Console.WriteLine($"\r\nPlugin: {plugin.PluginName}");

                foreach (var com in plugin.Commands)
                {
                    Console.WriteLine($"- {com}");
                }
            }

            EventHandler<WaveInEventArgs> ProcessNewWords = (s, waveEventArgs) =>
                           {
                               if (rec.AcceptWaveform(waveEventArgs.Buffer, waveEventArgs.BytesRecorded))
                               {
                                   var jsonResult = rec.Result();
                                   //Console.WriteLine(jsonResult);

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

                                       if (!collectingIntent)
                                       {
                                           commandAwaitTimert.Stop();
                                           var recognize = FuzzyEqual(wordResult.word, _appConfig.CallSign, _appConfig.DefaultSuccessRate);

                                           if (recognize)
                                           {
                                               collectingIntent = true;
                                               //запустить таймер на сброс ожидания тела команды
                                               commandAwaitTimert.Interval = _appConfig.CommandAwaitTime * 1000;
                                               commandAwaitTimert.Start();
                                           }
                                           else
                                           {
                                               //Console.WriteLine("No key word");
                                           }
                                       }
                                       else
                                       {
                                           //сбросить таймер на сброс ожидания тела команды
                                           commandAwaitTimert.Stop();

                                           // если это только начатая команда, то добавляем в нее варианты команд из плагинов
                                           if (currentCommand.Count == 0)
                                           {
                                               foreach (var plugin in _plugins)
                                               {
                                                   foreach (var command in plugin.Commands)
                                                   {
                                                       var newCmdItem = new ProcessingCommand()
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
                                               commandAwaitTimert.Stop();
                                           }
                                           else if (currentCommand.Count == 1) // нашел команду
                                           {
                                               collectingIntent = false;
                                               // запускаем найденную команду
                                               var command = currentCommand.FirstOrDefault();
                                               var foundPlugin = _plugins.FirstOrDefault(n => n.PluginName == currentCommand.FirstOrDefault().PluginName);
                                               var task = Task.Run(() => { return foundPlugin.Execute(command.ExpectedCommand.Name, command.CommandTokens); });
                                               currentCommand = new List<ProcessingCommand>();
                                               commandAwaitTimert.Stop();
                                           }
                                           else
                                           {
                                               commandAwaitTimert.Interval = _appConfig.NextWordAwaitTime * 1000;
                                               commandAwaitTimert.Start();
                                           }
                                       }
                                   }
                               }
                           };

            waveIn.DataAvailable += ProcessNewWords;

            waveIn.RecordingStopped += (s, a) =>
            {
                waveIn.Dispose();
            };

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

        private static bool FuzzyEqual(string expectedWord, string newWord, int successRate)
        {
            return Fuzz.WeightedRatio(expectedWord, newWord) >= successRate;
        }

        private static void ProcessNextToken(string newTokenString, ref List<ProcessingCommand> collectedCommands)
        {
            for (var i = 0; i < collectedCommands.Count; i++)
            {
                var command = collectedCommands[i];
                // проверить последний токен на совпадение с очередным CommandToken
                // если не команда, но ожидается параметр, то склеивать параметры в один токен
                var curentPosition = command.CommandTokens.Count;

                // если команда уже закончилась, но последним идет параметр
                if (command.ExpectedCommand.Tokens.Length == curentPosition - 1)
                {
                    if (command.ExpectedCommand.Tokens.LastOrDefault().Type == TokenType.Parameter)
                    {
                        var lastParam = command.CommandTokens.LastOrDefault();
                        lastParam.Value += " " + newTokenString;
                    }
                }

                if (command.ExpectedCommand.Tokens.Length <= curentPosition)
                {
                    //команда закончилась, а новые слова идут - что-то не то.
                    //убрать команду из списка.
                    collectedCommands.RemoveAt(i);
                    i--;
                    continue;
                }

                var nextCommandExpected = command.ExpectedCommand.Tokens[curentPosition];

                // если это параметр, то добавить слово к текущему списку как параметр.                
                if (nextCommandExpected.Type == TokenType.Parameter)
                {
                    var newToken = new Token
                    {
                        Value = newTokenString,
                        Type = TokenType.Parameter
                    };

                    command.CommandTokens.Add(newToken);
                }

                // если это команда, то обработать
                else if (nextCommandExpected.Type == TokenType.Command)
                {
                    // если средний коэфф. совпадения больше заданного, то отмечаем как очередную часть команды (CommandToken)
                    if (FuzzyEqual(nextCommandExpected.Value.ToLower(), newTokenString, nextCommandExpected.successRate))
                    {
                        var newToken = new Token
                        {
                            Value = newTokenString,
                            Type = TokenType.Command
                        };

                        command.CommandTokens.Add(newToken);
                    }
                    // если это не похоже на команду и перед этим шел параметр, то прибавляем к параметру
                    else if (command.CommandTokens.LastOrDefault()?.Type == TokenType.Parameter)
                    {
                        var lastParam = command.CommandTokens.LastOrDefault();
                        lastParam.Value += " " + newTokenString;
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
