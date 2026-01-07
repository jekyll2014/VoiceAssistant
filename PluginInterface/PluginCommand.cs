// This is an independent project of an individual developer. Dear PVS-Studio, please check it.
// PVS-Studio Static Code Analyzer for C, C++, C#, and Java: http://www.viva64.com
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PluginInterface
{
    public class PluginCommand
    {
        public string Name { get; set; }
        public Token[] Tokens { get; set; }

        public Token GetParameter(string paramName, IEnumerable<Token> tokens)
        {
            for (var i = 0; i < Tokens.Length; i++)
                if (Tokens[i].Value.Contains(paramName))
                    return tokens.ElementAt(i);

            return null;
        }

        public override string ToString()
        {
            var sb = new StringBuilder();

            if (Tokens != null && Tokens.Length > 0)
            {
                foreach (var token in Tokens.Select(n => n.Value))
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

            return sb.ToString().Trim();
        }
    }
}
