﻿namespace PluginInterface
{
    public static class TextToNumberConvertor
    {
        public static ITextToNumber GetTextToNumberConvertor(string culture)
        {
            if (culture.StartsWith("ru-"))
                return new TextToNumberRus();
            else if (culture.StartsWith("en-"))
                return new TextToNumberEng();
            else
                return new TextToNumberEmpty();
        }
    }

}