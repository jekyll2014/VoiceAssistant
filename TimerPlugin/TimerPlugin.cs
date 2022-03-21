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
        private readonly TimerPluginCommand[] TimerCommands;
        private readonly string _alarmSound;
        private readonly string _incorrectTime;
        private List<(string, Timer)> _timers = new List<(string, Timer)>();

        public TimerPlugin(IAudioOutSingleton audioOut, string pluginPath) : base(audioOut, pluginPath)
        {
            var configBuilder = new Config<TimerPluginSettings>($"{PluginPath}\\{PluginConfigFile}");
            if (!File.Exists($"{PluginPath}\\{PluginConfigFile}"))
            {
                configBuilder.SaveConfig();
            }

            TimerCommands = configBuilder.ConfigStorage.Commands;

            if (TimerCommands is PluginCommand[] newCmds)
            {
                _commands = newCmds;
            }

            _alarmSound = configBuilder.ConfigStorage.AlarmSound;
            _incorrectTime = configBuilder.ConfigStorage.IncorrectTime;
        }

        public override string Execute(string commandName, List<Token> commandTokens)
        {
            var command = TimerCommands.FirstOrDefault(n => n.Name == commandName);

            var response = string.Empty;

            if (command == null)
            {
                return response;
            }

            var minToken = command.GetParameter("%minutes%", commandTokens);
            var secToken = command.GetParameter("%seconds%", commandTokens);
            var minCount = 0;

            if (minToken != null)
            {
                minCount = TextToNumberRus.GetNumber(minToken.Value[0], minToken.SuccessRate);
            }

            var secCount = 0;
            if (secToken != null)
            {
                secCount = TextToNumberRus.GetNumber(secToken.Value[0], secToken.SuccessRate);
            }

            if (minCount + secCount == 0)
            {
                response = _incorrectTime;
            }
            else
            {
                var delay = $"{minCount}+{secCount}";

                if (command.isStopCommand)
                {
                    var timer = _timers.FirstOrDefault(n => n.Item2.Enabled && n.Item1 == delay);

                    if (timer.Item2 != null)
                    {
                        timer.Item2.Stop();
                        _timers.Remove(timer);
                        response = command.Response;
                        // string.Empty is used to avoid using {0} int templates
                        response = string.Format(command.Response, string.Empty, NumberToTextRus.Str(minCount),
                            NumberToTextRus.Str(secCount));
                    }
                    else
                    {
                        response = "не найден";
                    }
                }
                else
                {
                    var t = new Timer
                    {
                        AutoReset = false,
                        Interval = new TimeSpan(0, minCount, secCount).TotalMilliseconds
                    };

                    t.Elapsed += (obj, args) =>
                    {
                        var timer = _timers.FirstOrDefault(n => n.Item2 == obj);

                        if (timer.Item2 != null)
                        {
                            _timers.Remove(timer);
                        }

                        AudioOut.PlayFile($"{PluginPath}\\{_alarmSound}");
                    };

                    _timers.Add((delay, t));
                    t.Start();
                    // string.Empty is used to avoid using {0} int templates
                    response = string.Format(command.Response, string.Empty, NumberToTextRus.Str(minCount),
                        NumberToTextRus.Str(secCount));
                }
            }


            AudioOut.Speak(response);

            return response;
        }
    }
}
