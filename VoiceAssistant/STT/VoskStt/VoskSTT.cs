using Newtonsoft.Json;

using System;
using System.IO;

using Vosk;

namespace VoiceAssistant.STT.VoskStt
{
    internal class VoskSTT : IStt
    {
        private readonly int VoskLogLevel;
        private readonly string ModelFolder;
        private readonly int AudioInSampleRate;
        private readonly VoskRecognizer _recognizer;

        public VoskSTT(int voskLogLevel, string modelFolder, int audioInSampleRate)
        {
            VoskLogLevel = voskLogLevel;
            ModelFolder = modelFolder;
            AudioInSampleRate = audioInSampleRate;

            _recognizer = InitSpeechToText();
        }

        private VoskRecognizer? InitSpeechToText()
        {
            // set -1 to disable logging messages
            Vosk.Vosk.SetLogLevel(VoskLogLevel);
            VoskRecognizer rec;

            if (!Directory.Exists(ModelFolder))
            {
                Console.WriteLine($"Voice recognition model folder missing: {ModelFolder}");

                return null;
            }

            try
            {
                var model = new Model(ModelFolder);
                rec = new VoskRecognizer(model, AudioInSampleRate);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Can't load model (could it be missing?): {ex.Message}");

                return null;
            }

            rec.SetMaxAlternatives(0);
            rec.SetWords(true);

            return rec;
        }

        public bool StartRecognition(byte[] buffer, int length)
        {
            return _recognizer.AcceptWaveform(buffer, length);
        }

        public VoskResult GetRecognitionResult()
        {
            var jsonResult = _recognizer.Result();

            return JsonConvert.DeserializeObject<VoskResult>(jsonResult);
        }
    }
}
