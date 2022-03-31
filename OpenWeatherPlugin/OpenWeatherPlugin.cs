using Newtonsoft.Json;

using PluginInterface;

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text;

namespace OpenWeatherPlugin
{
    public partial class OpenWeatherPlugin : PluginBase
    {
        private readonly string ApiKey = "";

        private readonly OpenWeatherPluginCommand[] CurrencyRateCommands;
        public List<(string Name, int MaxDegree)> WindDirections;

        private readonly string OpenWeatherNowUrl;
        private readonly string OpenWeatherForecastUrl;

        private readonly string _morningPhrase;
        private readonly string _dayPhrase;
        private readonly string _eveningPhrase;
        private readonly string _nightPhrase;

        private readonly int MorningStartHour;
        private readonly int DayStartHour;
        private readonly int EveningStartHour;
        private readonly int NightStartHour;

        private Dictionary<string, CurrentWeatherData> _currentWeatherCache = new Dictionary<string, CurrentWeatherData>();

        private Dictionary<string, WeatherForecastData[]> _weatherForecastData = new Dictionary<string, WeatherForecastData[]>();

        public OpenWeatherPlugin(IAudioOutSingleton audioOut, string currentCulture, string pluginPath) : base(audioOut, currentCulture, pluginPath)
        {
            var configBuilder = new Config<OpenWeatherPluginSettings>($"{PluginPath}\\{PluginConfigFile}");
            if (!File.Exists($"{PluginPath}\\{PluginConfigFile}"))
            {
                configBuilder.SaveConfig();
            }

            CurrencyRateCommands = configBuilder.ConfigStorage.Commands;

            if (CurrencyRateCommands is PluginCommand[] newCmds)
            {
                _commands = newCmds;
            }

            OpenWeatherNowUrl = configBuilder.ConfigStorage.WeatherCurrentQuery;
            OpenWeatherForecastUrl = configBuilder.ConfigStorage.WeatherForecastQuery;
            _morningPhrase = configBuilder.ConfigStorage.MorningPhrase;
            _dayPhrase = configBuilder.ConfigStorage.DayPhrase;
            _eveningPhrase = configBuilder.ConfigStorage.EveningPhrase;
            _nightPhrase = configBuilder.ConfigStorage.NightPhrase;
            MorningStartHour = configBuilder.ConfigStorage.MorningStartHour;
            DayStartHour = configBuilder.ConfigStorage.DayStartHour;
            EveningStartHour = configBuilder.ConfigStorage.EveningStartHour;
            NightStartHour = configBuilder.ConfigStorage.NightStartHour;
            WindDirections = configBuilder.ConfigStorage.WindDirections;
            ApiKey = configBuilder.ConfigStorage.ApiKey;
        }

        public override void Execute(string commandName, List<Token> commandTokens)
        {
            var command = CurrencyRateCommands.FirstOrDefault(n => n.Name == commandName);

            if (command == null)
            {
                return;
            }

            var message = string.Empty;

            if (command.DayTime == "Now")
            {
                var currentWeather = GetCurrentWeather(ApiKey, command.CityId);
                var windDir = GetWindDirectionName(currentWeather.WindDirection);
                //message = string.Format(command.Response, "", "", "", "", "", currentWeather.WeatherDescription, currentWeather.Temperature, currentWeather.Humidity, currentWeather.WindSpeed, windDir);
                message = FormatStringWithClassFields(command.Response, currentWeather);
            }
            else
            {
                var weatherForecast = GetWeatherForecast(ApiKey, command.CityId);
                var dtMessages = new DayTimeMessages();

                if (command.DayTime == "Today" && weatherForecast.Count() >= 4)
                {
                    dtMessages.MorningPhrase = weatherForecast[0] == null ? "" : FormatStringWithClassFields(_morningPhrase, weatherForecast[0]);
                    dtMessages.DayPhrase = weatherForecast[1] == null ? "" : FormatStringWithClassFields(_dayPhrase, weatherForecast[1]);
                    dtMessages.EveningPhrase = weatherForecast[2] == null ? "" : FormatStringWithClassFields(_eveningPhrase, weatherForecast[2]);
                    dtMessages.NightPhrase = weatherForecast[3] == null ? "" : FormatStringWithClassFields(_nightPhrase, weatherForecast[3]);
                }
                else if (command.DayTime == "Tomorrow" && weatherForecast.Count() >= 8)
                {
                    dtMessages.MorningPhrase = weatherForecast[4] == null ? "" : FormatStringWithClassFields(_morningPhrase, weatherForecast[4]);
                    dtMessages.DayPhrase = weatherForecast[5] == null ? "" : FormatStringWithClassFields(_dayPhrase, weatherForecast[5]);
                    dtMessages.EveningPhrase = weatherForecast[6] == null ? "" : FormatStringWithClassFields(_eveningPhrase, weatherForecast[6]);
                    dtMessages.NightPhrase = weatherForecast[7] == null ? "" : FormatStringWithClassFields(_nightPhrase, weatherForecast[7]);
                }

                message = FormatStringWithClassFields(command.Response, dtMessages);
            }

            AudioOut.Speak(message);
        }

        public class DayTimeMessages
        {
            public string MorningPhrase;
            public string DayPhrase;
            public string EveningPhrase;
            public string NightPhrase;
        }

        private CurrentWeatherData GetCurrentWeather(string apiKey, string cityId)
        {
            var serviceUrl = string.Format(OpenWeatherNowUrl, "", apiKey, cityId);

            // проверить кэш
            if (_currentWeatherCache.TryGetValue(serviceUrl, out var cachedWeather))
            {
                if (cachedWeather.RecordDateTime < DateTime.Now.AddHours(1))
                {
                    return cachedWeather;
                }
                else
                {
                    _currentWeatherCache.Remove(serviceUrl);
                }
            }

            // получить json
            var result = new CurrentWeatherData();
            var jsonText = GetWeatherData(serviceUrl);

            if (string.IsNullOrEmpty(jsonText))
            {
                return result;
            }

            // распарсить
            var data = ParseJson<WeatherCurrent>(jsonText);

            // переложить из пришедших данных в свою структуру
            result = GetSimpleData(data);

            // добавить в кэш
            _currentWeatherCache.Add(serviceUrl, result);

            return result;
        }

        private CurrentWeatherData GetSimpleData(WeatherCurrent data)
        {
            var result = new CurrentWeatherData();

            if (data != null)
            {
                result.Clouds = data.clouds?.all ?? -1;
                result.Humidity = data.main?.humidity ?? -1;
                result.Pressure = data.main?.pressure ?? -1;
                result.RecordDateTime = UnixTimeStampToDateTime(data.dt);
                result.Temperature = (int)data.main?.temp;
                result.WeatherDescription = data.weather?.FirstOrDefault().description;
                result.WindDirection = data.wind?.deg ?? -1;
                result.WindDirectionName = result.WindDirection == -1 ? "" : GetWindDirectionName(result.WindDirection);
                result.WindSpeed = (int)data.wind?.speed;
            }

            return result;
        }

        public static DateTime UnixTimeStampToDateTime(double unixTimeStamp)
        {
            // Unix timestamp is seconds past epoch
            DateTime dateTime = new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc);
            dateTime = dateTime.AddSeconds(unixTimeStamp).ToLocalTime();
            return dateTime;
        }

        private WeatherForecastData[] GetWeatherForecast(string apiKey, string cityId)
        {
            var serviceUrl = string.Format(OpenWeatherForecastUrl, "", apiKey, cityId);

            // проверить кэш
            if (_weatherForecastData.TryGetValue(serviceUrl, out var cachedWeather))
            {
                if (cachedWeather.FirstOrDefault(n => n != null)?.RecordDateTime < DateTime.Now.AddHours(1))
                {
                    return cachedWeather;
                }
                else
                {
                    _weatherForecastData.Remove(serviceUrl);
                }
            }

            // получить json
            var result = new WeatherForecastData[] { };
            var jsonText = GetWeatherData(serviceUrl);

            if (string.IsNullOrEmpty(jsonText))
            {
                return result;
            }

            // распарсить
            var data = ParseJson<WeatherForecast>(jsonText);

            // переложить из пришедших данных в свою структуру
            if (data != null)
            {
                var timeNow = DateTimeToUnixTimestamp(DateTime.UtcNow);

                var timeFrames = new long[]
                    {
                        //timeMorningStarts
                        DateTimeToUnixTimestamp(DateTime.Now.AddHours(MorningStartHour)),
                        //timeDayStarts
                        DateTimeToUnixTimestamp(DateTime.Today.AddHours(DayStartHour)),
                        //timeEveningStarts
                        DateTimeToUnixTimestamp(DateTime.Today.AddHours(EveningStartHour)), 
                        //timeNightStarts
                        DateTimeToUnixTimestamp(DateTime.Today.AddHours(NightStartHour)), 
                        //timeNextMorningStarts
                        DateTimeToUnixTimestamp(DateTime.Today.AddDays(1).AddHours(MorningStartHour)), 
                        //timeNextDayStarts
                        DateTimeToUnixTimestamp(DateTime.Today.AddDays(1).AddHours(DayStartHour)), 
                        //timeNextEveningStarts
                        DateTimeToUnixTimestamp(DateTime.Today.AddDays(1).AddHours(EveningStartHour)), 
                        //timeNextNightStarts
                        DateTimeToUnixTimestamp(DateTime.Today.AddDays(1).AddHours(NightStartHour)), 
                        //timeNextNightEnds
                        DateTimeToUnixTimestamp(DateTime.Today.AddDays(2).AddHours(MorningStartHour)),
                };

                var dayTimeRecords = new List<WeatherForecastData>();
                for (var i = 0; i < timeFrames.Length - 1; i++)
                {
                    var fileredRecords = data.list.Where(n => n.dt >= timeNow && n.dt >= timeFrames[i] && n.dt < timeFrames[i + 1]).ToArray();
                    var r = GetAverageData(fileredRecords);
                    dayTimeRecords.Add(r);
                }

                result = dayTimeRecords.ToArray();
            }

            // добавить в кэш
            _weatherForecastData.Add(serviceUrl, result);


            return result;
        }

        public static long DateTimeToUnixTimestamp(DateTime dateTime)
        {
            return (long)((TimeZoneInfo.ConvertTimeToUtc(dateTime) -
                   new DateTime(1970, 1, 1, 0, 0, 0, 0, System.DateTimeKind.Utc)).TotalSeconds);
        }

        private WeatherForecastData GetAverageData(WeatherForecast.List[] data)
        {
            if (data == null || !data.Any())
                return null;

            WeatherForecastData result = new WeatherForecastData();

            result.RecordDateTime = UnixTimeStampToDateTime(data.FirstOrDefault().dt);

            result.Clouds = (int)(data.Average(n => n.clouds?.all ?? -1));
            result.MinClouds = data.Min(n => n.clouds?.all ?? -1);
            result.MaxClouds = data.Max(n => n.clouds?.all ?? -1);

            result.Humidity = (int)data.Average(n => n.main?.humidity ?? -1);
            result.MinHumidity = data.Min(n => n.main?.humidity ?? -1);
            result.MaxHumidity = data.Max(n => n.main?.humidity ?? -1);

            result.Pressure = (int)data.Average(n => n.main?.pressure ?? -1);
            result.MinPressure = data.Min(n => n.main?.pressure ?? -1);
            result.MaxPressure = data.Max(n => n.main?.pressure ?? -1);

            result.Temperature = (int)data.Average(n => n.main?.temp ?? -100);
            result.MinTemperature = (int)data.Min(n => n.main?.temp ?? -100);
            result.MaxTemperature = (int)data.Max(n => n.main?.temp ?? -100);

            result.WeatherDescription = data.FirstOrDefault()?.weather.FirstOrDefault()?.description;

            result.WindDirection = (int)data.Average(n => n.wind?.deg ?? -1);
            result.MinWindDirection = data.Min(n => n.wind?.deg ?? -1);
            result.MaxWindDirection = data.Max(n => n.wind?.deg ?? -1);

            result.WindDirectionName = result.WindDirection == -1 ? "" : GetWindDirectionName(result.WindDirection);
            result.MinWindDirectionName = result.MinWindDirection == -1 ? "" : GetWindDirectionName(result.MinWindDirection);
            result.MaxWindDirectionName = result.MaxWindDirection == -1 ? "" : GetWindDirectionName(result.MaxWindDirection);

            result.WindSpeed = (int)data.Average(n => n.wind?.speed ?? -1);
            result.MinWindSpeed = (int)data.Min(n => n.wind?.speed ?? -1);
            result.MaxWindSpeed = (int)data.Max(n => n.wind?.speed ?? -1);

            return result;
        }

        public string GetWeatherData(string serviceUrl)
        {
            string json = string.Empty;

            try
            {
                using (var httpClient = new HttpClient())
                {
                    json = httpClient.GetStringAsync(serviceUrl).Result;
                }
            }
            catch (Exception e)
            {
            }

            return json;
        }

        private string GetWindDirectionName(int degree)
        {
            foreach (var wind in WindDirections)
            {
                if (degree < wind.MaxDegree)
                    return wind.Name;
            }

            return string.Empty;
        }

        public static T ParseJson<T>(string data)
        {
            T newValues = default;

            if (string.IsNullOrEmpty(data))
                return newValues;

            using var jsonStream = new StringReader(data);
            var serializer = new JsonSerializer();
            newValues = (T)serializer.Deserialize(jsonStream, typeof(T));

            return newValues;
        }

        private string FormatStringWithClassFields(string sample, object sourceClass)
        {
            var result = new StringBuilder();
            var openBracketPosition = sample.IndexOf('{');

            if (openBracketPosition <= 0)
            {
                return sample;
            }

            if (openBracketPosition > 0)
            {
                result.Append(sample.Substring(0, openBracketPosition));
            }

            while (openBracketPosition >= 0)
            {
                var closeBracketPosition = sample.IndexOf('}', openBracketPosition + 1);
                if (closeBracketPosition < 0)
                {
                    result.Append(sample.Substring(openBracketPosition));

                    return result.ToString();
                }

                var propertyName = sample.Substring(openBracketPosition + 1, closeBracketPosition - openBracketPosition - 1);

                object propertyValue = null;
                try
                {
                    propertyValue = sourceClass.GetType()?.GetField(propertyName)?.GetValue(sourceClass);

                    if (propertyValue == null)
                    {
                        propertyValue = sourceClass.GetType()?.GetProperty(propertyName)?.GetValue(sourceClass);
                    }
                }
                catch
                {

                }

                if (propertyValue != null)
                {
                    result.Append(propertyValue.ToString());
                }
                else
                {
                    result.Append("{" + propertyName + "}");
                }

                openBracketPosition = sample.IndexOf('{', closeBracketPosition + 1);

                if (openBracketPosition > 0)
                {
                    result.Append(sample.Substring(closeBracketPosition + 1, openBracketPosition - closeBracketPosition - 1));
                }
                else
                {
                    result.Append(sample.Substring(closeBracketPosition + 1));
                }
            }

            return result.ToString();
        }
    }
}
