using PluginInterface;

using System.Collections.Generic;

namespace OpenWeatherPlugin
{
    public class OpenWeatherPluginSettings
    {
        public string[] ConfigurationNote =
            {
            "Interpolation macros:",
            "Only for \"Now\" commands:",
            "{WeatherDescription} - weather description",
            "{Temperature} - temperature",
            "{Humidity} - humidity, %",
            "{Pressure} - pressure, hPa",
            "{WindSpeed} - wind speed",
            "{WindDirection} - wind direction, degree",
            "{WindDirectionName} - wind direction name",
            "{Clouds} - cloudiness, %",
            "",
            "Only for \"Today\" and \"Tomorrow\" commands:",
            "{MorningPhrase} - use MorningPhrase sample",
            "{DayPhrase} - use DayPhrase sample",
            "{EveningPhrase} - use EveningPhrase sample",
            "{NightPhrase} - use NightPhrase sample",
            "",
            "Only for \"Morning/Day/Evening/Night\" phrases:",
            "{WeatherDescription} - weather description",
            "{Temperature} - average temperature",
            "{MinTemperature} - min. temperature",
            "{MaxTemperature} - max. temperature",
            "{Humidity} - average humidity, %",
            "{MinHumidity} - min. humidity, %",
            "{MaxHumidity} - max. humidity, %",
            "{Pressure} - average pressure, hPa",
            "{MinPressure} - min. pressure, hPa",
            "{MaxPressure} - max. pressure, hPa",
            "{WindSpeed} - average wind speed",
            "{MinWindSpeed} - min. wind speed",
            "{MaxWindSpeed} - max. wind speed",
            "{WindDirection} - average wind direction, degree",
            "{MinWindDirection} - min. wind direction, degree",
            "{MaxWindDirection} - max. wind direction, degree",
            "{WindDirectionName} - average wind direction name",
            "{MinWindDirectionName} - min. wind direction name",
            "{MaxWindDirectionName} - max. wind direction name",
            "{Clouds} - average cloudiness, %",
            "{MinClouds} - min. cloudiness, %",
            "{MaxClouds} - max. cloudiness, %",
        };

        public string ApiKey = "";

        public List<(string Name, int MaxDegree)> WindDirections = new List<(string, int)>
        {
            ("Северный", 23),
            ("Северо-восточный", 68),
            ("Восточный", 113),
            ("Юго-восточный", 158),
            ("Южный", 203),
            ("Юго-западный", 248),
            ("Западный", 293),
            ("Северо-западный", 338),
            ("Северный", 361),
        };

        public string WeatherCurrentQuery = "https://api.openweathermap.org/data/2.5/weather?q={2}&appid={1}&units=metric&lang=ru";
        public string WeatherForecastQuery = "https://api.openweathermap.org/data/2.5/forecast?q={2}&appid={1}&units=metric&lang=ru";
        public string MorningPhrase = "Утром {WeatherDescription}, температура от {MinTemperature} до {MaxTemperature} градусов, влажность {Humidity} процентов, ветер {WindDirectionName} {WindSpeed} метров в секунду";
        public string DayPhrase = "Днем {WeatherDescription}, температура от {MinTemperature} до {MaxTemperature} градусов, влажность {Humidity} процентов, ветер {WindDirectionName} {WindSpeed} метров в секунду";
        public string EveningPhrase = "Вечером {WeatherDescription}, температура от {MinTemperature} до {MaxTemperature} градусов, влажность {Humidity} процентов, ветер {WindDirectionName} {WindSpeed} метров в секунду";
        public string NightPhrase = "Ночью {WeatherDescription}, температура от {MinTemperature} до {MaxTemperature} градусов, влажность {Humidity} процентов, ветер {WindDirectionName} {WindSpeed} метров в секунду";

        public int MorningStartHour = 7;
        public int DayStartHour = 10;
        public int EveningStartHour = 18;
        public int NightStartHour = 22;

        //[JsonProperty(Required = Required.Always)]
        public OpenWeatherPluginCommand[] Commands =
        {
            new OpenWeatherPluginCommand
            {
                Name = "Short weather now",
                Tokens = new[]
                {
                    new Token
                    {
                        SuccessRate = 90,
                        Type = TokenType.Command,
                        Value = new[] {"какая"}
                    },
                    new Token
                    {
                        SuccessRate = 90,
                        Type = TokenType.Command,
                        Value = new[] {"сейчас"}
                    },
                    new Token
                    {
                        SuccessRate = 90,
                        Type = TokenType.Command,
                        Value = new[] {"погода"}
                    }
                },
                DayTime = "Now", // Now, Today, Tomorrow
                CityId = "Tashkent,uz",
                Response = "В Ташкенте сейчас {WeatherDescription}, температура {Temperature} градусов, влажность {Humidity} процентов, ветер {WindDirectionName}, {WindSpeed} метров в секунду"
            },
            new OpenWeatherPluginCommand
            {
                Name = "Short weather today",
                Tokens = new[]
                {
                    new Token
                    {
                        SuccessRate = 90,
                        Type = TokenType.Command,
                        Value = new[] {"какая"}
                    },
                    new Token
                    {
                        SuccessRate = 90,
                        Type = TokenType.Command,
                        Value = new[] {"сегодня"}
                    },
                    new Token
                    {
                        SuccessRate = 90,
                        Type = TokenType.Command,
                        Value = new[] {"погода"}
                    }
                },
                DayTime = "Today",
                CityId = "Tashkent,uz",
                Response = "В Ташкенте сегодня {MorningPhrase}, {DayPhrase}, {EveningPhrase}, {NightPhrase}"
            },
            new OpenWeatherPluginCommand
            {
                Name = "Short weather tomorrow",
                Tokens = new[]
                {
                    new Token
                    {
                        SuccessRate = 90,
                        Type = TokenType.Command,
                        Value = new[] {"какая"}
                    },
                    new Token
                    {
                        SuccessRate = 90,
                        Type = TokenType.Command,
                        Value = new[] {"завтра"}
                    },
                    new Token
                    {
                        SuccessRate = 90,
                        Type = TokenType.Command,
                        Value = new[] {"погода"}
                    }
                },
                DayTime = "Tomorrow", // Now, Today, Tomorrow
                CityId = "Tashkent,uz",
                Response = "В Ташкенте завтра {MorningPhrase}, {DayPhrase}, {EveningPhrase}, {NightPhrase}"
            },
            new OpenWeatherPluginCommand
            {
                Name = "Moscow weather now",
                Tokens = new[]
                {
                    new Token
                    {
                        SuccessRate = 90,
                        Type = TokenType.Command,
                        Value = new[] {"какая"}
                    },
                    new Token
                    {
                        SuccessRate = 90,
                        Type = TokenType.Command,
                        Value = new[] {"сейчас"}
                    },
                    new Token
                    {
                        SuccessRate = 90,
                        Type = TokenType.Command,
                        Value = new[] {"погода"}
                    },
                    new Token
                    {
                        SuccessRate = 90,
                        Type = TokenType.Command,
                        Value = new[] {"в"}
                    },
                    new Token
                    {
                        SuccessRate = 90,
                        Type = TokenType.Command,
                        Value = new[] {"Москве"}
                    }
                },
                DayTime = "Now", // Now, Today, Tomorrow
                CityId = "Moscow,ru",
                Response = "В Москве сейчас {WeatherDescription}, температура {Temperature} градусов, влажность {Humidity} процентов, ветер {WindDirectionName}, {WindSpeed} метров в секунду"
            },
            new OpenWeatherPluginCommand
            {
                Name = "Moscow weather today",
                Tokens = new[]
                {
                    new Token
                    {
                        SuccessRate = 90,
                        Type = TokenType.Command,
                        Value = new[] {"какая"}
                    },
                    new Token
                    {
                        SuccessRate = 90,
                        Type = TokenType.Command,
                        Value = new[] {"сегодня"}
                    },
                    new Token
                    {
                        SuccessRate = 90,
                        Type = TokenType.Command,
                        Value = new[] {"погода"}
                    },
                    new Token
                    {
                        SuccessRate = 90,
                        Type = TokenType.Command,
                        Value = new[] {"в"}
                    },
                    new Token
                    {
                        SuccessRate = 90,
                        Type = TokenType.Command,
                        Value = new[] {"Москве"}
                    }
                },
                DayTime = "Today", // Now, Today, Tomorrow
                CityId = "Moscow,ru",
                Response = "В Москве сегодня {MorningPhrase}, {DayPhrase}, {EveningPhrase}, {NightPhrase}"
            },
            new OpenWeatherPluginCommand
            {
                Name = "Moscow weather tomorrow",
                Tokens = new[]
                {
                    new Token
                    {
                        SuccessRate = 90,
                        Type = TokenType.Command,
                        Value = new[] {"какая"}
                    },
                    new Token
                    {
                        SuccessRate = 90,
                        Type = TokenType.Command,
                        Value = new[] {"завтра"}
                    },
                    new Token
                    {
                        SuccessRate = 90,
                        Type = TokenType.Command,
                        Value = new[] {"погода"}
                    },
                    new Token
                    {
                        SuccessRate = 90,
                        Type = TokenType.Command,
                        Value = new[] {"в"}
                    },
                    new Token
                    {
                        SuccessRate = 90,
                        Type = TokenType.Command,
                        Value = new[] {"Москве"}
                    }
                },
                DayTime = "Tomorrow", // Now, Today, Tomorrow
                CityId = "Moscow,ru",
                Response = "В Москве завтра {MorningPhrase}, {DayPhrase}, {EveningPhrase}, {NightPhrase}"
            },
        };
    }
}
