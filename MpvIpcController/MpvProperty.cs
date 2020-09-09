using System;
using System.Globalization;
using System.Threading.Tasks;
using HanumanInstitute.Validators;

namespace HanumanInstitute.MpvIpcController
{
    /// <summary>
    /// THe base class of MPV properties.
    /// </summary>
    /// <typeparam name="TResult">The return type of the property.</typeparam>
    /// <typeparam name="TApi">The API data type before parsing.</typeparam>
    public abstract class MpvProperty<TResult, TApi>
    {
        protected MpvApi Api { get; private set; }
        protected TApi DefaultValue { get; private set; }
        protected PropertyParser<TResult, TApi> Parser { get; private set; }

        public MpvProperty(MpvApi api, string name, TApi defaultValue, PropertyParser<TResult, TApi>? parser = null)
        {
            Api = api;
            PropertyName = name.CheckNotNullOrEmpty(nameof(name));
            DefaultValue = defaultValue;

            if (parser != null)
            {
                Parser = parser;
            }
            else
            {
                // Default parser if TIndex and TApi are the same.
                Parser = x =>
                {
                    if (typeof(TResult) == typeof(TApi))
                    {
                        return (TResult)Convert.ChangeType(x, typeof(TResult), CultureInfo.InvariantCulture);
                    }
                    else
                    {
                        throw new ArgumentException("Parser must be specified if TResult and TApi are different.", nameof(parser));
                    }
                };
            }
        }

        /// <summary>
        /// Gets the API name of the property.
        /// </summary>
        public string PropertyName { get; private set; }
    }

    public delegate TResult PropertyParser<TResult, TApi>(TApi value);
    public delegate TApi PropertyFormatter<TApi, TResult>(TResult value);
}
