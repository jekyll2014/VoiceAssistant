// This is an independent project of an individual developer. Dear PVS-Studio, please check it.
// PVS-Studio Static Code Analyzer for C, C++, C#, and Java: http://www.viva64.com

using PluginInterface.Converters.Eng;
using PluginInterface.Converters.Generic;
using PluginInterface.Converters.Rus;
using PluginInterface.Interfaces;

namespace PluginInterface.Converters
{
    public static class NumberToTextFactory
    {
        public static INumberToText GetNumberToTextConvertor(string culture)
        {
            if (culture.StartsWith("ru-"))
                return new NumberToTextRus();
            else if (culture.StartsWith("en-"))
                return new NumberToTextEng();
            else
                return new NumberToTextGeneric();
        }
    }
}