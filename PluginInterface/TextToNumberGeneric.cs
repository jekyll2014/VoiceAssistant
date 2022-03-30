using FuzzySharp;

using System.Collections.Generic;
using System.IO;

namespace PluginInterface
{
    class TextToNumberGeneric : ITextToNumber
    {
        private const string _settingsConfigFile = "GenericNumberSettings.json";

        private readonly Dictionary<string, bool> Signs;
        private readonly Dictionary<string, long> Numbers;
        private readonly Dictionary<string, long> Multipliers;

        public TextToNumberGeneric()
        {
            var configBuilder = new Config<GenericNumberSettings>($"{_settingsConfigFile}");
            if (!File.Exists($"{_settingsConfigFile}"))
            {
                configBuilder.SaveConfig();
            }

            var configStorage = configBuilder.ConfigStorage;

            Signs = new Dictionary<string, bool>
             {
                 {configStorage.Minus, false},
                 {configStorage.Plus, true}
             };

            Numbers = new Dictionary<string, long>
            {
                {configStorage.Zero, 0},
                {configStorage.One, 1},
                {configStorage.OneFemale, 1},
                {configStorage.Two, 2},
                {configStorage.TwoFemale, 2},
                {configStorage.Three, 3},
                {configStorage.Four, 4},
                {configStorage.Five, 5},
                {configStorage.Six, 6},
                {configStorage.Seven, 7},
                {configStorage.Eight, 8},
                {configStorage.Nine, 9},
                {configStorage.Ten, 10},
                {configStorage.Elleven, 11},
                {configStorage.Twelve, 12},
                {configStorage.Thirteen, 13},
                {configStorage.Fourteen, 14},
                {configStorage.Fifteen, 15},
                {configStorage.Sixteen, 16},
                {configStorage.Seventeen, 17},
                {configStorage.Eighteen, 18},
                {configStorage.Nineteen, 19},
                {configStorage.Twenty, 20},
                {configStorage.Thirty, 30},
                {configStorage.Forty, 40},
                {configStorage.Fifty, 50},
                {configStorage.Sixty, 60},
                {configStorage.Seventy, 70},
                {configStorage.Eighty, 80},
                {configStorage.Ninety, 90},
                {configStorage.OneHundred, 100},
                {configStorage.TwoHundred, 200},
                {configStorage.ThreeHundred, 300},
                {configStorage.FourHundred, 400},
                {configStorage.FiveHundred, 500},
                {configStorage.SixHundred, 600},
                {configStorage.SevenHundred, 700},
                {configStorage.EightHundred, 800},
                {configStorage.NineHundred, 900},
                {configStorage.OneThousand, 1000},
                {configStorage.OneMillion, 1000000},
                {configStorage.OneBillion, 1000000000},
                {configStorage.OneTrillion, 1000000000000},
                {configStorage.OneQuadrillion, 1000000000000},
            };

            Multipliers = new Dictionary<string, long>
            {
                {configStorage.OneThousand, 1000},
                {configStorage.TwoThousand, 1000},
                {configStorage.FiveThousand, 1000},

                {configStorage.OneMillion, 1000000},
                {configStorage.TwoMillion, 1000000},
                {configStorage.FiveMillion, 1000000},

                {configStorage.OneBillion, 1000000000},
                {configStorage.TwoBillion, 1000000000},
                {configStorage.FiveBillion, 1000000000},

                {configStorage.OneTrillion, 1000000000000},
                {configStorage.TwoTrillion, 1000000000000},
                {configStorage.FiveTrillion, 1000000000000},

                {configStorage.OneQuadrillion, 1000000000000000},
                {configStorage.TwoQuadrillion, 1000000000000000},
                {configStorage.FiveQuadrillion, 1000000000000000}
            };
        }

        public long ConvertStringToNumber(string numberString, int ratio = 100)
        {
            long result = 0;
            var positive = true;
            var i = 0;
            var tokens = numberString.ToLower().Split(' ');

            if (TryGetValueFuzz(Signs, tokens[0], ratio, out var p))
            {
                positive = p;
                i++;
            }

            for (; i < tokens.Length; i++)
                if (TryGetValueFuzz(Numbers, tokens[i], ratio, out var number))
                {
                    if (i + 1 < tokens.Length &&
                        TryGetValueFuzz(Multipliers, tokens[i + 1], ratio, out var multiplier))
                    {
                        number *= multiplier;
                        i++;
                    }

                    result += number;
                }
                else
                {
                    i = tokens.Length;
                }

            if (!positive)
                result *= -1;

            return result;
        }

        private bool TryGetValueFuzz<T>(Dictionary<string, T> dict, string sample, int ratio, out T value)
        {
            value = default;

            foreach (var (key1, value1) in dict)
            {
                if (Fuzz.WeightedRatio(key1, sample) > ratio)
                {
                    value = value1;
                    return true;
                }
            }

            return false;
        }
    }
}