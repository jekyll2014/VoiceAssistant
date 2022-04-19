// This is an independent project of an individual developer. Dear PVS-Studio, please check it.
// PVS-Studio Static Code Analyzer for C, C++, C#, and Java: http://www.viva64.com

using System;
using System.Text;

namespace PluginInterface
{
    public class NumberToTextRus : INumberToText
    {
        //Наименования сотен
        private readonly string[] _hundreds =
            {
            "", "сто ", "двести ", "триста ", "четыреста ",
            "пятьсот ", "шестьсот ", "семьсот ", "восемьсот ", "девятьсот "
        };

        //Наименования десятков
        private readonly string[] _tens =
            {
            "", "десять ", "двадцать ", "тридцать ", "сорок ", "пятьдесят ",
            "шестьдесят ", "семьдесят ", "восемьдесят ", "девяносто "
        };

        private readonly string[] _frac20 =
            {
                "", "один ", "два ", "три ", "четыре ", "пять ", "шесть ",
                "семь ", "восемь ", "девять ", "десять ", "одиннадцать ",
                "двенадцать ", "тринадцать ", "четырнадцать ", "пятнадцать ",
                "шестнадцать ", "семнадцать ", "восемнадцать ", "девятнадцать "
            };

        private readonly string[] _OneTwoFiveThousand = { "тысяча", "тысячи", "тысяч" };
        private readonly string[] _OneTwoFiveMillion = { "миллион", "миллиона", "миллионов" };
        private readonly string[] _OneTwoFiveBillion = { "миллиард", "миллиарда", "миллиардов" };
        private readonly string[] _OneTwoFiveTrillion = { "триллион", "триллиона", "триллионов" };
        private readonly string[] _OneTwoFiveQuadrillion = { "квадриллион", "квадриллиона", "квадриллионов" };

        private readonly string _zero = "ноль ";
        private readonly string _oneFemale = "одна ";
        private readonly string _twoFemale = "две ";
        private readonly string _minus = "минус ";

        /// <summary>
        ///     Перевод целого числа в строку
        /// </summary>
        /// <param name="val">Число</param>
        /// <returns>Возвращает строковую запись числа</returns>
        public string ConvertNumberToString(long var)
        {
            var minus = false;

            if (var < 0)
            {
                var = -var;
                minus = true;
            }

            var n = var;

            var r = new StringBuilder();

            if (0 == n)
                r.Append(_zero);

            if (n % 1000 != 0)
                r.Append(Str(n, true, new[] { "", "", "" }));

            n /= 1000;

            r.Insert(0, Str(n, false, _OneTwoFiveThousand));
            n /= 1000;

            r.Insert(0, Str(n, true, _OneTwoFiveMillion));
            n /= 1000;

            r.Insert(0, Str(n, true, _OneTwoFiveBillion));
            n /= 1000;

            r.Insert(0, Str(n, true, _OneTwoFiveTrillion));
            n /= 1000;

            r.Insert(0, Str(n, true, _OneTwoFiveQuadrillion));

            if (minus)
                r.Insert(0, _minus);

            return r.ToString();
        }

        /// <summary>
        ///     Перевод в строку числа с учётом падежного окончания относящегося к числу существительного
        /// </summary>
        /// <param name="val">Число</param>
        /// <param name="male">Род существительного, которое относится к числу</param>
        /// <param name="one">Форма существительного в единственном числе</param>
        /// <param name="two">Форма существительного от двух до четырёх</param>
        /// <param name="five">Форма существительного от пяти и больше</param>
        /// <returns></returns>
        private string Str(long val, bool male, string[] oneTwoFive)
        {
            var num = val % 1000;
            if (0 == num) return "";

            if (num < 0) throw new ArgumentOutOfRangeException(nameof(val), "Parameter can't be less than zero");

            if (!male)
            {
                _frac20[1] = _oneFemale + " ";
                _frac20[2] = _twoFemale + " ";
            }

            var r = new StringBuilder(_hundreds[num / 100] + " ");

            if (num % 100 < 20)
            {
                r.Append(_frac20[num % 100] + " ");
            }
            else
            {
                r.Append(_tens[num % 100 / 10] + " ");
                r.Append(_frac20[num % 10] + " ");
            }

            r.Append(Case(num, oneTwoFive[0], oneTwoFive[1], oneTwoFive[2]));

            if (r.Length != 0)
                r.Append(" ");

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
        private string Case(long val, string one, string two, string five)
        {
            var t = val % 100 > 20 ? val % 10 : val % 20;

            switch (t)
            {
                case 1: return one + " ";
                case 2:
                case 3:
                case 4: return two + " ";
                default: return five + " ";
            }
        }
    }
}
