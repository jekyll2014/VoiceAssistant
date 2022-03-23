using System.Collections.Generic;
using System.Text;

using PluginInterface;

namespace VoiceAssistant
{
    public class ProcessingCommand
    {
        public string PluginName;
        public PluginCommand ExpectedCommand;
        public List<Token> CommandTokens = new List<Token>();

        public override string ToString()
        {
            var sb = new StringBuilder();
            sb.AppendLine($"Plugin: {PluginName}");
            sb.AppendLine($"  Command name: {ExpectedCommand.Name}");
            sb.AppendLine($"    Expected phrase: {ExpectedCommand.ToString()}");
            sb.AppendLine($"    Assembled phrase: {CommandTokensToString()}");

            return sb.ToString();
        }

        public string CommandTokensToString()
        {
            var sb = new StringBuilder();

            if (CommandTokens != null && CommandTokens.Count > 0)
            {
                foreach (var token in CommandTokens)
                {
                    if (sb.Length > 0)
                    {
                        sb.Append(" ");
                    }

                    if (token.Value.Length == 1)
                    {
                        sb.Append($"{token.Value[0]}");
                    }
                    else
                    {
                        sb.Append("[");
                        var nextElement = false;

                        foreach (var word in token.Value)
                        {
                            if (nextElement)
                            {
                                sb.Append(", ");
                            }

                            sb.Append($"{word}");
                            nextElement = true;
                        }

                        sb.Append("]");
                    }
                }
            }

            return sb.ToString();
        }

    }
}
