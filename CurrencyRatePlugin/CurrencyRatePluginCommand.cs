// This is an independent project of an individual developer. Dear PVS-Studio, please check it.
// PVS-Studio Static Code Analyzer for C, C++, C#, and Java: http://www.viva64.com
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
