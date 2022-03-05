using System.Collections.Generic;
using System.IO;

namespace PluginInterface
{
    public abstract class PluginBase
    {
        protected string _pluginName;
        public string PluginName => _pluginName;
        protected readonly string PluginPath;
        protected readonly string PluginConfigFile;
        protected PluginCommand[] _commands;
        public PluginCommand[] Commands => _commands;
        protected readonly IAudioOutSingleton AudioOut;

        protected PluginBase(IAudioOutSingleton audioOut, string pluginPath)
        {
            var file = new FileInfo(pluginPath);
            AudioOut = audioOut;
            PluginPath = file.DirectoryName;
            _pluginName = file.Name;
            PluginConfigFile = file.Name[..^file.Extension.Length] + "Settings.json";
        }

        public abstract string Execute(string commandName, List<Token> commandTokens);
    }
}
