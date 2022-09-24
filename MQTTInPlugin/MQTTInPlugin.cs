// This is an independent project of an individual developer. Dear PVS-Studio, please check it.
// PVS-Studio Static Code Analyzer for C, C++, C#, and Java: http://www.viva64.com
using MQTTnet;
using MQTTnet.Client;
using MQTTnet.Diagnostics;
using MQTTnet.Implementations;

using PluginInterface;

using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace MQTTInPlugin
{
    public class MQTTInPlugin : PluginBase, IDisposable
    {
        private readonly MQTTInTopic[] MQTTInTopics;
        public string _requestServer = "";
        public int _requestServerPort = 1883;
        public string _requestServerLogin = "";
        public string _requestServerPassword = "";
        public string _clientID = "MQTTInPlugin";
        public bool _addTimeStampToClientID = true;
        public string _requestDeliveryMode = "";
        public string _requestTopic = "";
        public int _reconnectTimeOut = 60000;

        public string _failedConnectionResponse = "";

        private static readonly IMqttNetLogger Logger = new MqttNetEventLogger();
        private readonly IMqttClient _mqttClient = new MqttClient(new MqttClientAdapterFactory(), Logger);
        private readonly System.Timers.Timer reconnectTimer = new System.Timers.Timer();

        public MQTTInPlugin(IAudioOutSingleton audioOut, string currentCulture, string pluginPath) : base(audioOut, currentCulture, pluginPath)
        {
            var configBuilder = new Config<MQTTInPluginSettings>($"{PluginPath}\\{PluginConfigFile}");
            if (!File.Exists($"{PluginPath}\\{PluginConfigFile}"))
            {
                configBuilder.SaveConfig();
            }

            _requestServer = configBuilder.ConfigStorage.RequestServer;
            _requestServerPort = configBuilder.ConfigStorage.RequestServerPort;
            _requestServerLogin = configBuilder.ConfigStorage.RequestServerLogin;
            _requestServerPassword = configBuilder.ConfigStorage.RequestServerPassword;
            _clientID = configBuilder.ConfigStorage.ClientID;
            _addTimeStampToClientID = configBuilder.ConfigStorage.AddTimeStampToClientID;
            _requestDeliveryMode = configBuilder.ConfigStorage.RequestDeliveryMode;
            _reconnectTimeOut = configBuilder.ConfigStorage.ReconnectTimeOut * 1000;

            MQTTInTopics = configBuilder.ConfigStorage.Topics;

            _mqttClient.ApplicationMessageReceivedAsync += Mqtt_DataReceived;

            reconnectTimer.Interval = _reconnectTimeOut;
            reconnectTimer.Elapsed += Timer_reconnect_Tick;
            reconnectTimer.AutoReset = true;

            ConnectMQTT(false);

            base.CanInjectSound = true;
            base.CanInjectWords = true;
        }

        public override void Execute(string commandName, List<Token> commandTokens)
        {
        }

        private async Task<bool> ConnectMQTT(bool waitConnection)
        {
            var options = new MqttClientOptionsBuilder()
                .WithClientId(_clientID + (_addTimeStampToClientID
                    ? DateTime.Now.ToString(CultureInfo.InvariantCulture)
                    : ""))
                .WithTcpServer(_requestServer, _requestServerPort)
                .WithCredentials(_requestServerLogin, _requestServerPassword)
                .WithCleanSession()
                .Build();

            await Task.Run(async () =>
            {
                try
                {
                    reconnectTimer.Enabled = false;

                    await _mqttClient.ConnectAsync(options, CancellationToken.None).ConfigureAwait(true);
                    Console.WriteLine("MQTT connected");
                }
                catch (Exception ex)
                {
                    Console.WriteLine("MQTT connection exception: " + ex.Message);
                    await DisconnectMQTT();

                    return;
                }

                if (!_mqttClient.IsConnected)
                    await DisconnectMQTT();
            }).ConfigureAwait(waitConnection);

            if (!_mqttClient.IsConnected)
            {
                await DisconnectMQTT();
                return false;
            }

            _mqttClient.DisconnectedAsync -= MqttDisconnectedHandler;
            _mqttClient.DisconnectedAsync += MqttDisconnectedHandler;

            await SubscribeMQTT(MQTTInTopics.Select(n => n.MQTTSubscribeTopic));

            return true;
        }

        private async Task DisconnectMQTT()
        {
            _mqttClient.DisconnectedAsync -= MqttDisconnectedHandler;

            try
            {
                await _mqttClient.DisconnectAsync().ConfigureAwait(true);
            }
            catch (Exception ex)
            {
                Console.WriteLine("Disconnect error: " + ex);
                return;
            }

            Console.WriteLine("MQTT disconnected");
        }

        private Task MqttDisconnectedHandler(MqttClientDisconnectedEventArgs e)
        {
            //var arguments = e;
            Console.WriteLine("MQTT disconnected. Reconnecting...");

            DisconnectMQTT();

            reconnectTimer.Start();

            return new Task(() => { });
        }

        private async void Timer_reconnect_Tick(object sender, EventArgs e)
        {
            reconnectTimer.Stop();
            Console.WriteLine("Trying to reconnect...");
            var result = await ConnectMQTT(true);

            if (!result)
                reconnectTimer.Start();
        }

        private async Task<bool> SubscribeMQTT(IEnumerable<string> topics)
        {
            try
            {
                foreach (var topic in topics)
                {
                    if (!string.IsNullOrEmpty(topic))
                    {
                        var result = await SubscribeMQTT(topic);

                        if (result)
                            Console.WriteLine($"Topic subscribed: {topic}");
                        else
                            Console.WriteLine($"Topic not subscribed: {topic}");
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error subscribing topic: {ex.Message}");
                return false;
            }

            return true;
        }

        private async Task<bool> SubscribeMQTT(string topic)
        {
            if (!string.IsNullOrEmpty(topic))
            {
                try
                {
                    await _mqttClient
                    .SubscribeAsync(new MqttTopicFilterBuilder()
                    .WithTopic(_requestTopic)
                    .Build())
                    .ConfigureAwait(true);
                }
                catch (Exception ex)
                {
                    Console.WriteLine("MQTT topic subscribe exception: " + ex.Message);

                    return false;
                }
            }
            else
            {
                return false;
            }

            return true;
        }

        private Task Mqtt_DataReceived(MqttApplicationMessageReceivedEventArgs e)
        {
            var arguments = e;
            if (arguments?.ApplicationMessage?.Payload == null || arguments.ApplicationMessage.Payload.Length == 0)
                return new Task(() => { });

            foreach (var topic in MQTTInTopics)
            {
                if (topic.MQTTSubscribeTopic == arguments.ApplicationMessage.Topic)
                {
                    if (!topic.DataIsAudio)
                    {
                        var newCommand = Encoding.UTF8.GetString(arguments.ApplicationMessage.Payload);
                        InjectTextCommand(newCommand);
                    }
                    else
                    {
                        InjectAudioCommand(arguments.ApplicationMessage.Payload, topic.samplingRate, topic.bits, topic.channels);
                    }
                }
            }

            return new Task(() => { });
        }

        public void Dispose()
        {
            _mqttClient?.Dispose();
            reconnectTimer?.Dispose();
        }
    }
}
