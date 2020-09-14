using System;
using System.Diagnostics.CodeAnalysis;

namespace HanumanInstitute.MpvIpcController
{
    public static class MpvFormatters
    {
        [return: MaybeNull]
        public static T ParseDefaultNull<T>(MpvResponse? value)
        {
            try
            {
                return value.ParseData<T>();
            }
            catch (FormatException)
            {
                return default;
            }
        }

        [return: MaybeNull]
        public static T? ParseEnum<T>(MpvResponse? value)
            where T : struct, Enum
        {
            var str = value.ParseData<string?>();
            if (string.Compare(str, "true", StringComparison.InvariantCultureIgnoreCase) == 0)
            {
                str = "yes";
            }
            if (string.Compare(str, "false", StringComparison.InvariantCultureIgnoreCase) == 0)
            {
                str = "no";
            }
            return FlagExtensions.ParseMpvFlag<T>(str);
        }

        public static object? FormatEnum<T>(T? value)
            where T : struct, Enum
        {
            return value?.FormatMpvFlag();
        }
    }
}
