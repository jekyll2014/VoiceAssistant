using PluginInterface;

using System.Collections.Generic;
using System.IO;

namespace HelloPlugin
{
    public class HelloPlugin : PluginBase, IPluginInterface
    {
        public string PluginName { get; } = "HelloPlugin";
        private const string _pluginConfigFile = "HelloPluginSettings.json";
        public PluginCommand[] Commands { get; set; }

        public HelloPlugin(IAudioOutSingleton audioOut) : base(audioOut)
        {
            var configBuilder = new Config<HelloPluginSettings>(_pluginConfigFile);
            if (!File.Exists(_pluginConfigFile))
            {
                configBuilder.SaveConfig();
            }

            Commands = configBuilder.ConfigStorage.Commands;
        }

        public string Execute(string commandName, IEnumerable<Token> commandTokens)
        {
            _audioOut.Speak("И тебе привет");

            return "И тебе привет";
        }
    }
}
