using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Timers;

using PluginInterface;

namespace TimerPlugin
{
    public class TimerPlugin : PluginBase
    {
        private readonly string _alarmSound;
        private List<Timer> _timers = new List<Timer>();

        public TimerPlugin(IAudioOutSingleton audioOut, string pluginPath) : base(audioOut, pluginPath)
        {
            var configBuilder = new Config<TimerPluginSettings>($"{PluginPath}\\{PluginConfigFile}");

            if (!File.Exists($"{PluginPath}\\{PluginConfigFile}")) configBuilder.SaveConfig();

            _commands = configBuilder.ConfigStorage.Commands;
            _alarmSound = configBuilder.ConfigStorage.AlarmSound;
        }

        public override string Execute(string commandName, List<Token> commandTokens)
        {
            var command = Commands.FirstOrDefault(n => n.Name == commandName);

            var paramValue = string.Empty;

            if (command == null)
                return paramValue;

            if (commandName.StartsWith("stop", StringComparison.OrdinalIgnoreCase))
            {
                foreach (var t in _timers) t.Stop();
                _timers = new List<Timer>();

                paramValue = command.Response;
            }
            else
            {
                var minToken = command.GetParameter("%minutes%", commandTokens);
                var secToken = command.GetParameter("%seconds%", commandTokens);
                var minCount = 0;

                if (minToken != null)
                    minCount = TextToNumberRus.GetNumber(minToken.Value[0], minToken.SuccessRate);

                var secCount = 0;
                if (secToken != null)
                    secCount = TextToNumberRus.GetNumber(secToken.Value[0], secToken.SuccessRate);

                if (minCount + secCount == 0)
                {
                    paramValue = "Некорректное время";
                }
                else
                {
                    var t = new Timer
                    {
                        AutoReset = false,
                        Interval = new TimeSpan(0, minCount, secCount).TotalMilliseconds
                    };

                    t.Elapsed += (obj, args) => { AudioOut.PlayFile($"{PluginPath}\\{_alarmSound}"); };

                    _timers.Add(t);
                    t.Start();

                    // string.Empty is used to avoid using {0} int templates
                    paramValue = string.Format(command.Response, string.Empty, NumberToTextRus.Str(minCount),
                        NumberToTextRus.Str(secCount));
                }
            }

            AudioOut.Speak(paramValue);

            return paramValue;
        }
    }
}
