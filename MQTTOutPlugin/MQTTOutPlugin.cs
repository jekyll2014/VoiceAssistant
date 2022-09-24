﻿// This is an independent project of an individual developer. Dear PVS-Studio, please check it.
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
using System.Threading;
using System.Threading.Tasks;

namespace MQTTOutPlugin
{
    public class MQTTOutPlugin : PluginBase, IDisposable
    {
        private readonly MQTTOutPluginCommand[] MQTTOutCommands;
        public string _requestServer = "";
        public int _requestServerPort = 1883;
        public string _requestServerLogin = "";
        public string _requestServerPassword = "";
        public string _clientID = "MQTTOutPlugin";
        public bool _addTimeStampToClientID = true;
        public string _requestDeliveryMode = "";
        public string _requestTopic = "";
        public int _reconnectTimeOut = 60000;

        public string _failedConnectionResponse = "";

        private static readonly IMqttNetLogger Logger = new MqttNetEventLogger();
        private readonly IMqttClient _mqttClient = new MqttClient(new MqttClientAdapterFactory(), Logger);
        private readonly System.Timers.Timer reconnectTimer = new System.Timers.Timer();

        public MQTTOutPlugin(IAudioOutSingleton audioOut, string currentCulture, string pluginPath) : base(audioOut, currentCulture, pluginPath)
        {
            var configBuilder = new Config<MQTTOutPluginSettings>($"{PluginPath}\\{PluginConfigFile}");
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
            _failedConnectionResponse = configBuilder.ConfigStorage.FailedConnectionResponse;
            _reconnectTimeOut = configBuilder.ConfigStorage.ReconnectTimeOut * 1000;

            MQTTOutCommands = configBuilder.ConfigStorage.Commands;

            if (MQTTOutCommands is PluginCommand[] newCmds)
            {
                _commands = newCmds;
            }

            reconnectTimer.Interval = _reconnectTimeOut;
            reconnectTimer.Elapsed += Timer_reconnect_Tick;
            reconnectTimer.AutoReset = true;

            ConnectMQTT(false);

            // example of listening to the audio/word stream from core module.
            // base.CanAcceptSound = true;
            // base.CanAcceptWords = true;
        }

        public override void Execute(string commandName, List<Token> commandTokens)
        {
            var command = MQTTOutCommands.FirstOrDefault(n => n.Name == commandName);

            if (command == null)
                return;

            if (string.IsNullOrEmpty(command.MQTTRequestTopic) || string.IsNullOrEmpty(command.MQTTRequestText))
            {
                Console.WriteLine("MQTT topic or data is empty");

                return;
            }

            var subscribed = SubscribeMQTT(command.MQTTRequestTopic).Result;
            if (subscribed && SendToMQTT(command.MQTTRequestTopic, command.MQTTRequestText).Result)
                AudioOut.Speak(command.PluginResponse);
            else
                AudioOut.Speak(_failedConnectionResponse);

            // example of listening to the audio/word stream from core module
            /*
            var sndData = GetSound();
            AudioOut.PlayDataBuffer(sndData);

            var wordsData = "";            
            foreach (var w in GetWords())
            {
                wordsData += " " + w;
            }
            Console.WriteLine($"ReceivedText: {wordsData}");
            */

            // example of injecting command audio/text to execute by main module
            /*InjectTextCommand("Вася привет");
            InjectAudioCommand(new byte[1024], 44100, 16, 1);*/
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

            //await SubscribeMQTT(MQTTInTopics.Select(n => n.MQTTSubscribeTopic));

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

        private async Task<bool> SendToMQTT(string topic, string data)
        {
            if (!_mqttClient.IsConnected)
            {
                Console.WriteLine("...not connected. Reconnecting...");
                var result = await ConnectMQTT(true);
                if (!result)
                {
                    Console.WriteLine("MQTT not connected");

                    return false;
                }
            }

            var message = new MqttApplicationMessageBuilder()
                .WithTopic(topic)
                .WithPayload(data)
                .Build();

            try
            {
                await _mqttClient.PublishAsync(message).ConfigureAwait(true);
            }
            catch (Exception ex)
            {
                Console.WriteLine("Write error: " + ex);

                return false;
            }

            return true;
        }

        public void Dispose()
        {
            _mqttClient?.Dispose();
            reconnectTimer?.Dispose();
        }
    }
}
