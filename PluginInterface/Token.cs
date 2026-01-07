namespace PluginInterface;

public class Token
{
    public string[] Value { get; set; }
    public TokenType Type { get; set; } = TokenType.Unknown;
    public int SuccessRate { get; set; } = 100; // success ratio for fuzzy compare
}