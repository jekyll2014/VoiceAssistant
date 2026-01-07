// This is an independent project of an individual developer. Dear PVS-Studio, please check it.
// PVS-Studio Static Code Analyzer for C, C++, C#, and Java: http://www.viva64.com
using PluginInterface;

using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace VoiceAssistant
{
    public class ProcessingCommand
    {
        public string PluginName { get; }
        public PluginCommand ExpectedCommand { get; }
        public readonly List<Token> CommandTokens = new();

        public ProcessingCommand(string pluginName, PluginCommand expectedCommand)
        {
            PluginName = pluginName;
            ExpectedCommand = expectedCommand;
        }

        public override string ToString()
        {
            var sb = new StringBuilder();
            sb.AppendLine($"Plugin: {PluginName}");
            sb.AppendLine($"\tCommand name: {ExpectedCommand.Name}");
            sb.AppendLine($"\t\tExpected phrase: {ExpectedCommand.ToString()}");
            sb.AppendLine($"\t\tAssembled phrase: {CommandTokensToString()}");

            return sb.ToString();
        }

        public string CommandTokensToString()
        {
            var sb = new StringBuilder();

            if (CommandTokens != null && CommandTokens.Count > 0)
            {
                foreach (var token in CommandTokens.Select(n => n.Value))
                {
                    if (sb.Length > 0)
                    {
                        sb.Append(' ');
                    }

                    if (token.Length == 1)
                    {
                        sb.Append($"{token[0]}");
                    }
                    else
                    {
                        sb.Append('[');
                        var nextElement = false;

                        foreach (var word in token)
                        {
                            if (nextElement)
                            {
                                sb.Append(", ");
                            }

                            sb.Append($"{word}");
                            nextElement = true;
                        }

                        sb.Append(']');
                    }
                }
            }

            return sb.ToString();
        }
    }
}
