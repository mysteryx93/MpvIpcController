using System;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using HanumanInstitute.Validators;

namespace HanumanInstitute.MpvIpcController
{
    public static class MpvFormatters
    {
        /// <summary>
        /// The default parser to use when TApi and TResult are the same.
        /// </summary>
        [return: MaybeNull]
        public static TResult ParseDefault<TResult, TApi>(TApi value)
        {
            if (typeof(TResult) == typeof(TApi))
            {
                return (TResult)(object?)value;
            }
            else
            {
                throw new ArgumentException("Parser must be specified if TResult and TApi are different.");
            }
        }

        /// <summary>
        /// The default formatter to use when TApi and TResult are the same.
        /// </summary>
        [return: MaybeNull]
        public static TApi FormatDefault<TResult, TApi>(TResult value)
        {
            if (typeof(TResult) == typeof(TApi))
            {
                return (TApi)(object?)value;
            }
            else
            {
                throw new ArgumentException("Formatter must be specified if TResult and TApi are different.");
            }
        }

        /// <summary>
        /// Formats specified bool value as a string.
        /// </summary>
        public static string? FormatBool(bool? value)
        {
            if (value == null) { return null; }

            return value.Value ? "yes" : "no";
        }

        /// <summary>
        /// Parses specified value as a bool.
        /// </summary>
        public static bool? ParseBool(string? value)
        {
            if (string.IsNullOrEmpty(value))
            {
                return null;
            }
            else if (string.Compare(value, "yes", StringComparison.InvariantCultureIgnoreCase) == 0 || string.Compare(value, "true", StringComparison.InvariantCultureIgnoreCase) == 0)
            {
                return true;
            }
            else if (string.Compare(value, "no", StringComparison.InvariantCultureIgnoreCase) == 0 || string.Compare(value, "false", StringComparison.InvariantCultureIgnoreCase) == 0)
            {
                return false;
            }
            else
            {
                throw new NotSupportedException(@"Value ""{0}"" cannot be converted to boolean.".FormatInvariant(value));
            }
        }
    }
}
