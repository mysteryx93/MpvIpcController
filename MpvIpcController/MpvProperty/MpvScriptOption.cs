namespace HanumanInstitute.MpvIpcController;

/// <summary>
/// Represents a key in the script-opts option dictionary.
/// </summary>
public class MpvScriptOption
{
    private readonly MpvOptionDictionary _options;
    private readonly string _key;

    public MpvScriptOption(MpvApi api, string key)
    {
        api.CheckNotNull(nameof(api));
        _options = new MpvOptionDictionary(api, "script-opts");
        _key = key.CheckNotNull(nameof(key));
    }

    /// <summary>
    /// Gets the value of the script option.
    /// </summary>
    public Task<string?> GetAsync(ApiOptions? options = null) => _options.GetAsync(_key, options);
    /// <summary>
    /// Sets the value of the script option.
    /// </summary>
    public async Task SetAsync(string value, ApiOptions? options = null)
    {
        if (value.HasValue())
        {
            await _options.AddAsync(_key, value, options).ConfigureAwait(false);
        }
        else
        {
            await RemoveAsync(options).ConfigureAwait(false);
        }
    }

    /// <summary>
    /// Removes the value from script options.
    /// </summary>
    public Task RemoveAsync(ApiOptions? options = null) => _options.RemoveAsync(_key, options);
}