using System;
using System.Globalization;
using HanumanInstitute.Validators;

namespace HanumanInstitute.MpvIpcController
{
    /// <summary>
    /// The base class of MPV properties.
    /// </summary>
    /// <typeparam name="TResult">The return type of the property.</typeparam>
    /// <typeparam name="TApi">The API data type before parsing.</typeparam>
    public abstract class MpvProperty<TResult, TApi>
        where TApi : struct
    {
        protected MpvApi Api { get; private set; }
        protected TApi? DefaultValue { get; private set; }
        protected PropertyParser<TResult, TApi?> Parser { get; private set; }

        public MpvProperty(MpvApi api, string name, TApi? defaultValue = null, PropertyParser<TResult, TApi?>? parser = null)
        {
            Api = api;
            PropertyName = name.CheckNotNullOrEmpty(nameof(name));
            DefaultValue = defaultValue;
            Parser = parser ?? DefaultParser;
        }

        /// <summary>
        /// Gets the API name of the property.
        /// </summary>
        public string PropertyName { get; private set; }

        /// <summary>
        /// The default parser to use when TApi and TResult are the same.
        /// </summary>
        protected static TResult DefaultParser(TApi? value)
        {
            if (typeof(TResult) == typeof(TApi))
            {
                return (TResult)Convert.ChangeType(value, typeof(TResult), CultureInfo.InvariantCulture);
            }
            else
            {
                throw new ArgumentException("Parser must be specified if TResult and TApi are different.");
            }
        }

        /// <summary>
        /// The default formatter to use when TApi and TResult are the same.
        /// </summary>
        protected static TApi? DefaultFormatter(TResult value)
        {
            if (typeof(TResult) == typeof(TApi))
            {
                return (TApi)Convert.ChangeType(value, typeof(TApi), CultureInfo.InvariantCulture);
            }
            else
            {
                throw new ArgumentException("Formatter must be specified if TResult and TApi are different.");
            }
        }
    }
}
