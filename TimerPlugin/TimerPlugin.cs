using PluginInterface;

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Timers;

namespace TimerPlugin
{
    class TimerPlugin : PluginBase, IPluginInterface
    {
        public string PluginName { get; } = "TimerPlugin";
        private string _pluginConfigFile = "TestPluginSettings.json";
        private string _alarmSound = "TestPluginSettings.json";
        public PluginCommand[] Commands { get; set; }
        List<Timer> timers = new List<Timer>();

        public TimerPlugin(IAudioOutSingleton audioOut) : base(audioOut)
        {
            var configBuilder = new Config<TimerPluginSettings>(_pluginConfigFile);
            if (!File.Exists(_pluginConfigFile))
            {
                configBuilder.SaveConfig();
            }

            Commands = configBuilder.ConfigStorage.Commands;
            _alarmSound = configBuilder.ConfigStorage.AlarmSound;
        }

        public string Execute(string commandName, IEnumerable<Token> commandTokens)
        {
            var command = Commands.FirstOrDefault(n => n.Name == commandName);

            if (commandName.StartsWith("stop", StringComparison.OrdinalIgnoreCase))
            {
                foreach (var t in timers)
                {
                    t.Stop();
                }
                timers = new List<Timer>();

                _audioOut.Speak("Все таймеры остановлены");

                return "";
            }
            else
            {
                var minToken = command.GetParameter("%minutes%", commandTokens);
                var secToken = command.GetParameter("%seconds%", commandTokens);
                var minCount = TextToNumberRus.GetNumber(minToken.Value, minToken.successRate);
                var secCount = TextToNumberRus.GetNumber(secToken.Value, secToken.successRate);

                if (minCount + secCount == 0)
                {
                    return "Некорректное время";
                }

                var t = new Timer()
                {
                    AutoReset = false,
                    Interval = new TimeSpan(0, minCount, secCount).TotalMilliseconds,
                };
                t.Elapsed += new ElapsedEventHandler((obj, args) =>
                {
                    _audioOut.PlayFile(_alarmSound);
                });

                timers.Add(t);
                t.Start();

                var paramValue = "";
                if (minCount > 0)
                    paramValue = NumberToTextRus.Str(minCount) + "минут ";

                if (secCount > 0)
                    paramValue += NumberToTextRus.Str(secCount) + "секунд";

                _audioOut.Speak($"Таймер заведен на {paramValue}");

                return "";
            }
        }
    }
}
