using System;

namespace OpenWeatherPlugin
{
    public partial class OpenWeatherPlugin
    {
        public class WeatherForecastData
        {
            // weather.description - Weather condition within the group. You can get the output in your language.
            public string WeatherDescription;

            // main.temp - Temperature
            public int Temperature;
            public int MinTemperature;
            public int MaxTemperature;

            // main.humidity - Humidity, %
            public int Humidity;
            public int MinHumidity;
            public int MaxHumidity;

            // main.pressure - Atmospheric pressure (on the sea level, if there is no sea_level or grnd_level data), hPa
            public int Pressure;
            public int MinPressure;
            public int MaxPressure;

            // wind.speed - Wind speed. Unit Default: meter/sec, Metric: meter/sec, Imperial: miles/hour
            public int WindSpeed;
            public int MinWindSpeed;
            public int MaxWindSpeed;

            // wind.deg - Wind direction, degrees (meteorological)
            public int WindDirection;
            public int MinWindDirection;
            public int MaxWindDirection;

            public string WindDirectionName;
            public string MinWindDirectionName;
            public string MaxWindDirectionName;

            // clouds.all - Cloudiness, %
            public int Clouds;
            public int MinClouds;
            public int MaxClouds;

            // dt - Time of data calculation, unix, UTC
            public DateTime RecordDateTime;
        }
    }
}
