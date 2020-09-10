using System;
using System.ComponentModel;
using System.Linq;
using System.Globalization;
using HanumanInstitute.Validators;

namespace HanumanInstitute.MpvIpcController
{
    /// <summary>
    /// The base class of MPV properties. This is an exact copy of MpvProperty but with "where TApi : class".
    /// </summary>
    /// <typeparam name="TResult">The return type of the property.</typeparam>
    /// <typeparam name="TApi">The API data type before parsing.</typeparam>
    public abstract class MpvPropertyC<TResult, TApi>
        where TApi : class
    {
        protected MpvApi Api { get; private set; }
        protected TApi? DefaultValue { get; private set; }
        protected PropertyParser<TResult, TApi?> Parser { get; private set; }

        public MpvPropertyC(MpvApi api, string name, TApi? defaultValue = null, PropertyParser<TResult, TApi?>? parser = null)
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
                return (TResult)(object)value!;
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
                return (TApi)(object)value!;
            }
            else
            {
                throw new ArgumentException("Formatter must be specified if TResult and TApi are different.");
            }
        }
    }
}
