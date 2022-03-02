using FuzzySharp;

using System;
using System.Collections.Generic;
using System.Text;

namespace PluginInterface
{
    public static class TextToNumberRus
    {
        private static Dictionary<string, bool> _signs = new Dictionary<string, bool>()
        {
            { "минус", false},
            { "плюс", true}
        };

        private static Dictionary<string, int> _numbers = new Dictionary<string, int>
        {
            { "ноль", 0 },
            { "один", 1 },
            { "одна", 1 },
            { "одну", 1 },
            { "два", 2 },
            { "две", 2 },
            { "три", 3 },
            { "четыре", 4 },
            { "пять", 5 },
            { "шесть", 6 },
            { "семь", 7 },
            { "восемь", 8 },
            { "девять", 9 },
            { "десять", 10 },
            { "одиннадцать", 11 },
            { "двенадцать", 12 },
            { "тринадцать", 13 },
            { "четырнадцать", 14 },
            { "пятнадцать", 15 },
            { "шестнадцать", 16 },
            { "семнадцать", 17 },
            { "восемнадцать", 18 },
            { "девятнадцать", 19 },
            { "двадцать", 20 },
            { "тридцать", 30 },
            { "сорок", 40 },
            { "пятьдесят", 50 },
            { "шестьдесят", 60 },
            { "семьдесят", 70 },
            { "восемьдесят", 80 },
            { "девяносто", 90 },
            { "сто", 100 },
            { "двести", 200 },
            { "триста", 300 },
            { "четыреста", 400 },
            { "пятьсот", 500 },
            { "шестьсот", 600 },
            { "семьсот", 700 },
            { "восемьсот", 800 },
            { "девятьсот", 900 },
        };

        private static Dictionary<string, int> _multipliers = new Dictionary<string, int>
        {
            {"тысяча", 1000   },
            {"тысячи", 1000   },
            {"тысяч",  1000   },
            {"миллион",1000000   },
            {"миллиона", 1000000 },
            {"миллионов",1000000 },
            {"миллиард", 1000000000 },
            {"миллиарда",1000000000 },
            {"миллиардов",1000000000},
        };

        public static int GetNumber(string text, int ratio = 100)
        {
            var result = 0;
            var positive = true;
            var i = 0;
            var tokens = text.ToLower().Split(' ');

            if (TryGetValueFuzz(_signs, tokens[0], ratio, out var p))
            {
                positive = p;
                i++;
            }

            for (; i < tokens.Length; i++)
            {
                if (TryGetValueFuzz(_numbers, tokens[i], ratio, out var number))
                {
                    if (i + 1 < tokens.Length && TryGetValueFuzz(_multipliers, tokens[i + 1], ratio, out var multiplier))
                    {
                        number *= multiplier;
                        i++;
                    }

                    result += number;
                }
                else
                    i = tokens.Length;
            }

            if (!positive)
                result *= -1;

            return result;
        }

        private static bool TryGetValueFuzz<K>(Dictionary<string, K> dict, string sample, int ratio, out K value)
        {
            value = default;

            foreach (var item in dict)
            {
                if (Fuzz.WeightedRatio(item.Key, sample) > ratio)
                {
                    value = item.Value;
                    return true;
                }
            }

            return false;
        }
    }

    public static class NumberToTextRus
    {
        //Наименования сотен
        private static string[] hunds =
        {
            "", "сто ", "двести ", "триста ", "четыреста ",
            "пятьсот ", "шестьсот ", "семьсот ", "восемьсот ", "девятьсот "
        };
        //Наименования десятков
        private static string[] tens =
        {
            "", "десять ", "двадцать ", "тридцать ", "сорок ", "пятьдесят ",
            "шестьдесят ", "семьдесят ", "восемьдесят ", "девяносто "
        };

        /// <summary>
        /// Перевод в строку числа с учётом падежного окончания относящегося к числу существительного
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

            int num = val % 1000;
            if (0 == num) return "";
            if (num < 0) throw new ArgumentOutOfRangeException("val", "Параметр не может быть отрицательным");
            if (!male)
            {
                frac20[1] = "одна ";
                frac20[2] = "две ";
            }

            StringBuilder r = new StringBuilder(hunds[num / 100]);

            if (num % 100 < 20)
            {
                r.Append(frac20[num % 100]);
            }
            else
            {
                r.Append(tens[num % 100 / 10]);
                r.Append(frac20[num % 10]);
            }

            r.Append(Case(num, one, two, five));

            if (r.Length != 0) r.Append(" ");
            return r.ToString();
        }

        /// <summary>
        /// Выбор правильного падежного окончания сущесвительного
        /// </summary>
        /// <param name="val">Число</param>
        /// <param name="one">Форма существительного в единственном числе</param>
        /// <param name="two">Форма существительного от двух до четырёх</param>
        /// <param name="five">Форма существительного от пяти и больше</param>
        /// <returns>Возвращает существительное с падежным окончанием, которое соответсвует числу</returns>
        public static string Case(int val, string one, string two, string five)
        {
            int t = (val % 100 > 20) ? val % 10 : val % 20;

            switch (t)
            {
                case 1: return one;
                case 2: case 3: case 4: return two;
                default: return five;
            }
        }

        /// <summary>
        /// Перевод целого числа в строку
        /// </summary>
        /// <param name="val">Число</param>
        /// <returns>Возвращает строковую запись числа</returns>
        public static string Str(int val)
        {
            bool minus = false;
            if (val < 0) { val = -val; minus = true; }

            int n = (int)val;

            StringBuilder r = new StringBuilder();

            if (0 == n) r.Append("0 ");
            if (n % 1000 != 0)
                r.Append(NumberToTextRus.Str(n, true, "", "", ""));

            n /= 1000;

            r.Insert(0, NumberToTextRus.Str(n, false, "тысяча", "тысячи", "тысяч"));
            n /= 1000;

            r.Insert(0, NumberToTextRus.Str(n, true, "миллион", "миллиона", "миллионов"));
            n /= 1000;

            r.Insert(0, NumberToTextRus.Str(n, true, "миллиард", "миллиарда", "миллиардов"));
            n /= 1000;

            r.Insert(0, NumberToTextRus.Str(n, true, "триллион", "триллиона", "триллионов"));
            n /= 1000;

            r.Insert(0, NumberToTextRus.Str(n, true, "триллиард", "триллиарда", "триллиардов"));
            if (minus) r.Insert(0, "минус ");

            return r.ToString();
        }
    }
}
