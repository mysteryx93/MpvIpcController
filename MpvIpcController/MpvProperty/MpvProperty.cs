using System;
using HanumanInstitute.Validators;

namespace HanumanInstitute.MpvIpcController
{
    /// <summary>
    /// The base class of MPV properties.
    /// </summary>
    /// <typeparam name="TResult">The return type of the property.</typeparam>
    /// <typeparam name="TApi">The API data type before parsing.</typeparam>
    public abstract class MpvProperty<TResult, TApi>
    {
        protected MpvApi Api { get; private set; }
        protected PropertyParser<TResult, TApi> Parser { get; private set; }

        public MpvProperty(MpvApi api, string name, PropertyParser<TResult, TApi>? parser = null)
        {
            Api = api;
            PropertyName = name.CheckNotNullOrEmpty(nameof(name));
            Parser = parser ?? MpvFormatters.ParseDefault<TResult, TApi>;
        }

        /// <summary>
        /// Gets the API name of the property.
        /// </summary>
        public string PropertyName { get; private set; }
    }
}
