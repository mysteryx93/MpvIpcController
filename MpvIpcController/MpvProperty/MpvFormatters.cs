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
    }
}
