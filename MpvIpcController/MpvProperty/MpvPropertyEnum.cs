using System;

namespace HanumanInstitute.MpvIpcController
{
    public class MpvPropertyEnum<T> : MpvPropertyWrite<T>
        where T : struct, Enum
    {
        public MpvPropertyEnum(MpvApi api, string name) : base(api, name)
        {
        }

        /// <summary>
        /// Parse value as specified type.
        /// </summary>
        /// <param name="value">The raw value to parse.</param>
        /// <returns>The typed parsed value.</returns>
        protected override T? ParseValue(string? value)
        {
            //if (string.Compare(value, "true", StringComparison.InvariantCultureIgnoreCase) == 0)
            //{
            //    value = "yes";
            //}
            //if (string.Compare(value, "false", StringComparison.InvariantCultureIgnoreCase) == 0)
            //{
            //    value = "no";
            //}
            return FlagExtensions.ParseMpvFlag<T>(value);
        }

        /// <summary>
        /// Formats specified value to send into a MPV request.
        /// </summary>
        /// <param name="value">The value to format.</param>
        /// <returns>The formatted value.</returns>
        protected override string? FormatValue(T? value) => value?.FormatMpvFlag() ?? null;
    }
}
