using System;

namespace PluginInterface
{
    public class NumberToTextEng : INumberToText
    {
        //converts any number between 0 & INT_MAX (2,147,483,647)
        public string ConvertNumberToString(long val)
        {
            var result = string.Empty;

            if (val < 0)
            {
                result = "minus ";
                val = -val;
            }

            if (val == 0)
                result = "zero";
            else if (val < 10)
                result += ConvertDigitToString(val);
            else if (val < 20)
                result += ConvertTeensToString(val);
            else if (val < 100)
                result += ConvertHighTensToString(val);
            else if (val < 1000)
                result += ConvertBigNumberToString(val, (long)1e2, "hundred");
            else if (val < 1e6)
                result += ConvertBigNumberToString(val, (long)1e3, "thousand");
            else if (val < 1e9)
                result += ConvertBigNumberToString(val, (long)1e6, "million");
            else if (val < 1e12)
                result += ConvertBigNumberToString(val, (long)1e9, "billion");
            else if (val < 1e12)
                result += ConvertBigNumberToString(val, (long)1e12, "trillion");
            else if (val < 1e12)
                result += ConvertBigNumberToString(val, (long)1e15, "quadrillion");
            else if (val < 1e12)
                result += ConvertBigNumberToString(val, (long)1e18, "quintillion");
            else
                return "more than quintillion";

            return result;
        }

        private string ConvertDigitToString(long i)
        {
            switch (i)
            {
                case 0: return "";
                case 1: return "one";
                case 2: return "two";
                case 3: return "three";
                case 4: return "four";
                case 5: return "five";
                case 6: return "six";
                case 7: return "seven";
                case 8: return "eight";
                case 9: return "nine";
                default:
                    throw new IndexOutOfRangeException(String.Format("{0} not a digit", i));
            }
        }

        //assumes a number between 10 & 19
        private string ConvertTeensToString(long n)
        {
            switch (n)
            {
                case 10: return "ten";
                case 11: return "eleven";
                case 12: return "twelve";
                case 13: return "thirteen";
                case 14: return "fourteen";
                case 15: return "fiveteen";
                case 16: return "sixteen";
                case 17: return "seventeen";
                case 18: return "eighteen";
                case 19: return "nineteen";
                default:
                    throw new IndexOutOfRangeException(String.Format("{0} not a teen", n));
            }
        }

        //assumes a number between 20 and 99
        private string ConvertHighTensToString(long n)
        {
            long tensDigit = (long)(Math.Floor((double)n / 10.0));

            string tensStr;
            switch (tensDigit)
            {
                case 2: tensStr = "twenty"; break;
                case 3: tensStr = "thirty"; break;
                case 4: tensStr = "forty"; break;
                case 5: tensStr = "fifty"; break;
                case 6: tensStr = "sixty"; break;
                case 7: tensStr = "seventy"; break;
                case 8: tensStr = "eighty"; break;
                case 9: tensStr = "ninety"; break;
                default:
                    throw new IndexOutOfRangeException(String.Format("{0} not in range 20-99", n));
            }
            if (n % 10 == 0) return tensStr;
            string onesStr = ConvertDigitToString(n - tensDigit * 10);
            return tensStr + "-" + onesStr;
        }

        // Use this to convert any integer bigger than 99
        private string ConvertBigNumberToString(long n, long baseNum, string baseNumStr)
        {
            // special case: use commas to separate portions of the number, unless we are in the hundreds
            string separator = (baseNumStr != "hundred") ? ", " : " ";

            // Strategy: translate the first portion of the number, then recursively translate the remaining sections.
            // Step 1: strip off first portion, and convert it to string:
            long bigPart = (long)(Math.Floor((double)n / baseNum));
            string bigPartStr = ConvertNumberToString(bigPart) + " " + baseNumStr;
            // Step 2: check to see whether we're done:
            if (n % baseNum == 0) return bigPartStr;
            // Step 3: concatenate 1st part of string with recursively generated remainder:
            long restOfNumber = n - bigPart * baseNum;
            return bigPartStr + separator + ConvertNumberToString(restOfNumber);
        }
    }
}








