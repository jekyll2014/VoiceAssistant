// This is an independent project of an individual developer. Dear PVS-Studio, please check it.
// PVS-Studio Static Code Analyzer for C, C++, C#, and Java: http://www.viva64.com
using PluginInterface;

namespace MQTTOutPlugin
{
    public class MQTTOutPluginSettings
    {
        public string RequestServer = "";
        public int RequestServerPort = 1883;
        public string RequestServerLogin = "";
        public string RequestServerPassword = "";
        public string ClientID = "MQTTOutPlugin";
        public bool AddTimeStampToClientID = true;
        public bool KeepConnectionAlive = true;
        public int ReconnectTimeOut = 60;

        public string FailedConnectionResponse = "подключение не работает";

        //[JsonProperty(Required = Required.Always)]
        public MQTTOutPluginCommand[] Commands =
        {
            new MQTTOutPluginCommand
            {
                Name = "Test",
                Tokens = new[]
                {
                    new Token
                    {
                        SuccessRate = 90,
                        Type = TokenType.Command,
                        Value = new[] {"тест"}
                    },
                    new Token
                    {
                        SuccessRate = 90,
                        Type = TokenType.Command,
                        Value = new[] {"соединения"}
                    }
                },
                PluginResponse = "Соединение установлено",
                MQTTRequestText = "вася привет",
                MQTTRequestTopic = "testTopic"
            }
        };
    }
}
