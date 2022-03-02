using PluginInterface;

namespace HelloPlugin
{
    public class HelloPluginSettings
    {
        //[JsonProperty(Required = Required.Always)]
        public PluginCommand[] Commands { get; set; } = new PluginCommand[]
        {
            new PluginCommand
            {
                Name = "Greeting informal",
                Tokens = new[]
                {
                    new Token
                    {
                        successRate = 90,
                        Type = TokenType.Command,
                        Value = "привет"
                    }
                }
            },
            new PluginCommand
            {
                Name = "Greeting formal",
                Tokens = new[]
                {
                    new Token
                    {
                        successRate = 90,
                        Type = TokenType.Command,
                        Value = "Здравствуй",
                    }
                }
            },
            new PluginCommand
            {
                Name = "Greeting wishing",
                Tokens = new[]
                {
                    new Token
                    {
                        successRate = 90,
                        Type = TokenType.Command,
                        Value =  "Доброго",
                    }
                }
            },
            new PluginCommand
            {
                Name = "Greeting short informal",
                Tokens = new[]
                {
                    new Token
                    {
                        successRate = 90,
                        Type = TokenType.Command,
                        Value = "Хай"
                    }
                }
            }
        };
    }
}
