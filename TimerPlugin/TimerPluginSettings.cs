using PluginInterface;

namespace TimerPlugin
{
    public class TimerPluginSettings
    {
        //[JsonProperty(Required = Required.Always)]
        public string AlarmSound = "timer.wav";
        public PluginCommand[] Commands { get; set; } = new PluginCommand[]
        {
            new PluginCommand
            {
                Name = "Timer minutes",
                Tokens = new[]
                {
                    new Token
                    {
                        successRate = 90,
                        Type = TokenType.Command,
                        Value = "Поставь"
                    },
                    new Token
                    {
                        successRate = 90,
                        Type = TokenType.Command,
                        Value = "таймер"
                    },
                    new Token
                    {
                        successRate = 90,
                        Type = TokenType.Command,
                        Value = "на"
                    },
                    new Token
                    {
                        successRate = 90,
                        Type = TokenType.Parameter,
                        Value = "%minutes%"
                    },
                    new Token
                    {
                        successRate = 90,
                        Type = TokenType.Command,
                        Value = "минут"
                    }
                }
            },
            new PluginCommand
            {
                Name = "Timer minutes informal",
                Tokens = new[]
                {
                    new Token
                    {
                        successRate = 90,
                        Type = TokenType.Command,
                        Value = "Заведи"
                    },
                    new Token
                    {
                        successRate = 90,
                        Type = TokenType.Command,
                        Value = "таймер"
                    },
                    new Token
                    {
                        successRate = 90,
                        Type = TokenType.Command,
                        Value = "на"
                    },
                    new Token
                    {
                        successRate = 90,
                        Type = TokenType.Parameter,
                        Value = "%minutes%"
                    },
                    new Token
                    {
                        successRate = 90,
                        Type = TokenType.Command,
                        Value = "минут"
                    }
                }
            },
            new PluginCommand
            {
                Name = "Timer seconds",
                Tokens = new[]
                {
                    new Token
                    {
                        successRate = 90,
                        Type = TokenType.Command,
                        Value = "Поставь"
                    },
                    new Token
                    {
                        successRate = 90,
                        Type = TokenType.Command,
                        Value = "таймер"
                    },
                    new Token
                    {
                        successRate = 90,
                        Type = TokenType.Command,
                        Value = "на"
                    },
                    new Token
                    {
                        successRate = 90,
                        Type = TokenType.Parameter,
                        Value = "%seconds%"
                    },
                    new Token
                    {
                        successRate = 90,
                        Type = TokenType.Command,
                        Value = "секунд"
                    }
                }
            },
            new PluginCommand
            {
                Name = "Timer seconds informal",
                Tokens = new[]
                {
                    new Token
                    {
                        successRate = 90,
                        Type = TokenType.Command,
                        Value = "Заведи"
                    },
                    new Token
                    {
                        successRate = 90,
                        Type = TokenType.Command,
                        Value = "таймер"
                    },
                    new Token
                    {
                        successRate = 90,
                        Type = TokenType.Command,
                        Value = "на"
                    },
                    new Token
                    {
                        successRate = 90,
                        Type = TokenType.Parameter,
                        Value = "%seconds%"
                    },
                    new Token
                    {
                        successRate = 90,
                        Type = TokenType.Command,
                        Value = "секунд"
                    }
                }
            },
            new PluginCommand
            {
                Name = "Timer minutes, seconds",
                Tokens = new[]
                {
                    new Token
                    {
                        successRate = 90,
                        Type = TokenType.Command,
                        Value = "Поставь"
                    },
                    new Token
                    {
                        successRate = 90,
                        Type = TokenType.Command,
                        Value = "таймер"
                    },
                    new Token
                    {
                        successRate = 90,
                        Type = TokenType.Command,
                        Value = "на"
                    },
                    new Token
                    {
                        successRate = 90,
                        Type = TokenType.Parameter,
                        Value = "%minutes%"
                    },
                    new Token
                    {
                        successRate = 90,
                        Type = TokenType.Command,
                        Value = "минут"
                    },
                    new Token
                    {
                        successRate = 90,
                        Type = TokenType.Parameter,
                        Value = "%seconds%"
                    },
                    new Token
                    {
                        successRate = 90,
                        Type = TokenType.Command,
                        Value = "секунд"
                    }
                }
            },
            new PluginCommand
            {
                Name = "Timer minutes, seconds informal",
                Tokens = new[]
                {
                    new Token
                    {
                        successRate = 90,
                        Type = TokenType.Command,
                        Value = "Заведи"
                    },
                    new Token
                    {
                        successRate = 90,
                        Type = TokenType.Command,
                        Value = "таймер"
                    },
                    new Token
                    {
                        successRate = 90,
                        Type = TokenType.Command,
                        Value = "на"
                    },
                    new Token
                    {
                        successRate = 90,
                        Type = TokenType.Parameter,
                        Value = "%minutes%"
                    },
                    new Token
                    {
                        successRate = 90,
                        Type = TokenType.Command,
                        Value = "минут"
                    },
                    new Token
                    {
                        successRate = 90,
                        Type = TokenType.Parameter,
                        Value = "%seconds%"
                    },
                    new Token
                    {
                        successRate = 90,
                        Type = TokenType.Command,
                        Value = "секунд"
                    }
                }
            },
            new PluginCommand
            {
                Name = "Stop timers",
                Tokens = new[]
                {
                    new Token
                    {
                        successRate = 90,
                        Type = TokenType.Command,
                        Value = "Останови"
                    },
                    new Token
                    {
                        successRate = 90,
                        Type = TokenType.Command,
                        Value = "таймеры"
                    },
                }
            },
            new PluginCommand
            {
                Name = "Stop all timers",
                Tokens = new[]
                {
                    new Token
                    {
                        successRate = 90,
                        Type = TokenType.Command,
                        Value = "Останови"
                    },
                    new Token
                    {
                        successRate = 90,
                        Type = TokenType.Command,
                        Value = "все"
                    },
                    new Token
                    {
                        successRate = 90,
                        Type = TokenType.Command,
                        Value = "таймеры"
                    },
                }
            },
        };
    }
}
