using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace PluginInterface
{
    public abstract class PluginBase
    {
        public const string PluginInterfaceVersion = "1.0";
        protected readonly string PluginPath;
        protected readonly string PluginConfigFile;
        protected readonly IAudioOutSingleton AudioOut;
        public volatile bool AcceptsSound = false;
        public volatile bool AcceptsWords = false;
        private static readonly object SyncRootAudio = new object();
        private static readonly object SyncRootWords = new object();
        private readonly List<string> _recognizedWords = new List<string>();
        private readonly List<byte> _recordedAudio = new List<byte>();

        protected string _pluginName;
        public string PluginName => _pluginName;

        protected string _currentCulture;
        public string CurrentCulture => _currentCulture;

        protected PluginCommand[] _commands;
        public PluginCommand[] Commands => _commands;

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
                    _recognizedWords.AddRange(tmpWords.Split(' '));
                }
            });
        }

        public string[] GetWords()
        {
            lock (SyncRootWords)
            {
                var result = _recognizedWords.ToArray();
                _recognizedWords.Clear();
                return result;
            }
        }

        public void AddSound(byte[] data)
        {
            var tmpData = data;
            Task.Run(() =>
            {
                lock (SyncRootAudio)
                {
                    _recordedAudio.AddRange(tmpData);
                }
            });
        }

        public byte[] GetSound()
        {
            lock (SyncRootAudio)
            {
                var result = _recordedAudio.ToArray();
                _recordedAudio.Clear();
                return result;
            }
        }
    }
}
