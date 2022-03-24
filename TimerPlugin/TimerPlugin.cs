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
        private readonly string _timerNotFound;
        private readonly List<(string, Timer)> _timers = new List<(string, Timer)>();

        public TimerPlugin(IAudioOutSingleton audioOut, string currentCulture, string pluginPath) : base(audioOut, currentCulture, pluginPath)
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
            _timerNotFound = configBuilder.ConfigStorage.TimerNotFound;
        }

        public override void Execute(string commandName, List<Token> commandTokens)
        {
            var command = TimerCommands.FirstOrDefault(n => n.Name == commandName);


            if (command == null)
            {
                return;
            }

            var minToken = command.GetParameter("%minutes%", commandTokens);
            var secToken = command.GetParameter("%seconds%", commandTokens);
            var minCount = 0;

            var stringToNumberConvertor = TextToNumberConvertor.GetTextToNumberConvertor(CurrentCulture);

            if (minToken != null)
            {
                minCount = stringToNumberConvertor.ConvertStringToNumber(minToken.Value[0], minToken.SuccessRate);
            }

            var secCount = 0;
            if (secToken != null)
            {
                secCount = stringToNumberConvertor.ConvertStringToNumber(secToken.Value[0], secToken.SuccessRate);
            }

            var response = string.Empty;
            if (!command.isStopCommand && minCount + secCount == 0)
            {
                response = _incorrectTime;
            }
            else
            {
                var delay = $"{minCount}+{secCount}";

                var NumberToStringConvertor = NumberToTextConvertor.GetNumberToTextConvertor(CurrentCulture);


                if (command.isStopCommand)
                {
                    var timer = _timers.FirstOrDefault(n => n.Item2.Enabled && n.Item1 == delay);

                    if (timer.Item2 != null)
                    {
                        timer.Item2.Stop();
                        _timers.Remove(timer);
                        response = command.Response;
                        // string.Empty is used to avoid using {0} int templates
                        response = string.Format(command.Response, string.Empty, 
                            NumberToStringConvertor.ConvertNumberToString(minCount),
                            NumberToStringConvertor.ConvertNumberToString(secCount));
                    }
                    else
                    {
                        response = _timerNotFound;
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
                    response = string.Format(command.Response, string.Empty, 
                        NumberToStringConvertor.ConvertNumberToString(minCount),
                        NumberToStringConvertor.ConvertNumberToString(secCount));
                }
            }

            AudioOut.Speak(response);
        }
    }
}
