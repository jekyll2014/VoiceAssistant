using System;
using System.IO;
using System.Text;

namespace PluginInterface
{
    public class NumberToTextGeneric : INumberToText
    {
        private const string _settingsConfigFile = "GenericNumberSettings.json";

        private readonly string _zero;
        private readonly string _oneFemale;
        private readonly string _twoFemale;
        private readonly string _minus;
        private readonly string[] _frac20;
        private readonly string[] _hundreds;
        private readonly string[] _tens;
        private readonly string[] _OneTwoFiveThousand;
        private readonly string[] _OneTwoFiveMillion;
        private readonly string[] _OneTwoFiveBillion;
        private readonly string[] _OneTwoFiveTrillion;
        private readonly string[] _OneTwoFiveQuadrillion;

        public NumberToTextGeneric()
        {
            var configBuilder = new Config<GenericNumberSettings>($"{_settingsConfigFile}");
            if (!File.Exists($"{_settingsConfigFile}"))
            {
                configBuilder.SaveConfig();
            }

            var configStorage = configBuilder.ConfigStorage;

            _minus = configStorage.Minus;
            _zero = configStorage.Zero;
            _oneFemale = configStorage.OneFemale;
            _twoFemale = configStorage.TwoFemale;

            _frac20 = new[]
            {
                "",
                configStorage.One,
                configStorage.Two,
                configStorage.Three,
                configStorage.Four,
                configStorage.Five,
                configStorage.Six,
                configStorage.Seven,
                configStorage.Eight,
                configStorage.Nine,
                configStorage.Ten,
                configStorage.Elleven,
                configStorage.Twelve,
                configStorage.Thirteen,
                configStorage.Fourteen,
                configStorage.Fifteen,
                configStorage.Sixteen,
                configStorage.Seventeen,
                configStorage.Eighteen,
                configStorage.Nineteen,
            };

            _tens = new[]
            {
                "",
                configStorage.Ten,
                configStorage.Twenty,
                configStorage.Thirty,
                configStorage.Forty,
                configStorage.Fifty,
                configStorage.Sixty,
                configStorage.Seventy,
                configStorage.Eighty,
                configStorage.Ninety
            };

            _hundreds = new[]
            {
                "",
                configStorage.OneHundred,
                configStorage.TwoHundred,
                configStorage.ThreeHundred,
                configStorage.FourHundred,
                configStorage.FiveHundred,
                configStorage.SixHundred,
                configStorage.SevenHundred,
                configStorage.EightHundred,
                configStorage.NineHundred,
            };

            _OneTwoFiveThousand = new[]
            {
                configStorage.OneThousand,
                configStorage.TwoThousand,
                configStorage.FiveThousand
            };

            _OneTwoFiveMillion = new[]
            {
                configStorage.OneMillion,
                configStorage.TwoMillion,
                configStorage.FiveMillion
            };

            _OneTwoFiveBillion = new[]
            {
                configStorage.OneBillion,
                configStorage.TwoBillion,
                configStorage.FiveBillion
            };

            _OneTwoFiveTrillion = new[]
            {
                configStorage.OneTrillion,
                configStorage.TwoTrillion,
                configStorage.FiveTrillion
            };

            _OneTwoFiveQuadrillion = new[]
            {
                configStorage.OneQuadrillion,
                configStorage.TwoQuadrillion,
                configStorage.FiveQuadrillion
            };
        }

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








