// This is an independent project of an individual developer. Dear PVS-Studio, please check it.
// PVS-Studio Static Code Analyzer for C, C++, C#, and Java: http://www.viva64.com
namespace MQTTInPlugin
{
    public class MQTTInTopic
    {
        public string MQTTSubscribeTopic = "";
        public bool DataIsAudio = false;
        public int samplingRate = 16000;
        public int bits = 16;
        public int channels = 1;
    }
}
