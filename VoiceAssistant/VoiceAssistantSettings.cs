namespace VoiceAssistant
{
    public class VoiceAssistantSettings
    {
        //[JsonProperty(Required = Required.Always)]
        public string ModelFolder = "model";
        public string SelectedAudioInDevice = "";
        public string SelectedAudioOutDevice = "";
        public int AudioInSampleRate = 16000;
        public int AudioOutSampleRate = 44100;
        public string[] CallSign = { "Вася" };
        public int DefaultSuccessRate = 90;
        public string VoiceName = "";
        public string SpeakerCulture = "ru-RU";
        public string PluginsFolder = "plugins";
        public string PluginFileMask = "*Plugin.dll";
        public string StartSound = "AssistantStart.wav";
        public string MisrecognitionSound = "Misrecognition.wav";
        public int CommandAwaitTime = 10;
        public int NextWordAwaitTime = 3;
        public int VoskLogLevel = -1;
        public string CommandNotRecognizedMessage = "Команда не распознана";
        public string CommandNotFoundMessage = "Команда не найдена";
        public bool AllowPluginsToListenToSound = false;
        public bool AllowPluginsToListenToWords = false;
    }
}
