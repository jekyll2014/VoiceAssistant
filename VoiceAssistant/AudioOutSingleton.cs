using NAudio.Wave;

using PluginInterface;

using System.Globalization;
using System.IO;
using System.Speech.AudioFormat;
using System.Speech.Synthesis;
using System.Threading;

namespace VoiceAssistant
{
    public class AudioOutSingleton : IAudioOutSingleton
    {
        private static AudioOutSingleton _instance;
        private static readonly object SyncRoot = new object();
        private readonly SpeechSynthesizer _synthesizer;
        private readonly PromptBuilder _promptBuilder;
        private readonly WaveOut _waveOut;
        private readonly int _sampleRate;
        private readonly string _speakerLanguage;

        protected AudioOutSingleton(string speakerLanguage, SpeechSynthesizer synthesizer, PromptBuilder promptBuilder,
            WaveOut waveOut, int sampleRate)
        {
            _speakerLanguage = speakerLanguage;
            _synthesizer = synthesizer;
            _promptBuilder = promptBuilder;
            _waveOut = waveOut;
            _sampleRate = sampleRate;
        }

        public static AudioOutSingleton GetInstance(string speakerLanguage, SpeechSynthesizer synthesizer,
            PromptBuilder promptBuilder, WaveOut waveOut, int sampleRate)
        {
            if (_instance != null)
                return _instance;

            lock (SyncRoot)
            {
                _instance ??= new AudioOutSingleton(speakerLanguage, synthesizer, promptBuilder, waveOut,
                    sampleRate);
            }

            return _instance;
        }

        public void Speak(string text, bool exclusive = true)
        {
            if (_instance == null)
                return;

            lock (SyncRoot)
            {
                var speechStream = new MemoryStream();
                var rs = new RawSourceWaveStream(speechStream, new WaveFormat(_sampleRate, 2));
                _synthesizer.SetOutputToAudioStream(speechStream,
                    new SpeechAudioFormatInfo(_sampleRate, AudioBitsPerSample.Sixteen, AudioChannel.Stereo));

                _promptBuilder.StartVoice(new CultureInfo(_speakerLanguage));
                _promptBuilder.AppendText(text);
                _promptBuilder.EndVoice();
                _synthesizer.Speak(_promptBuilder);
                _promptBuilder.ClearContent();

                rs.Position = 0;
                _waveOut.Init(rs);
                _waveOut.Play();

                if (!exclusive)
                    return;

                while (_waveOut.PlaybackState == PlaybackState.Playing)
                {
                    Thread.Sleep(100);
                }
            }
        }

        public void PlayFile(string audioFile, bool exclusive = true)
        {
            if (_instance == null)
                return;

            lock (SyncRoot)
            {
                using (var file = new AudioFileReader(audioFile))
                {
                    _waveOut.Init(file);
                    _waveOut.Play();

                    if (!exclusive)
                        return;

                    while (_waveOut.PlaybackState == PlaybackState.Playing)
                    {
                        Thread.Sleep(100);
                    }
                }
            }
        }

        public void PlayDataBuffer(byte[] data, bool exclusive = true)
        {
            if (_instance == null)
                return;

            lock (SyncRoot)
            {
                using (var provider = new RawSourceWaveStream(new MemoryStream(data), new WaveFormat(16000, 1)))
                {
                    provider.Position = 0;
                    _waveOut.Init(provider);
                    _waveOut.Play();

                    if (!exclusive)
                        return;

                    while (_waveOut.PlaybackState == PlaybackState.Playing)
                    {
                        Thread.Sleep(100);
                    }
                }
            }
        }
    }
}
