using System;
using System.Linq;
using System.Text.RegularExpressions;

namespace HanumanInstitute.MpvIpcController
{
    internal static class FlagExtensions
    {
        /// <summary>
        /// Returns a flag name in MPV format, adding '-' between words. "EachFrame" becomes "each-frame".
        /// </summary>
        /// <param name="flag">The flag value to format.</param>
        /// <returns>The flag name in MPV's syntax.</returns>
        public static string GetFlagName<T>(this T flag)
            where T : Enum
        {
            var value = flag.ToString();
            value = s_regexFlagName.Replace(value, x => $"{x.Value[0]}-{x.Value[1]}");
            return value.ToLowerInvariant();
        }
        private static readonly Regex s_regexFlagName = new Regex("[a-z][A-Z]");

        /// <summary>
        /// Returns a list of flags in MPV format, combining flags with '+'. "EachFrame" "More" becomes "each-frame+more"
        /// </summary>
        /// <typeparam name="T">The type of flag enumeration.</typeparam>
        /// <param name="flags">The type of the flag array to normalize.</param>
        /// <returns>A string representation of the flags.</returns>
        public static string? NormalizeFlags<T>(this T[] flags)
            where T : Enum
        {
            if (flags == null || flags.Length == 0)
            {
                return null;
            }

            var values = flags.Select(x => x.GetFlagName());
            return string.Join("+", values);
        }
    }
}
