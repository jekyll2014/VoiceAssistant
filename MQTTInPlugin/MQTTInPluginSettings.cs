// This is an independent project of an individual developer. Dear PVS-Studio, please check it.
// PVS-Studio Static Code Analyzer for C, C++, C#, and Java: http://www.viva64.com
using PluginInterface;

namespace MQTTInPlugin
{
    public class MQTTInPluginSettings
    {
        public string RequestServer = "";
        public int RequestServerPort = 1883;
        public string RequestServerLogin = "";
        public string RequestServerPassword = "";
        public string ClientID = "";
        public bool AddTimeStampToClientID = true;
        public string RequestDeliveryMode = "";
        public bool KeepConnectionAlive = true;
        public int ReconnectTimeOut = 60;

        //[JsonProperty(Required = Required.Always)]
        public MQTTInTopic[] Topics =
        {
            new MQTTInTopic
            {
                MQTTSubscribeTopic = "testTopic",
                DataIsAudio = false
            }
        };
    }
}
