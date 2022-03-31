using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Xml.Linq;

using PluginInterface;

namespace CurrencyRatePlugin
{
    public class CurrencyRatePlugin : PluginBase
    {
        private readonly CurrencyRatePluginCommand[] CurrencyRateCommands;
        private readonly string CurrencyServiceUrl;
        private readonly string CurrencyDecimalSeparatorWord;

        public CurrencyRatePlugin(IAudioOutSingleton audioOut, string currentCulture, string pluginPath) : base(audioOut, currentCulture, pluginPath)
        {
            var configBuilder = new Config<CurrencyRatePluginSettings>($"{PluginPath}\\{PluginConfigFile}");
            if (!File.Exists($"{PluginPath}\\{PluginConfigFile}"))
            {
                configBuilder.SaveConfig();
            }

            CurrencyRateCommands = configBuilder.ConfigStorage.Commands;

            if (CurrencyRateCommands is PluginCommand[] newCmds)
            {
                _commands = newCmds;
            }
            CurrencyServiceUrl = configBuilder.ConfigStorage.CurrencyServiceUrl;
            CurrencyDecimalSeparatorWord = configBuilder.ConfigStorage.CurrencyDecimalSeparatorWord;
        }

        public override void Execute(string commandName, List<Token> commandTokens)
        {
            var command = CurrencyRateCommands.FirstOrDefault(n => n.Name == commandName);

            if (command == null)
            {
                return;
            }

            var currencyRate = GetCurrencyRate(CurrencyServiceUrl, command.CurencyCode, command.RateDecimalRound).Result;
            if (currencyRate.Item1 > 0)
            {
                var message = string.Format(command.Response, "", currencyRate.Item1, currencyRate.Item2
                    .ToString()
                    .Replace(System.Globalization.NumberFormatInfo.CurrentInfo.NumberDecimalSeparator,
                    $" {CurrencyDecimalSeparatorWord} "));
                AudioOut.Speak(message);
            }
        }

        private async Task<(int, float)> GetCurrencyRate(string currencyServiceUrl, string curencyCode, int decimalRound)
        {
            var currencyRates = await GetRate(currencyServiceUrl);
            var currencyRate = currencyRates.FirstOrDefault(n => n.CurrencyCode == curencyCode);

            if (!int.TryParse(currencyRate.Nominal, out var nominal))
                return (-1, -1);
            var decimalSeparator = System.Globalization.NumberFormatInfo.CurrentInfo.NumberDecimalSeparator;
            var v = currencyRate.Value.Replace(".", decimalSeparator);
            v = v.Replace(",", decimalSeparator);
            v = v.Substring(0, v.IndexOf(decimalSeparator) + decimalRound + 1);
            if (!float.TryParse(v, out var val))
                return (-1, -1);

            return (nominal, val);
        }

        private async Task<CurrencyRate[]> GetRate(string currencyServiceUrl)
        {
            try
            {
                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(currencyServiceUrl);
                request.AutomaticDecompression = DecompressionMethods.GZip | DecompressionMethods.Deflate;

                using (var response = await request.GetResponseAsync())
                {
                    using (Stream stream = response.GetResponseStream())
                    {
                        using (StreamReader reader = new StreamReader(stream))
                        {
                            var xmlDoc = await reader.ReadToEndAsync();
                            XDocument xdoc = XDocument.Parse(xmlDoc);

                            // получаем корневой узел
                            XElement? rates = xdoc.Element("ValCurs");
                            var currencyRates = new List<CurrencyRate>();

                            if (rates != null)
                            {
                                // проходим по всем элементам person
                                foreach (XElement rate in rates.Elements("Valute"))
                                {
                                    var code = rate.Element("CharCode");
                                    var name = rate.Element("Name");
                                    var nom = rate.Element("Nominal");
                                    var val = rate.Element("Value");

                                    var newRate = new CurrencyRate()
                                    {
                                        CurrencyCode = code?.Value,
                                        CurrencyName = name?.Value,
                                        Nominal = nom?.Value,
                                        Value = val?.Value
                                    };

                                    currencyRates.Add(newRate);
                                }
                            }

                            return currencyRates.ToArray();
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error reading from {currencyServiceUrl}: {ex.Message}");

                return null;
            }
        }
    }

    public class CurrencyRate
    {
        public string CurrencyCode;
        public string CurrencyName;
        public string Nominal;
        public string Value;
    }
}
