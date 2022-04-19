// This is an independent project of an individual developer. Dear PVS-Studio, please check it.
// PVS-Studio Static Code Analyzer for C, C++, C#, and Java: http://www.viva64.com
namespace PluginInterface
{
    public static class TextToNumberFactory
    {
        public static ITextToNumber GetTextToNumberConvertor(string culture)
        {
            if (culture.StartsWith("ru-"))
                return new TextToNumberRus();
            else if (culture.StartsWith("en-"))
                return new TextToNumberEng();
            else
                return new TextToNumberGeneric();
        }
    }
}