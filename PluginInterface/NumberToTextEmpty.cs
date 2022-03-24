namespace PluginInterface
{
    public class NumberToTextEmpty : INumberToText
    {
        public string ConvertNumberToString(int val)
        {
            return string.Empty;
        }
    }
}