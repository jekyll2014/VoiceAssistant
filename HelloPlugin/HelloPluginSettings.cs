using PluginInterface;

namespace HelloPlugin
{
    public class HelloPluginCommand : PluginCommand
    {
        public string Response = "";
    }

    public class HelloPluginSettings
    {
        //[JsonProperty(Required = Required.Always)]
        public HelloPluginCommand[] Commands { get; set; } =
        {
            new HelloPluginCommand
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
            new HelloPluginCommand
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
            new HelloPluginCommand
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
            new HelloPluginCommand
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
