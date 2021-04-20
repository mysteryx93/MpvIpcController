using System;
using System.Globalization;
using System.Text.Json;
using HanumanInstitute.Validators;

namespace HanumanInstitute.MpvIpcController
{
    /// <summary>
    /// Represents a read-only MPV property.
    /// </summary>
    /// <typeparam name="T">The return type of the property.</typeparam>
    /// /// <typeparam name="T">The nullable return type of the property.</typeparam>
    public abstract class MpvProperty<TNull>
    {
        protected MpvApi Api { get; private set; }

        public MpvProperty(MpvApi api, string name)
        {
            Api = api;
            PropertyName = name.CheckNotNullOrEmpty(nameof(name));
        }

        /// <summary>
        /// Gets the API name of the property.
        /// </summary>
        public string PropertyName { get; private set; }

        /// <summary>
        /// Parse value as specified type.
        /// </summary>
        /// <param name="value">The raw value to parse.</param>
        /// <returns>The typed parsed value.</returns>
        /// <exception cref="FormatException">Value is not in a valid format.</exception>
        /// <exception cref="OverflowException">Value represents a number that is out of the range.</exception>
        protected virtual TNull ParseValue(string? value) => ParseDefault(value);


        public static TNull ParseDefault(string? value)
        {
            if (value == null) { return default!; }

            if (typeof(TNull) == typeof(string))
            {
                var str = value.ToString();
                if (str.Length >= 2 && str[0] == '"' && str[str.Length - 1] == '"')
                {
                    str = str.Substring(1, str.Length - 2);
                }
                return (TNull)(object)str;
            }
            if (typeof(TNull).IsValueType)
            {
                var type = Nullable.GetUnderlyingType(typeof(TNull)) ?? typeof(TNull);
                return (TNull)Convert.ChangeType(value, type, CultureInfo.InvariantCulture);
            }
            else
            {
                var jsonOptions = new JsonSerializerOptions()
                {
                    PropertyNamingPolicy = new MpvJsonNamingPolicy()
                };
                return (TNull)JsonSerializer.Deserialize<TNull>(value, jsonOptions)! ?? default!;
            }
        }

        /// <summary>
        /// Formats specified value to send into a MPV request.
        /// </summary>
        /// <param name="value">The value to format.</param>
        /// <returns>The formatted value.</returns>
        protected virtual string? FormatValue(TNull value) => value?.ToStringInvariant();
    }
}
