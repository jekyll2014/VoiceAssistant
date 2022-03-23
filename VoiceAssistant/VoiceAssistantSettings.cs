namespace VoiceAssistant
{
    public class VoiceAssistantSettings
    {
        //[JsonProperty(Required = Required.Always)]
        public string ModelFolder { get; set; } = "model";
        public string SelectedAudioInDevice { get; set; } = "";
        public string SelectedAudioOutDevice { get; set; } = "";
        public int AudioInSampleRate { get; set; } = 16000;
        public int AudioOutSampleRate { get; set; } = 44100;
        public string[] CallSign { get; set; } = {"Вася"};
        public int DefaultSuccessRate { get; set; } = 90;
        public string VoiceName { get; set; } = "Aleksandr";
        public string SpeakerCulture { get; set; } = "ru-RU";
        public string PluginsFolder { get; set; } = "plugins";
        public string PluginFileMask { get; set; } = "*Plugin.dll";
        public string StartSound { get; set; } = "AssistantStart.wav";
        public string MisrecognitionSound { get; set; } = "Misrecognition.wav";
        public int CommandAwaitTime { get; set; } = 10;
        public int NextWordAwaitTime { get; set; } = 3;
        public int VoskLogLevel { get; set; } = -1;
        public string CommandNotRecognizedMessage { get; set; } = "Команда не распознана";
        public string CommandNotFoundMessage { get; set; } = "Команда не найдена";
        public bool AllowPluginsToListenToSound { get; set; } = false;
        public bool AllowPluginsToListenToWords { get; set; } = false;
    }
}
