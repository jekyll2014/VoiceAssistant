﻿using PluginInterface;

namespace RunProgramPlugin
{
    public class RunProgramPluginCommand : PluginCommand
    {
        public string Response = "";
        public string CommandLine = "";
        public bool IsStopCommand = false;
        public bool AllowMultiple = false;
    }

    public class RunProgramPluginSettings
    {
        //[JsonProperty(Required = Required.Always)]
        public string CanNotClose = "не удалось закрыть";
        public string NotRunning = "не запущен";
        public string CanNotRun = "не удалось запустить";
        public string AlreadyRunning = "уже запущен";

        public RunProgramPluginCommand[] Commands { get; set; } =
        {
            new RunProgramPluginCommand
            {
                Name = "Run VSCode",
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
                        Value = new[] {"вес"}
                    },
                    new Token
                    {
                        SuccessRate = 90,
                        Type = TokenType.Command,
                        Value = new[] {"код"}
                    }
                },
                Response = "Вижуал студио код запущена",
                CommandLine = "code",
                IsStopCommand = false,
                AllowMultiple = true
            },
            new RunProgramPluginCommand
            {
                Name = "Stop VSCode",
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
                        Value = new[] {"вес"}
                    },
                    new Token
                    {
                        SuccessRate = 90,
                        Type = TokenType.Command,
                        Value = new[] {"код"}
                    }
                },
                Response = "Вижуал студио код закрыт",
                CommandLine = "code",
                IsStopCommand = true,
                AllowMultiple = true
            },
            new RunProgramPluginCommand
            {
                Name = "Run notepad++",
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
                        Value = new[] {"блокнот"}
                    }
                },
                Response = "Блокнот запущен",
                CommandLine = "notepad++.exe",
                IsStopCommand = false,
                AllowMultiple = false
            },
            new RunProgramPluginCommand
            {
                Name = "Stop notepad++",
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
                        Value = new[] { "блокнот" }
                    }
                },
                Response = "Блокнот закрыт",
                CommandLine = "notepad++.exe",
                IsStopCommand = true,
                AllowMultiple = false
            }
        };
    }
}