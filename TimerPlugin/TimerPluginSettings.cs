using PluginInterface;

namespace TimerPlugin
{
    public class TimerPluginCommand : PluginCommand
    {
        public string Response = "";
        public bool isStopCommand = false;
    }

    public class TimerPluginSettings
    {
        //[JsonProperty(Required = Required.Always)]
        public string AlarmSound = "timer.wav";
        public string IncorrectTime = "Некорректное время";

        public TimerPluginCommand[] Commands { get; set; } =
        {
            new TimerPluginCommand
            {
                Name = "Timer minutes",
                Tokens = new[]
                {
                    new Token
                    {
                        SuccessRate = 90,
                        Type = TokenType.Command,
                        Value = new[] {"Поставь", "Заведи"}
                    },
                    new Token
                    {
                        SuccessRate = 90,
                        Type = TokenType.Command,
                        Value = new[] {"таймер"}
                    },
                    new Token
                    {
                        SuccessRate = 90,
                        Type = TokenType.Command,
                        Value = new[] {"на"}
                    },
                    new Token
                    {
                        SuccessRate = 90,
                        Type = TokenType.Parameter,
                        Value = new[] {"%minutes%"}
                    },
                    new Token
                    {
                        SuccessRate = 90,
                        Type = TokenType.Command,
                        Value = new[] {"минут", "минуты", "минуту"}
                    }
                },
                Response = "Таймер заведен на {1} минут",
                isStopCommand = false
            },
            new TimerPluginCommand
            {
                Name = "Stop timer minutes",
                Tokens = new[]
                {
                    new Token
                    {
                        SuccessRate = 90,
                        Type = TokenType.Command,
                        Value = new[] {"Останови", "Удали"}
                    },
                    new Token
                    {
                        SuccessRate = 90,
                        Type = TokenType.Command,
                        Value = new[] {"таймер"}
                    },
                    new Token
                    {
                        SuccessRate = 90,
                        Type = TokenType.Command,
                        Value = new[] {"на"}
                    },
                    new Token
                    {
                        SuccessRate = 90,
                        Type = TokenType.Parameter,
                        Value = new[] {"%minutes%"}
                    },
                    new Token
                    {
                        SuccessRate = 90,
                        Type = TokenType.Command,
                        Value = new[] {"минут", "минуты", "минуту"}
                    }
                },
                Response = "Таймер на {1} минут остановлен",
                isStopCommand = true
            },
            new TimerPluginCommand
            {
                Name = "Timer seconds",
                Tokens = new[]
                {
                    new Token
                    {
                        SuccessRate = 90,
                        Type = TokenType.Command,
                        Value = new[] {"Поставь", "Заведи"}
                    },
                    new Token
                    {
                        SuccessRate = 90,
                        Type = TokenType.Command,
                        Value = new[] {"таймер"}
                    },
                    new Token
                    {
                        SuccessRate = 90,
                        Type = TokenType.Command,
                        Value = new[] {"на"}
                    },
                    new Token
                    {
                        SuccessRate = 90,
                        Type = TokenType.Parameter,
                        Value = new[] {"%seconds%"}
                    },
                    new Token
                    {
                        SuccessRate = 90,
                        Type = TokenType.Command,
                        Value = new[] {"секунд", "секунды", "секунду"}
                    }
                },
                Response = "Таймер заведен на {2} секунд",
                isStopCommand = false
            },
            new TimerPluginCommand
            {
                Name = "Stop timer seconds",
                Tokens = new[]
                {
                    new Token
                    {
                        SuccessRate = 90,
                        Type = TokenType.Command,
                        Value = new[] { "Останови", "Удали" }
                    },
                    new Token
                    {
                        SuccessRate = 90,
                        Type = TokenType.Command,
                        Value = new[] {"таймер"}
                    },
                    new Token
                    {
                        SuccessRate = 90,
                        Type = TokenType.Command,
                        Value = new[] {"на"}
                    },
                    new Token
                    {
                        SuccessRate = 90,
                        Type = TokenType.Parameter,
                        Value = new[] {"%seconds%"}
                    },
                    new Token
                    {
                        SuccessRate = 90,
                        Type = TokenType.Command,
                        Value = new[] {"секунд", "секунды", "секунду"}
                    }
                },
                Response = "Таймер на {2} секунд остановлен",
                isStopCommand = true
            },
            new TimerPluginCommand
            {
                Name = "Timer minutes, seconds",
                Tokens = new[]
                {
                    new Token
                    {
                        SuccessRate = 90,
                        Type = TokenType.Command,
                        Value = new[] {"Поставь", "Заведи"}
                    },
                    new Token
                    {
                        SuccessRate = 90,
                        Type = TokenType.Command,
                        Value = new[] {"таймер"}
                    },
                    new Token
                    {
                        SuccessRate = 90,
                        Type = TokenType.Command,
                        Value = new[] {"на"}
                    },
                    new Token
                    {
                        SuccessRate = 90,
                        Type = TokenType.Parameter,
                        Value = new[] {"%minutes%"}
                    },
                    new Token
                    {
                        SuccessRate = 90,
                        Type = TokenType.Command,
                        Value = new[] {"минут", "минуты", "минуту"}
                    },
                    new Token
                    {
                        SuccessRate = 90,
                        Type = TokenType.Parameter,
                        Value = new[] {"%seconds%"}
                    },
                    new Token
                    {
                        SuccessRate = 90,
                        Type = TokenType.Command,
                        Value = new[] {"секунд", "секунды", "секунду"}
                    }
                },
                Response = "Таймер заведен на {1} минут {2} секунд",
                isStopCommand = false
            },
            new TimerPluginCommand
            {
                Name = "Stop timer minutes, seconds",
                Tokens = new[]
                {
                    new Token
                    {
                        SuccessRate = 90,
                        Type = TokenType.Command,
                        Value = new[] { "Останови", "Удали" }
                    },
                    new Token
                    {
                        SuccessRate = 90,
                        Type = TokenType.Command,
                        Value = new[] {"таймер"}
                    },
                    new Token
                    {
                        SuccessRate = 90,
                        Type = TokenType.Command,
                        Value = new[] {"на"}
                    },
                    new Token
                    {
                        SuccessRate = 90,
                        Type = TokenType.Parameter,
                        Value = new[] {"%minutes%"}
                    },
                    new Token
                    {
                        SuccessRate = 90,
                        Type = TokenType.Command,
                        Value = new[] {"минут", "минуты", "минуту"}
                    },
                    new Token
                    {
                        SuccessRate = 90,
                        Type = TokenType.Parameter,
                        Value = new[] {"%seconds%"}
                    },
                    new Token
                    {
                        SuccessRate = 90,
                        Type = TokenType.Command,
                        Value = new[] {"секунд", "секунды", "секунду"}
                    }
                },
                Response = "Таймер на {1} минут {2} секунд остановлен",
                isStopCommand = true
            },
            new TimerPluginCommand
            {
                Name = "Stop timers",
                Tokens = new[]
                {
                    new Token
                    {
                        SuccessRate = 90,
                        Type = TokenType.Command,
                        Value = new[] {"Останови"}
                    },
                    new Token
                    {
                        SuccessRate = 90,
                        Type = TokenType.Command,
                        Value = new[] {"таймеры"}
                    }
                },
                Response = "Таймеры остановлены",
                isStopCommand = true
            },
            new TimerPluginCommand
            {
                Name = "Stop all timers",
                Tokens = new[]
                {
                    new Token
                    {
                        SuccessRate = 90,
                        Type = TokenType.Command,
                        Value = new[] {"Останови"}
                    },
                    new Token
                    {
                        SuccessRate = 90,
                        Type = TokenType.Command,
                        Value = new[] {"все"}
                    },
                    new Token
                    {
                        SuccessRate = 90,
                        Type = TokenType.Command,
                        Value = new[] {"таймеры"}
                    }
                },
                Response = "Все таймеры остановлены",
                isStopCommand = true
            }
        };
    }
}
