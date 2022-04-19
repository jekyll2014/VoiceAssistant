﻿// This is an independent project of an individual developer. Dear PVS-Studio, please check it.
// PVS-Studio Static Code Analyzer for C, C++, C#, and Java: http://www.viva64.com
using System.Collections.Generic;
using System.IO;
using System.Linq;

using PluginInterface;

namespace HelloPlugin
{
    public class HelloPlugin : PluginBase
    {
        private readonly HelloPluginCommand[] HelloCommands;

        public HelloPlugin(IAudioOutSingleton audioOut, string currentCulture, string pluginPath) : base(audioOut, currentCulture, pluginPath)
        {
            var configBuilder = new Config<HelloPluginSettings>($"{PluginPath}\\{PluginConfigFile}");
            if (!File.Exists($"{PluginPath}\\{PluginConfigFile}"))
            {
                configBuilder.SaveConfig();
            }

            HelloCommands = configBuilder.ConfigStorage.Commands;

            if (HelloCommands is PluginCommand[] newCmds)
            {
                _commands = newCmds;
            }

            // example of listening to the audio/word stream from core module.
            //base.AcceptsSound = true;
            //base.AcceptsWords = true;
        }

        public override void Execute(string commandName, List<Token> commandTokens)
        {
            var command = HelloCommands.FirstOrDefault(n => n.Name == commandName);

            if (command == null)
            {
                return;
            }

            // example of listening to the audio/word stream from core module.
            /*
            var sndData = GetSound();
            AudioOut.PlayDataBuffer(sndData);
            
            var wordsData = "";            
            foreach (var w in GetWords())
            {
                wordsData += " " + w;
            }
            Console.WriteLine($"ReceivedText: {wordsData}");
            */

            AudioOut.Speak(command.Response);
        }
    }
}
