// This is an independent project of an individual developer. Dear PVS-Studio, please check it.
// PVS-Studio Static Code Analyzer for C, C++, C#, and Java: http://www.viva64.com
using PluginInterface.Interfaces;

namespace PluginInterface.Converters.Empty
{
    public class TextToNumberEmpty : ITextToNumber
    {
        public long ConvertStringToNumber(string numberString, int ratio = 100)
        {
            return 0;
        }
    }
}