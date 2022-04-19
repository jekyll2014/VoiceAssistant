// This is an independent project of an individual developer. Dear PVS-Studio, please check it.
// PVS-Studio Static Code Analyzer for C, C++, C#, and Java: http://www.viva64.com
using PluginInterface;

namespace CurrencyRatePlugin
{
    public class CurrencyRatePluginSettings
    {
        public string CurrencyServiceUrl = "http://www.cbr.ru/scripts/XML_daily.asp";
        public string CurrencyDecimalSeparatorWord = "точка";

        //[JsonProperty(Required = Required.Always)]
        public CurrencyRatePluginCommand[] Commands =
        {
            new CurrencyRatePluginCommand
            {
                Name = "Short USD request",
                Tokens = new[]
                {
                    new Token
                    {
                        SuccessRate = 90,
                        Type = TokenType.Command,
                        Value = new[] {"курс"}
                    },
                    new Token
                    {
                        SuccessRate = 90,
                        Type = TokenType.Command,
                        Value = new[] {"доллара"}
                    }
                },
                CurencyCode = "USD",
                RateDecimalRound=1,
                Response = "{1} доллар равен {2} рублей"
            },
            new CurrencyRatePluginCommand
            {
                Name = "Long USD request",
                Tokens = new[]
                {
                    new Token
                    {
                        SuccessRate = 90,
                        Type = TokenType.Command,
                        Value = new[] {"какой"}
                    },
                    new Token
                    {
                        SuccessRate = 90,
                        Type = TokenType.Command,
                        Value = new[] {"курс"}
                    },
                    new Token
                    {
                        SuccessRate = 90,
                        Type = TokenType.Command,
                        Value = new[] {"доллара"}
                    }
                },
                CurencyCode = "USD",
                RateDecimalRound=1,
                Response = "{1} доллар равен {2} рублей"
            },
            new CurrencyRatePluginCommand
            {
                Name = "Short EUR request",
                Tokens = new[]
                {
                    new Token
                    {
                        SuccessRate = 90,
                        Type = TokenType.Command,
                        Value = new[] {"курс"}
                    },
                    new Token
                    {
                        SuccessRate = 90,
                        Type = TokenType.Command,
                        Value = new[] { "евро" }
                    }
                },
                CurencyCode = "EUR",
                RateDecimalRound=1,
                Response = "{1} евро равен {2} рублей"
            },
            new CurrencyRatePluginCommand
            {
                Name = "Long EUR request",
                Tokens = new[]
                {
                    new Token
                    {
                        SuccessRate = 90,
                        Type = TokenType.Command,
                        Value = new[] {"какой"}
                    },
                    new Token
                    {
                        SuccessRate = 90,
                        Type = TokenType.Command,
                        Value = new[] {"курс"}
                    },
                    new Token
                    {
                        SuccessRate = 90,
                        Type = TokenType.Command,
                        Value = new[] {"евро"}
                    }
                },
                CurencyCode = "EUR",
                RateDecimalRound=1,
                Response = "{1} евро равен {2} рублей"
            },
        };
    }
}
