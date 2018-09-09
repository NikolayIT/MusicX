namespace MusicX.Common
{
    using System;
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

        public static string GetStringBetween(this string input, string startString, string endString, int startFrom = 0)
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
                    md5.ComputeHash(Encoding.ASCII.GetBytes(input)).Select(x => x.ToString("X2"))).ToLower();
            }
        }
    }
}
