using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;

namespace Hamster.Plugin
{
    public static class Glob
    {
        public static bool IsMatch(string input, string pattern)
        {
            RegexOptions options = RegexOptions.Singleline | RegexOptions.IgnoreCase | RegexOptions.CultureInvariant;
            return Regex.IsMatch(input, ToRegexPattern(pattern), options);
        }

        public static string ToRegexPattern(string globPattern)
        {
            return "^" + Regex.Replace(globPattern, @"\*\*|[^\w]", RegexTransform) + "$";
        }

        public static Regex CreateRegex(string pattern)
        {
            return CreateRegex(pattern, false);
        }

        public static Regex CreateRegex(string pattern, bool compile)
        {
            RegexOptions options = RegexOptions.Singleline | RegexOptions.IgnoreCase | RegexOptions.CultureInvariant;
            if (compile)
            {
                options |= RegexOptions.Compiled;
            }

            return CreateRegex(pattern, options);
        }

        public static Regex CreateRegex(string pattern, RegexOptions options)
        {
            return new Regex(ToRegexPattern(pattern), options);
        }

        private static string RegexTransform(Match m)
        {
            switch (m.Value)
            {
                case "**":
                    return ".*";

                case "*":
                    return @"[^/\\]*";

                case "/":
                case @"\":
                    return @"[/\\]+";

                default:
                    return '\\' + m.Value;
            }
        }
    }
}
