using System.Collections.Generic;

namespace PluginInterface
{
    public interface IPluginInterface
    {
        public string PluginName { get; }
        public PluginCommand[] Commands { get; set; }
        public string Execute(string commandName, IEnumerable<Token> commandTokens);
    }

    public abstract class PluginBase
    {
        protected PluginBase(IAudioOutSingleton audioOut)
        {
            _audioOut = audioOut;
        }

        protected readonly IAudioOutSingleton _audioOut;
    }
}
