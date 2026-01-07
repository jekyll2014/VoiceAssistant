using System.Text.Json.Serialization;

namespace VoiceAssistant.STT.VoskStt;

public class ResultWord
{
    [JsonPropertyName("conf")]
    public double Conf { get; set; }
    [JsonPropertyName("end")]
    public double End { get; set; }
    [JsonPropertyName("start")]
    public double Start { get; set; }
    [JsonPropertyName("word")]
    public string Word { get; set; }
}