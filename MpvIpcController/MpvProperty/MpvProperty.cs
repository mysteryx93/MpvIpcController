using System;
using System.Threading.Tasks;
using HanumanInstitute.Validators;

namespace HanumanInstitute.MpvIpcController
{
    /// <summary>
    /// The base class of MPV properties.
    /// </summary>
    /// <typeparam name="T">The return type of the property.</typeparam>
    /// <typeparam name="TApi">The API data type before parsing.</typeparam>
    public abstract class MpvProperty<T>
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
        /// Watches a property for changes. If the given property is changed, then an event 'property-change' will be generated.
        /// </summary>
        /// <param name="observeId">An ID that will be passed to the generated events as parameter 'id'.</param>
        public Task ObserveAsync(int observeId, ApiOptions? options = null) =>
            Api.ObservePropertyAsync(observeId, PropertyName, options);

        /// <summary>
        /// Undo ObserveProperty or ObservePropertyString. This requires the numeric id passed to the observed command as argument.
        /// </summary>
        /// <param name="observeId">The ID of the observer.</param>
        public Task UnobservePropertyAsync(int observeId, ApiOptions? options = null) =>
            Api.UnobservePropertyAsync(observeId, options);
    }

    ///// <summary>
    ///// The base class of MPV properties.
    ///// </summary>
    ///// <typeparam name="TResult">The return type of the property.</typeparam>
    ///// <typeparam name="TApi">The API data type before parsing.</typeparam>
    //public abstract class MpvProperty<TResult, TApi>
    //{
    //    protected MpvApi Api { get; private set; }
    //    protected PropertyParser<TResult, TApi> Parser { get; private set; }

    //    public MpvProperty(MpvApi api, string name, PropertyParser<TResult, TApi>? parser = null)
    //    {
    //        Api = api;
    //        PropertyName = name.CheckNotNullOrEmpty(nameof(name));
    //        Parser = parser ?? MpvFormatters.ParseDefault<TResult, TApi>;
    //    }

    //    /// <summary>
    //    /// Gets the API name of the property.
    //    /// </summary>
    //    public string PropertyName { get; private set; }

    //    /// <summary>
    //    /// Watches a property for changes. If the given property is changed, then an event 'property-change' will be generated.
    //    /// </summary>
    //    /// <param name="observeId">An ID that will be passed to the generated events as parameter 'id'.</param>
    //    public Task ObserveAsync(int observeId, MpvCommandOptions? options = null) =>
    //        Api.ObservePropertyAsync(observeId, PropertyName, options);

    //    /// <summary>
    //    /// Undo ObserveProperty or ObservePropertyString. This requires the numeric id passed to the observed command as argument.
    //    /// </summary>
    //    /// <param name="observeId">The ID of the observer.</param>
    //    public Task UnobservePropertyAsync(int observeId, MpvCommandOptions? options = null) =>
    //        Api.UnobservePropertyAsync(observeId, options);
    //}
}
