// This is an independent project of an individual developer. Dear PVS-Studio, please check it.
// PVS-Studio Static Code Analyzer for C, C++, C#, and Java: http://www.viva64.com
using System;

namespace OpenWeatherPlugin
{
    public partial class OpenWeatherPlugin
    {
        public class CurrentWeatherData
        {
            // weather.description - Weather condition within the group. You can get the output in your language.
            public string WeatherDescription;

            // main.temp - Temperature
            public int Temperature;

            // main.humidity - Humidity, %
            public int Humidity;

            // main.pressure - Atmospheric pressure (on the sea level, if there is no sea_level or grnd_level data), hPa
            public int Pressure;

            // wind.speed - Wind speed. Unit Default: meter/sec, Metric: meter/sec, Imperial: miles/hour
            public int WindSpeed;

            // wind.deg - Wind direction, degrees (meteorological)
            public int WindDirection;

            public string WindDirectionName;

            // clouds.all - Cloudiness, %
            public int Clouds;

            // dt - Time of data calculation, unix, UTC
            public DateTime RecordDateTime;
        }
    }
}
