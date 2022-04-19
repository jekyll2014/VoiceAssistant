// This is an independent project of an individual developer. Dear PVS-Studio, please check it.
// PVS-Studio Static Code Analyzer for C, C++, C#, and Java: http://www.viva64.com
using PluginInterface;

namespace HelloPlugin
{
    public class HelloPluginSettings
    {
        //[JsonProperty(Required = Required.Always)]
        public HelloPluginCommand[] Commands =
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
