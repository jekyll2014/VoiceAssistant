// This is an independent project of an individual developer. Dear PVS-Studio, please check it.
// PVS-Studio Static Code Analyzer for C, C++, C#, and Java: http://www.viva64.com
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace VoiceAssistant.STT.VoskStt
{
    public class VoskResult
    {
        [JsonPropertyName("result")]
        public List<ResultWord> Result { get; set; }
        [JsonPropertyName("text")]
        public string Text { get; set; }
    }
}
