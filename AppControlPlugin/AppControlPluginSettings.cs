// This is an independent project of an individual developer. Dear PVS-Studio, please check it.
// PVS-Studio Static Code Analyzer for C, C++, C#, and Java: http://www.viva64.com
using PluginInterface;

namespace AppControlPlugin
{
    public class AppControlPluginSettings
    {
        public string KeyNotFound = "Кнопка не найдена";
        public string ProcessNotFound = "Процесс не найден";

        //[JsonProperty(Required = Required.Always)]
        public AppControlPluginCommand[] Commands =
        {
            new AppControlPluginCommand
            {
                Name = "MPC next",
                Tokens = new[]
                {
                    new Token
                    {
                        SuccessRate = 90,
                        Type = TokenType.Command,
                        Value = new[] {"следующий"}
                    },
                    new Token
                    {
                        SuccessRate = 90,
                        Type = TokenType.Command,
                        Value = new[] {"фильм"}
                    }
                },
                ApplicationId = "mpc-hc64",
                KeyNames = new[] { "VK_NEXT" },
                Response = "Перешел на следующий"
            },
            new AppControlPluginCommand
            {
                Name = "MPC previous",
                Tokens = new[]
                {
                    new Token
                    {
                        SuccessRate = 90,
                        Type = TokenType.Command,
                        Value = new[] {"предыдущий"}
                    },
                    new Token
                    {
                        SuccessRate = 90,
                        Type = TokenType.Command,
                        Value = new[] {"фильм"}
                    }
                },
                ApplicationId = "mpc-hc64",
                KeyNames = new[] { "VK_PRIOR" },
                Response = "Вернулся на предыдущий"
            },
            new AppControlPluginCommand
            {
                Name = "MPC play",
                Tokens = new[]
                {
                    new Token
                    {
                        SuccessRate = 90,
                        Type = TokenType.Command,
                        Value = new[] {"Запусти"}
                    },
                    new Token
                    {
                        SuccessRate = 90,
                        Type = TokenType.Command,
                        Value = new[] { "фильм" }
                    }
                },
                ApplicationId = "mpc-hc64",
                KeyNames = new[] { "VK_SPACE" },
                Response = "фильм запущен"
            },
            new AppControlPluginCommand
            {
                Name = "MPC stop",
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
                        Value = new[] { "фильм" }
                    }
                },
                ApplicationId = "mpc-hc64",
                KeyNames = new[] { "VK_DECIMAL" },
                Response = "фильм остановлен"
            },
            new AppControlPluginCommand
            {
                Name = "MPC play/pause",
                Tokens = new[]
                {
                    new Token
                    {
                        SuccessRate = 90,
                        Type = TokenType.Command,
                        Value = new[] { "фильм" }
                    },
                    new Token
                    {
                        SuccessRate = 90,
                        Type = TokenType.Command,
                        Value = new[] { "на" }
                    },
                    new Token
                    {
                        SuccessRate = 90,
                        Type = TokenType.Command,
                        Value = new[] {"паузу"}
                    }
                },
                ApplicationId = "mpc-hc64",
                KeyNames = new[] { "VK_SPACE" },
                Response = "Пауза"
            },
            new AppControlPluginCommand
            {
                Name = "MPC volume up",
                Tokens = new[]
                {
                    new Token
                    {
                        SuccessRate = 90,
                        Type = TokenType.Command,
                        Value = new[] { "сделай" }
                    },
                    new Token
                    {
                        SuccessRate = 90,
                        Type = TokenType.Command,
                        Value = new[] { "громче" }
                    }
                },
                ApplicationId="mpc-hc64",
                KeyNames = new[] { "VK_UP","VK_UP","VK_UP","VK_UP" },
                Response = "сделал громче"
            },
            new AppControlPluginCommand
            {
                Name = "MPC volume down",
                Tokens = new[]
                {
                    new Token
                    {
                        SuccessRate = 90,
                        Type = TokenType.Command,
                        Value = new[] { "сделай" }
                    },
                    new Token
                    {
                        SuccessRate = 90,
                        Type = TokenType.Command,
                        Value = new[] { "тише" }
                    }
                },
                ApplicationId = "mpc-hc64",
                KeyNames = new[] { "VK_DOWN","VK_DOWN","VK_DOWN","VK_DOWN" },
                Response = "сделал тише"
            }
        };
    }
}
