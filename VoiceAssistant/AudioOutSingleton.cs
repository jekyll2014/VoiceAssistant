using NAudio.Wave;

using PluginInterface;

using System;
using System.Globalization;
using System.IO;
using System.Speech.AudioFormat;
using System.Speech.Synthesis;
using System.Threading;

namespace VoiceAssistant
{
    public class AudioOutSingleton : IAudioOutSingleton
    {
        private static AudioOutSingleton instance;
        private static object syncRoot = new Object();
        private static SpeechSynthesizer _synthesizer;
        private static PromptBuilder _promptBuilder;
        private static WaveOut _waveOut;
        private static int _sampleRate;
        private static string _speakerLanguage;

        protected AudioOutSingleton(string speakerLanguage, SpeechSynthesizer synthesizer, PromptBuilder promptBuilder, WaveOut waveOut, int sampleRate)
        {
            _speakerLanguage = speakerLanguage;
            _synthesizer = synthesizer;
            _promptBuilder = promptBuilder;
            _waveOut = waveOut;
            _sampleRate = sampleRate;
        }

        public static AudioOutSingleton GetInstance(string speakerLanguage, SpeechSynthesizer synthesizer, PromptBuilder promptBuilder, WaveOut waveOut, int sampleRate)
        {
            if (instance == null)
            {
                lock (syncRoot)
                {
                    if (instance == null)
                        instance = new AudioOutSingleton(speakerLanguage, synthesizer, promptBuilder, waveOut, sampleRate);
                }
            }

            return instance;
        }

        public void Speak(string text, bool exclusive = true)
        {
            if (instance != null)
            {
                lock (syncRoot)
                {
                    var speechStream = new MemoryStream();
                    var rs = new RawSourceWaveStream(speechStream, new WaveFormat(_sampleRate, 2));
                    _synthesizer.SetOutputToAudioStream(speechStream, new SpeechAudioFormatInfo(_sampleRate, AudioBitsPerSample.Sixteen, AudioChannel.Stereo));

                    _promptBuilder.StartVoice(new CultureInfo(_speakerLanguage));
                    _promptBuilder.AppendText(text);
                    _promptBuilder.EndVoice();
                    _synthesizer.Speak(_promptBuilder);
                    _promptBuilder.ClearContent();

                    rs.Position = 0;
                    _waveOut.Init(rs);
                    _waveOut.Play();

                    if (exclusive)
                    {
                        while (_waveOut.PlaybackState == PlaybackState.Playing)
                        {
                            Thread.Sleep(100);
                        }
                    }
                }
            }
        }

        public void PlayFile(string audioFile, bool exclusive = true)
        {
            if (instance != null)
            {
                lock (syncRoot)
                {
                    using (var file = new AudioFileReader(audioFile))
                    {
                        _waveOut.Init(file);
                        _waveOut.Play();

                        if (exclusive)
                        {
                            while (_waveOut.PlaybackState == PlaybackState.Playing)
                            {
                                Thread.Sleep(100);
                            }
                        }
                    }
                }
            }
        }
    }
}
