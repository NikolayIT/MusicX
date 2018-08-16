namespace MusicX.Common
{
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
    }
}
