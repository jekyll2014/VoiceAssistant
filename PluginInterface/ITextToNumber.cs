namespace PluginInterface
{
    public interface ITextToNumber
    {
        public abstract int ConvertStringToNumber(string numberString, int ratio = 100);
    }
}