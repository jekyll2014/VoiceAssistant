using System.Collections.Generic;

using PluginInterface;

namespace VoiceAssistant
{
    public class ProcessingCommand
    {
        public string PluginName;
        public PluginCommand ExpectedCommand;
        public List<Token> CommandTokens = new List<Token>();
    }
}
