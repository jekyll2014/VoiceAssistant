using FuzzySharp;

using System;
using System.Collections.Generic;
using System.Text;

namespace PluginInterface
{
    public static class TextToNumberRus
    {
        private static readonly Dictionary<string, bool> Signs = new Dictionary<string, bool>
        {
            {"минус", false},
            {"плюс", true}
        };

        private static readonly Dictionary<string, int> Numbers = new Dictionary<string, int>
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
            {"девятьсот", 900}
        };

        private static readonly Dictionary<string, int> Multipliers = new Dictionary<string, int>
        {
            {"тысяча", 1000},
            {"тысячи", 1000},
            {"тысяч", 1000},
            {"миллион", 1000000},
            {"миллиона", 1000000},
            {"миллионов", 1000000},
            {"миллиард", 1000000000},
            {"миллиарда", 1000000000},
            {"миллиардов", 1000000000}
        };

        public static int GetNumber(string text, int ratio = 100)
        {
            var result = 0;
            var positive = true;
            var i = 0;
            var tokens = text.ToLower().Split(' ');

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

        private static bool TryGetValueFuzz<TK>(Dictionary<string, TK> dict, string sample, int ratio, out TK value)
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

    public static class NumberToTextRus
    {
        //Наименования сотен
        private static readonly string[] Hundreds =
        {
            "", "сто ", "двести ", "триста ", "четыреста ",
            "пятьсот ", "шестьсот ", "семьсот ", "восемьсот ", "девятьсот "
        };

        //Наименования десятков
        private static readonly string[] Tens =
        {
            "", "десять ", "двадцать ", "тридцать ", "сорок ", "пятьдесят ",
            "шестьдесят ", "семьдесят ", "восемьдесят ", "девяносто "
        };

        /// <summary>
        ///     Перевод в строку числа с учётом падежного окончания относящегося к числу существительного
        /// </summary>
        /// <param name="val">Число</param>
        /// <param name="male">Род существительного, которое относится к числу</param>
        /// <param name="one">Форма существительного в единственном числе</param>
        /// <param name="two">Форма существительного от двух до четырёх</param>
        /// <param name="five">Форма существительного от пяти и больше</param>
        /// <returns></returns>
        public static string Str(int val, bool male, string one, string two, string five)
        {
            string[] frac20 =
            {
                "", "один ", "два ", "три ", "четыре ", "пять ", "шесть ",
                "семь ", "восемь ", "девять ", "десять ", "одиннадцать ",
                "двенадцать ", "тринадцать ", "четырнадцать ", "пятнадцать ",
                "шестнадцать ", "семнадцать ", "восемнадцать ", "девятнадцать "
            };

            var num = val % 1000;
            if (0 == num) return "";
            if (num < 0) throw new ArgumentOutOfRangeException("val", "Параметр не может быть отрицательным");
            if (!male)
            {
                frac20[1] = "одна ";
                frac20[2] = "две ";
            }

            var r = new StringBuilder(Hundreds[num / 100]);

            if (num % 100 < 20)
            {
                r.Append(frac20[num % 100]);
            }
            else
            {
                r.Append(Tens[num % 100 / 10]);
                r.Append(frac20[num % 10]);
            }

            r.Append(Case(num, one, two, five));

            if (r.Length != 0) r.Append(" ");
            return r.ToString();
        }

        /// <summary>
        ///     Выбор правильного падежного окончания сущесвительного
        /// </summary>
        /// <param name="val">Число</param>
        /// <param name="one">Форма существительного в единственном числе</param>
        /// <param name="two">Форма существительного от двух до четырёх</param>
        /// <param name="five">Форма существительного от пяти и больше</param>
        /// <returns>Возвращает существительное с падежным окончанием, которое соответсвует числу</returns>
        public static string Case(int val, string one, string two, string five)
        {
            var t = val % 100 > 20 ? val % 10 : val % 20;

            switch (t)
            {
                case 1: return one;
                case 2:
                case 3:
                case 4: return two;
                default: return five;
            }
        }

        /// <summary>
        ///     Перевод целого числа в строку
        /// </summary>
        /// <param name="val">Число</param>
        /// <returns>Возвращает строковую запись числа</returns>
        public static string Str(int val)
        {
            var minus = false;
            if (val < 0)
            {
                val = -val;
                minus = true;
            }

            var n = val;

            var r = new StringBuilder();

            if (0 == n) r.Append("0 ");
            if (n % 1000 != 0)
                r.Append(Str(n, true, "", "", ""));

            n /= 1000;

            r.Insert(0, Str(n, false, "тысяча", "тысячи", "тысяч"));
            n /= 1000;

            r.Insert(0, Str(n, true, "миллион", "миллиона", "миллионов"));
            n /= 1000;

            r.Insert(0, Str(n, true, "миллиард", "миллиарда", "миллиардов"));
            n /= 1000;

            r.Insert(0, Str(n, true, "триллион", "триллиона", "триллионов"));
            n /= 1000;

            r.Insert(0, Str(n, true, "триллиард", "триллиарда", "триллиардов"));
            if (minus) r.Insert(0, "минус ");

            return r.ToString();
        }
    }
}
