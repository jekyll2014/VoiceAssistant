using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PluginInterface
{
    public class PluginCommand
    {
        public string Name;
        public Token[] Tokens;

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
                foreach (var token in Tokens)
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

            return sb.ToString().Trim();
        }
    }

    public class Token
    {
        public string[] Value;
        public TokenType Type = TokenType.Unknown;
        public int SuccessRate = 100; // success rato for fuzzy compare
    }

    public enum TokenType
    {
        Unknown,
        Command,
        Parameter
    }
}
