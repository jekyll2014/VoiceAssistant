using System.Collections.Generic;

namespace VoiceAssistant
{
    public class VoskResult
    {
        public List<Result> result { get; set; }
        public string text { get; set; }
    }

    public class Result
    {
        public double conf { get; set; }
        public double end { get; set; }
        public double start { get; set; }
        public string word { get; set; }
    }
}
