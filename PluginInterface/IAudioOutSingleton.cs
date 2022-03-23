namespace PluginInterface
{
    public interface IAudioOutSingleton
    {
        void PlayFile(string audioFile, bool exclusive = true);
        void Speak(string text, bool exclusive = true);
        void PlayDataBuffer(byte[] data, bool exclusive = true);
    }
}