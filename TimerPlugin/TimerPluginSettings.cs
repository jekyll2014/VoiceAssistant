using PluginInterface;

namespace TimerPlugin
{
    public class TimerPluginSettings
    {
        //[JsonProperty(Required = Required.Always)]
        public string AlarmSound = "timer.wav";

        public PluginCommand[] Commands { get; set; } =
        {
            new PluginCommand
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
                Response = "Таймер заведен на {1} минут"
            },
            new PluginCommand
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
                Response = "Таймер заведен на {2} секунд"
            },
            new PluginCommand
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
                Response = "Таймер заведен на {1} минут {2} секунд"
            },
            new PluginCommand
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
                Response = "Таймеры остановлены"
            },
            new PluginCommand
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
                Response = "Таймеры остановлены"
            }
        };
    }
}
