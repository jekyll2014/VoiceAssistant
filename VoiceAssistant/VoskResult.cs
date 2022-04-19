// This is an independent project of an individual developer. Dear PVS-Studio, please check it.
// PVS-Studio Static Code Analyzer for C, C++, C#, and Java: http://www.viva64.com
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
