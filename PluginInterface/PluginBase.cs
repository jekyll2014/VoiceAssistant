// This is an independent project of an individual developer. Dear PVS-Studio, please check it.
// PVS-Studio Static Code Analyzer for C, C++, C#, and Java: http://www.viva64.com
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace PluginInterface
{
    public abstract class PluginBase
    {
        public const string PluginInterfaceVersion = "1.1";
        protected readonly string PluginPath;
        protected readonly string PluginConfigFile;
        protected readonly IAudioOutSingleton AudioOut;
        public volatile bool CanAcceptSound = false;
        public volatile bool CanAcceptWords = false;
        public volatile bool CanInjectSound = false;
        public volatile bool CanInjectWords = false;
        private static readonly object SyncRootAudio = new object();
        private static readonly object SyncRootWords = new object();
        private readonly List<string> _recognizedWords = new List<string>();
        private readonly List<byte> _recordedAudio = new List<byte>();
        private readonly int _recognizedWordsBufferLimit = 1024;
        private readonly int _recordedAudioBufferLimit = 1024 * 1024;
        private readonly bool _recognizedWordsAlwaysBuffer = true;
        private readonly bool _recordedAudioAlwaysBuffer = true;

        protected string _pluginName;
        public string PluginName => _pluginName;

        protected string _currentCulture;
        public string CurrentCulture => _currentCulture;

        protected PluginCommand[] _commands;
        public PluginCommand[] Commands => _commands;

        public delegate void ExternalAudioCommandHandler(byte[] buffer, int samplingRate, int bits, int channels);
        public event ExternalAudioCommandHandler ExternalAudioCommand;

        public delegate void ExternalTextCommandHandler(string command);
        public event ExternalTextCommandHandler ExternalTextCommand;

        public delegate void RecordedAudioCommandHandler(byte[] buffer, int samplingRate, int bits, int channels);
        public event RecordedAudioCommandHandler RecordedAudioCommand;

        public delegate void RecordedTextCommandHandler(string command);
        public event RecordedTextCommandHandler RecordedTextCommand;

        protected PluginBase(IAudioOutSingleton audioOut, string currentCulture, string pluginPath)
        {
            var file = new FileInfo(pluginPath);
            AudioOut = audioOut;
            PluginPath = file.DirectoryName;
            _pluginName = file.Name;
            PluginConfigFile = file.Name[..^file.Extension.Length] + "Settings.json";
            _currentCulture = currentCulture;
        }

        public abstract void Execute(string commandName, List<Token> commandTokens);

        public void AddWords(string words)
        {
            var tmpWords = words;
            Task.Run(() =>
            {
                lock (SyncRootWords)
                {
                    if (RecordedTextCommand == null || _recognizedWordsAlwaysBuffer)
                    {
                        var newBuffer = tmpWords.Split(' ');
                        var newBufferLength = _recognizedWords.Count + newBuffer.Length;

                        if (newBufferLength > _recognizedWordsBufferLimit)
                        {
                            var removeWords = newBufferLength - _recognizedWordsBufferLimit;
                            _recognizedWords.RemoveRange(0, removeWords);
                        }

                        _recognizedWords.AddRange(newBuffer);
                    }

                    RecordedTextCommand?.Invoke(words);
                }
            });
        }

        protected string[] GetWords()
        {
            lock (SyncRootWords)
            {
                var result = _recognizedWords.ToArray();
                _recognizedWords.Clear();
                return result;
            }
        }

        public void AddSound(byte[] data, int samplingRate, int bits, int channels)
        {
            var tmpData = data;
            Task.Run(() =>
            {
                lock (SyncRootAudio)
                {
                    if (RecordedAudioCommand == null || _recordedAudioAlwaysBuffer)
                    {
                        var newBufferLength = _recordedAudio.Count + tmpData.Length;

                        if (newBufferLength > _recordedAudioBufferLimit)
                        {
                            var removeBytes = newBufferLength - _recordedAudioBufferLimit;
                            _recordedAudio.RemoveRange(0, removeBytes);
                        }

                        _recordedAudio.AddRange(tmpData);
                    }

                    RecordedAudioCommand?.Invoke(data, samplingRate, bits, channels);
                }
            });
        }

        protected byte[] GetSound()
        {
            lock (SyncRootAudio)
            {
                var result = _recordedAudio.ToArray();
                _recordedAudio.Clear();
                return result;
            }
        }

        protected void InjectTextCommand(string command)
        {
            ExternalTextCommand?.Invoke(command);
        }

        protected void InjectAudioCommand(byte[] buffer, int samplingRate, int bits, int channels)
        {
            ExternalAudioCommand?.Invoke(buffer, samplingRate, bits, channels);
        }
    }
}
