// This is an independent project of an individual developer. Dear PVS-Studio, please check it.
// PVS-Studio Static Code Analyzer for C, C++, C#, and Java: http://www.viva64.com
namespace PluginInterface
{
    public interface IAudioOutSingleton
    {
        void PlayFile(string audioFile, bool exclusive = true);
        void Speak(string text, bool exclusive = true);
        void PlayDataBuffer(byte[] data, bool exclusive = true);
    }
}