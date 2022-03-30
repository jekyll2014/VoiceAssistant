namespace PluginInterface
{
    public interface ITextToNumber
    {
        public abstract long ConvertStringToNumber(string numberString, int ratio = 100);
    }
}