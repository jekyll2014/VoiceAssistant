using PluginInterface;

namespace HelloPlugin
{
    public class HelloPluginSettings
    {
        //[JsonProperty(Required = Required.Always)]
        public PluginCommand[] Commands { get; set; } =
        {
            new PluginCommand
            {
                Name = "Greeting informal",
                Tokens = new[]
                {
                    new Token
                    {
                        SuccessRate = 90,
                        Type = TokenType.Command,
                        Value = new[] {"привет"}
                    }
                },
                Response = "И тебе привет"
            },
            new PluginCommand
            {
                Name = "Greeting formal",
                Tokens = new[]
                {
                    new Token
                    {
                        SuccessRate = 90,
                        Type = TokenType.Command,
                        Value = new[] {"Здравствуй"}
                    }
                },
                Response = "И тебе не болеть"
            },
            new PluginCommand
            {
                Name = "Greeting wishing",
                Tokens = new[]
                {
                    new Token
                    {
                        SuccessRate = 90,
                        Type = TokenType.Command,
                        Value = new[] {"Доброго", "Добрый"}
                    },
                    new Token
                    {
                        SuccessRate = 90,
                        Type = TokenType.Command,
                        Value = new[] {"дня", "день"}
                    }
                },
                Response = "И тебе доброго дня"
            },
            new PluginCommand
            {
                Name = "Greeting short informal",
                Tokens = new[]
                {
                    new Token
                    {
                        SuccessRate = 90,
                        Type = TokenType.Command,
                        Value = new[] {"Хай"}
                    }
                },
                Response = "Хаюшки"
            }
        };
    }
}
