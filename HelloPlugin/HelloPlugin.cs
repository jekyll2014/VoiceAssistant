using System.Collections.Generic;
using System.IO;
using System.Linq;
using PluginInterface;

namespace HelloPlugin
{
    public class HelloPlugin : PluginBase
    {
        private readonly HelloPluginCommand[] HelloCommands;

        public HelloPlugin(IAudioOutSingleton audioOut, string pluginPath) : base(audioOut, pluginPath)
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
        }

        public override string Execute(string commandName, List<Token> commandTokens)
        {
            var command = HelloCommands.FirstOrDefault(n => n.Name == commandName);

            if (command == null)
            {
                return string.Empty;
            }

            AudioOut.Speak(command.Response);

            return command.Response;
        }
    }
}
