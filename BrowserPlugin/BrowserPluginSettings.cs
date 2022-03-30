using PluginInterface;

namespace BrowserPlugin
{
    public class BrowserPluginSettings
    {
        //[JsonProperty(Required = Required.Always)]
        public string CanNotClose = "не удалось закрыть";
        public string NotRunning = "не запущен";
        public string CanNotRun = "не удалось запустить";
        public string AlreadyRunning = "уже запущен";

        public BrowserPluginCommand[] Commands =
        {
            new BrowserPluginCommand
            {
                Name = "Run Yandex",
                Tokens = new[]
                {
                    new Token
                    {
                        SuccessRate = 90,
                        Type = TokenType.Command,
                        Value = new[] {"запусти", "открой"}
                    },
                    new Token
                    {
                        SuccessRate = 90,
                        Type = TokenType.Command,
                        Value = new[] {"яндекс"}
                    }
                },
                Response = "Яндекс запущен",
                URL = "www.yandex.ru",
                isStopCommand = false,
                useStandAloneBrowser = true
            },
            new BrowserPluginCommand
            {
                Name = "Stop Yandex",
                Tokens = new[]
                {
                    new Token
                    {
                        SuccessRate = 90,
                        Type = TokenType.Command,
                        Value = new[] {"останови", "закрой"}
                    },
                    new Token
                    {
                        SuccessRate = 90,
                        Type = TokenType.Command,
                        Value = new[] {"яндекс"}
                    }
                },
                Response = "Яндекс закрыт",
                URL = "www.yandex.ru",
                isStopCommand = true,
                useStandAloneBrowser = true
            },
            new BrowserPluginCommand
            {
                Name = "Run Google",
                Tokens = new[]
                {
                    new Token
                    {
                        SuccessRate = 90,
                        Type = TokenType.Command,
                        Value = new[] {"запусти", "открой" }
                    },
                    new Token
                    {
                        SuccessRate = 90,
                        Type = TokenType.Command,
                        Value = new[] {"гугл"}
                    }
                },
                Response = "Гугл запущен",
                URL = "www.google.com",
                isStopCommand = false,
                useStandAloneBrowser = false
            },
            new BrowserPluginCommand
            {
                Name = "Stop Google",
                Tokens = new[]
                {
                    new Token
                    {
                        SuccessRate = 90,
                        Type = TokenType.Command,
                        Value = new[] {"останови", "закрой"}
                    },
                    new Token
                    {
                        SuccessRate = 90,
                        Type = TokenType.Command,
                        Value = new[] { "гугл" }
                    }
                },
                Response = "Гугл закрыт",
                URL = "www.google.com",
                isStopCommand = true,
                useStandAloneBrowser = false
            },
            new BrowserPluginCommand
            {
                Name = "Run Kinopoisk",
                Tokens = new[]
                {
                    new Token
                    {
                        SuccessRate = 90,
                        Type = TokenType.Command,
                        Value = new[] {"запусти", "открой" }
                    },
                    new Token
                    {
                        SuccessRate = 90,
                        Type = TokenType.Command,
                        Value = new[] {"кинопоиск"}
                    }
                },
                Response = "Кинопоиск запущен",
                URL = "www.kinopoisk.ru",
                isStopCommand = false,
                useStandAloneBrowser = false
            },
            new BrowserPluginCommand
            {
                Name = "Stop Kinopoisk",
                Tokens = new[]
                {
                    new Token
                    {
                        SuccessRate = 90,
                        Type = TokenType.Command,
                        Value = new[] {"останови", "закрой"}
                    },
                    new Token
                    {
                        SuccessRate = 90,
                        Type = TokenType.Command,
                        Value = new[] { "кинопоиск" }
                    }
                },
                Response = "Кинопоиск закрыт",
                URL = "www.kinopoisk.ru",
                isStopCommand = true,
                useStandAloneBrowser = false
            }
        };
    }
}
