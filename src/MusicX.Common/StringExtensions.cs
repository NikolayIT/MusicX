namespace MusicX.Common
{
    using System;
    using System.Globalization;
    using System.Linq;
    using System.Security.Cryptography;
    using System.Text;
    using System.Text.RegularExpressions;
    using System.Web;

    public static class StringExtensions
    {
        public static string ReplaceCaseInsensitive(this string input, string search, string replacement)
        {
            /* TODO: Test
                var tests = new[] {
                    new { Input="abcdef", Search="abc", Replacement="xyz", Expected="xyzdef" },
                    new { Input="ABCdef", Search="abc", Replacement="xyz", Expected="xyzdef" },
                    new { Input="A*BCdef", Search="a*bc", Replacement="xyz", Expected="xyzdef" },
                    new { Input="abcdef", Search="abc", Replacement="x*yz", Expected="x*yzdef" },
                    new { Input="abcdef", Search="abc", Replacement="$", Expected="$def" },
                };
            */
            var result = Regex.Replace(
                input,
                Regex.Escape(search),
                replacement.Replace("$", "$$"),
                RegexOptions.IgnoreCase);

            return result;
        }

        public static string HtmlDecodeSpecialChars(this string inputString)
        {
            return HttpUtility.HtmlDecode(inputString);
        }

        public static string TrimDashes(this string input)
        {
            return input?.Trim().Trim('-').Trim();
        }

        public static string GetStringBetween(
            this string input,
            string startString,
            string endString,
            int startFrom = 0)
        {
            input = input.Substring(startFrom);
            if (!input.Contains(startString) || !input.Contains(endString))
            {
                return string.Empty;
            }

            var startPosition = input.IndexOf(startString, StringComparison.Ordinal) + startString.Length;
            if (startPosition == -1)
            {
                return string.Empty;
            }

            var endPosition = input.IndexOf(endString, startPosition, StringComparison.Ordinal);
            if (endPosition == -1)
            {
                return string.Empty;
            }

            return input.Substring(startPosition, endPosition - startPosition);
        }

        public static string StripHtmlTags(this string inputString) =>
            Regex.Replace(inputString, "<.*?>", string.Empty);

        public static string ToMd5Hash(this string input)
        {
            using (var md5 = MD5.Create())
            {
                return string.Join(
                    string.Empty,
                    md5.ComputeHash(Encoding.UTF8.GetBytes(input)).Select(x => x.ToString("X2"))).ToLower();
            }
        }

        public static string CapitalizeFirstLetter(this string input)
        {
            if (string.IsNullOrEmpty(input))
            {
                return input;
            }

            return input.Substring(0, 1).ToUpper(CultureInfo.CurrentCulture) + input.Substring(1, input.Length - 1);
        }

        public static string ConvertCyrillicToLatinLetters(this string input)
        {
            var cyrillicLetters = new[]
                                  {
                                      "а", "б", "в", "г", "д", "е", "ж", "з", "и", "й", "к", "л", "м", "н", "о", "п",
                                      "р", "с", "т", "у", "ф", "х", "ц", "ч", "ш", "щ", "ъ", "ь", "ю", "я"
                                  };
            var latinLetters = new[]
                               {
                                   "a", "b", "v", "g", "d", "e", "j", "z", "i", "y", "k", "l", "m", "n", "o", "p", "r",
                                   "s", "t", "u", "f", "h", "c", "ch", "sh", "sht", "u", "i", "yu", "ya"
                               };
            for (var index = 0; index < cyrillicLetters.Length; ++index)
            {
                input = input.Replace(cyrillicLetters[index], latinLetters[index]);
                input = input.Replace(cyrillicLetters[index].ToUpper(), latinLetters[index].CapitalizeFirstLetter());
            }

            return input;
        }

        public static string ConvertLatinToCyrillicLetters(this string input)
        {
            var latinLetters = new[]
                               {
                                   "sht", "sh", "ck", "th",
                                   "a", "b", "c", "d", "e", "f", "g", "h", "i", "j", "k", "l", "m", "n", "o", "p", "q",
                                   "r", "s", "t", "u", "v", "w", "x", "y", "z"
                               };
            var cyrillicLetters = new[]
                                  {
                                      "щ", "ш", "к", "д",
                                      "а", "б", "ц", "д", "е", "ф", "г", "х", "и", "й", "к", "л", "м", "н", "о", "п",
                                      "кю", "р", "с", "т", "у", "в", "у", "кс", "я", "з"
                                  };
            for (var index = 0; index < latinLetters.Length; ++index)
            {
                input = input.Replace(latinLetters[index], cyrillicLetters[index]);
                input = input.Replace(latinLetters[index].CapitalizeFirstLetter(), cyrillicLetters[index].CapitalizeFirstLetter());
            }

            return input;
        }
    }
}
