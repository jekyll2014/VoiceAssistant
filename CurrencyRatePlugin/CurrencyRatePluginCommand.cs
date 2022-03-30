using PluginInterface;

namespace CurrencyRatePlugin
{
    public class CurrencyRatePluginCommand : PluginCommand
    {
        public string CurencyCode = "";
        public int RateDecimalRound = 2;
        public string Response = "";
    }
}
