using FuzzySharp;

using System.Collections.Generic;

namespace PluginInterface
{
    public class TextToNumberRus : ITextToNumber
    {
        private readonly Dictionary<string, bool> Signs = new Dictionary<string, bool>
        {
            {"минус", false},
            {"плюс", true}
        };

        private readonly Dictionary<string, long> Numbers = new Dictionary<string, long>
        {
            {"ноль", 0},
            {"один", 1},
            {"одна", 1},
            {"одну", 1},
            {"два", 2},
            {"две", 2},
            {"три", 3},
            {"четыре", 4},
            {"пять", 5},
            {"шесть", 6},
            {"семь", 7},
            {"восемь", 8},
            {"девять", 9},
            {"десять", 10},
            {"одиннадцать", 11},
            {"двенадцать", 12},
            {"тринадцать", 13},
            {"четырнадцать", 14},
            {"пятнадцать", 15},
            {"шестнадцать", 16},
            {"семнадцать", 17},
            {"восемнадцать", 18},
            {"девятнадцать", 19},
            {"двадцать", 20},
            {"тридцать", 30},
            {"сорок", 40},
            {"пятьдесят", 50},
            {"шестьдесят", 60},
            {"семьдесят", 70},
            {"восемьдесят", 80},
            {"девяносто", 90},
            {"сто", 100},
            {"двести", 200},
            {"триста", 300},
            {"четыреста", 400},
            {"пятьсот", 500},
            {"шестьсот", 600},
            {"семьсот", 700},
            {"восемьсот", 800},
            {"девятьсот", 900},
            {"тысяча", 1000},
            {"миллион", 1000000},
            {"миллиард", 1000000000},
            {"триллион", 1000000000000},
            {"квадриллион", 1000000000000},
        };

        private readonly Dictionary<string, long> Multipliers = new Dictionary<string, long>
        {
            {"тысяча", 1000},
            {"тысячи", 1000},
            {"тысяч", 1000},

            {"миллион", 1000000},
            {"миллиона", 1000000},
            {"миллионов", 1000000},

            {"миллиард", 1000000000},
            {"миллиарда", 1000000000},
            {"миллиардов", 1000000000},

            {"триллион", 1000000000000},
            {"триллиона", 1000000000000},
            {"триллионов", 1000000000000},

            {"квадриллион", 1000000000000000},
            {"квадриллиона", 1000000000000000},
            {"квадриллионов", 1000000000000000}
        };

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

        private bool TryGetValueFuzz<TK>(Dictionary<string, TK> dict, string sample, int ratio, out TK value)
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
