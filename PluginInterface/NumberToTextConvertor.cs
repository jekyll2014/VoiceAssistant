namespace PluginInterface
{
    public static class NumberToTextConvertor
    {
        public static INumberToText GetNumberToTextConvertor(string culture)
        {
            if (culture.StartsWith("ru-"))
                return new NumberToTextRus();
            else if (culture.StartsWith("en-"))
                return new NumberToTextEng();
            else
                return new NumberToTextEmpty();
        }
    }
}