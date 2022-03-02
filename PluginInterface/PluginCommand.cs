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
            {
                if (Tokens[i].Value == paramName)
                {
                    return tokens.ElementAt(i);
                }
            }

            return null;
        }

        public override string ToString()
        {
            var sb = new StringBuilder();

            if (Tokens != null && Tokens.Length > 0)
            {
                foreach (var token in Tokens)
                {
                    sb.Append(token.Value + " ");
                }
            }

            return sb.ToString().Trim();
        }
    }

    public class Token
    {
        public string Value;
        public TokenType Type = TokenType.Unknown;
        public int successRate = 100; // success rato for fuzzy compare
    }

    public class ProcessingCommand
    {
        public string PluginName;
        public PluginCommand ExpectedCommand;
        public List<Token> CommandTokens = new List<Token>();
    }

    public enum TokenType
    {
        Unknown,
        Command,
        Parameter
    }
}
