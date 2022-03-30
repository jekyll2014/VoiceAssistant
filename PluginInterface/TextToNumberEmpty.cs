namespace PluginInterface
{
    public class TextToNumberEmpty : ITextToNumber
    {
        public long ConvertStringToNumber(string numberString, int ratio = 100)
        {
            return 0;
        }
    }
}