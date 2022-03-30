namespace PluginInterface
{
    public class NumberToTextEmpty : INumberToText
    {
        public string ConvertNumberToString(long val)
        {
            return string.Empty;
        }
    }
}