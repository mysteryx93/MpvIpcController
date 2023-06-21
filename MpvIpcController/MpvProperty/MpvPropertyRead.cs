namespace HanumanInstitute.MpvIpcController;

/// <summary>
/// Represents a read-only MPV property.
/// </summary>
/// <typeparam name="T">The return type of the property.</typeparam>
public class MpvPropertyRead<T> : MpvProperty<T?>
    where T : struct
{
    public MpvPropertyRead(MpvApi api, string name) : base(api, name)
    {
    }

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

    /// <summary>
    /// Returns the value of the given property. The value will be sent in the data field of the replay message.
    /// </summary>
    public async Task<T?> GetAsync(ApiOptions? options = null)
    {
        var result = await GetRawAsync(options).ConfigureAwait(false);
        return ParseValue(result);
    }

    /// <summary>
    /// Returns the value of the given property. The value will be sent in the data field of the replay message.
    /// </summary>
    public async Task<string?> GetRawAsync(ApiOptions? options = null)
    {
        var result = await Api.GetPropertyAsync(PropertyName, options).ConfigureAwait(false);
        return result.Data;
    }
}

/// <summary>
/// Represents a read-only MPV property.
/// </summary>
/// <typeparam name="T">The return type of the property.</typeparam>
public class MpvPropertyReadRef<T> : MpvProperty<T?>
    where T : class
{
    public MpvPropertyReadRef(MpvApi api, string name) : base(api, name)
    {
    }

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

    /// <summary>
    /// Returns the value of the given property. The value will be sent in the data field of the replay message.
    /// </summary>
    public virtual async Task<T?> GetAsync(ApiOptions? options = null)
    {
        var result = await GetRawAsync(options).ConfigureAwait(false);
        return ParseValue(result);
    }

    /// <summary>
    /// Returns the value of the given property. The value will be sent in the data field of the replay message.
    /// </summary>
    public async Task<string?> GetRawAsync(ApiOptions? options = null)
    {
        var result = await Api.GetPropertyAsync(PropertyName, options).ConfigureAwait(false);
        return result.Data;
    }
}

/// <summary>
/// Represents a read-only MPV property of type String.
/// </summary>
public class MpvPropertyReadString : MpvPropertyReadRef<string>
{
    public MpvPropertyReadString(MpvApi api, string name) : base(api, name)
    {
    }
}