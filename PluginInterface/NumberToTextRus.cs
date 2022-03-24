
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

        private readonly string _zero = "ноль ";
        private readonly string _oneFemale = "одна ";
        private readonly string _twoFemale = "две ";
        private readonly string _minus = "минус ";

        /// <summary>
        ///     Перевод в строку числа с учётом падежного окончания относящегося к числу существительного
        /// </summary>
        /// <param name="val">Число</param>
        /// <param name="male">Род существительного, которое относится к числу</param>
        /// <param name="one">Форма существительного в единственном числе</param>
        /// <param name="two">Форма существительного от двух до четырёх</param>
        /// <param name="five">Форма существительного от пяти и больше</param>
        /// <returns></returns>
        private string Str(int val, bool male, string one, string two, string five)
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

            r.Append(Case(num, one, two, five));

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
        private string Case(int val, string one, string two, string five)
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

        /// <summary>
        ///     Перевод целого числа в строку
        /// </summary>
        /// <param name="val">Число</param>
        /// <returns>Возвращает строковую запись числа</returns>
        public string ConvertNumberToString(int var)
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

            if (minus)
                r.Insert(0, _minus);

            return r.ToString();
        }
    }
}
