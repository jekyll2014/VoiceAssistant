// Licensed under the MIT license: https://opensource.org/licenses/MIT

using NAudio.Utils;
using NAudio.Wave;

using System;
using System.Collections.Concurrent;
using System.IO;
using System.Net.Http;
using System.Threading.Tasks;

using VoiceAssistant.STT.VoskStt;

using Whisper.net;
using Whisper.net.Ggml;
using Whisper.net.Wave;

namespace VoiceAssistant.STT.Whisper
{
    internal class WhisperStt : IStt
    {
        private GgmlType ggmlType = GgmlType.Small;
        private readonly string _modelFileName;
        private readonly int AudioInSampleRate;

        int maxProcessingTimeMs = 10000;
        int minProcessingTimeMs = 1500;

        int advancingProcessingTimeMs = 500;

        private readonly WaveParser waveParser;
        private WhisperProcessorBuilder builder;
        private ConcurrentQueue<SegmentData> _result = new ConcurrentQueue<SegmentData>();

        public WhisperStt(string modelFileName, int audioInSampleRate)
        {
            //_modelFileName = modelFileName;
            AudioInSampleRate = audioInSampleRate;
            _modelFileName = $"ggml-{ggmlType}.bin";
            InitAsync(_modelFileName).Wait();
        }

        private async Task InitAsync(string modelFileName)
        {
            if (!File.Exists(modelFileName))
            {
                await DownloadModelAsync(modelFileName, ggmlType);
            }

            using var whisperFactory = WhisperFactory.FromPath(modelFileName);

            builder = whisperFactory.CreateBuilder()
                .WithProbabilities()
                .WithLanguage("ru");
        }

        private static async Task DownloadModelAsync(string fileName, GgmlType ggmlType)
        {
            Console.WriteLine($"Downloading Model {fileName}");
            using (var httpClient = new HttpClient())
            {
                var loader = new WhisperGgmlDownloader(httpClient);
                using (var modelStream = await loader.GetGgmlModelAsync(ggmlType))
                {
                    using (var fileWriter = File.OpenWrite(fileName))
                        await modelStream.CopyToAsync(fileWriter);
                }
            }
        }

        public bool StartRecognition(byte[] buffer, int length)
        {
            try
            {
                // Convert audio to the required format for Whisper.net (16kHz, mono, WAV)
                using (var memStream = new MemoryStream())
                {
                    var format = WaveFormat.CreateALawFormat(AudioInSampleRate, 1);
                    using (var writer = new WaveFileWriter(new IgnoreDisposeStream(memStream), format))
                    {
                        //important i am writing the bytes on the fly to the wavefilewriter
                        writer.Write(buffer, 0, length);
                        writer.Flush();
                    }

                    memStream.Position = 0;
                    using (var waveReader = new WaveFileReader(memStream))
                    {
                        var targetFormat = new WaveFormat(16000, 16, 1); // 16kHz, 16-bit, mono
                        using (var conversionStream = new WaveFormatConversionStream(targetFormat, waveReader))
                        {
                            using (var convertedMemoryStream = new MemoryStream())
                            {
                                conversionStream.CopyToAsync(convertedMemoryStream);
                                using (var processor = builder.Build())
                                {
                                    foreach (var data in processor.ProcessAsync(convertedMemoryStream).ToBlockingEnumerable())
                                    {
                                        Console.WriteLine($"Decoded Text: {data.Text}");
                                        _result.Enqueue(data);
                                    }
                                }

                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine($"Error decoding audio: {ex.Message}");
                // Handle exceptions appropriately (e.g., log, display error message)
            }

            return true;
        }

        public VoskResult GetRecognitionResult()
        {
            var result = new VoskResult();
            while (_result.TryDequeue(out var data))
            {
                result.Result.Add(new ResultWord()
                {
                    Word = data.Text,
                    Conf = data.Probability,
                    Start = data.Start.TotalSeconds,
                    End = data.End.TotalSeconds
                });
            }

            return result;
        }
    }
}