namespace HanumanInstitute.MpvIpcController;

/// <summary>
/// Represents a comma-delimited option list of key-value pairs. ex: Val1=a,Val2=b
/// </summary>
public class MpvOptionDictionary : MpvOptionRef<IDictionary<string, string>>
{
    private readonly char _separator;

    public MpvOptionDictionary(MpvApi api, string name, bool isPath = false) : base(api, name)
    {
        _separator = isPath ? System.IO.Path.PathSeparator : ',';
    }

    private static string FormatKeyValue(string key, string value) => EscapeValue(key) + "=" + EscapeValue(value);

    private string FormatKeyValueList(IDictionary<string, string> values) => string.Join(_separator.ToString(), values.Select(x => FormatKeyValue(x.Key, x.Value)));

    private static string EscapeValue(string value)
    {
        if (string.IsNullOrEmpty(value))
        {
            return "";
        }
        else
        {
            var length = System.Text.Encoding.UTF8.GetByteCount(value);
            return "%{0}%{1}".FormatInvariant(length, value);
        }
    }

    /// <summary>
    /// Sets a dictionary of key/value pairs.
    /// </summary>
    public override Task SetAsync(IDictionary<string, string> values, ApiOptions? options = null) => Api.ChangeListAsync(PropertyName, ListOptionOperation.Set, FormatKeyValueList(values), options);

    /// <summary>
    /// Gets the value of specified key.
    /// </summary>
    public async Task<string?> GetAsync(string key, ApiOptions? options = null)
    {
        var values = await GetAsync(options).ConfigureAwait(false);
        if (values != null && values.TryGetValue(key, out var result))
        {
            return result;
        }
        return null;
    }

    /// <summary>
    /// Gets a dictionary containing all values.
    /// </summary>
    //public override async Task<IDictionary<string, string>?> GetAsync(ApiOptions? options = null)
    //{
    //    var values = await Api.GetPropertyAsync(PropertyName, options).ConfigureAwait(false);
    //    return ParseValue(values.Data) ?? new Dictionary<string, string>();
    //}

    /// <summary>
    /// Adds a key/value pair to the list.
    /// </summary>
    public async Task AddAsync(string key, string value, ApiOptions? options = null)
    {
        key.CheckNotNullOrEmpty(nameof(key));

        await Api.ChangeListAsync(PropertyName, ListOptionOperation.Add, FormatKeyValue(key, value), options).ConfigureAwait(false);
    }

    /// <summary>
    /// Adds a dictionary of key/value pairs to the list.
    /// </summary>
    public override Task AddAsync(IDictionary<string, string> values, ApiOptions? options = null) => Api.ChangeListAsync(PropertyName, ListOptionOperation.Add, FormatKeyValueList(values), options);

    /// <summary>
    /// Delete item if present (does not interpret escapes).
    /// </summary>
    public async Task RemoveAsync(string key, ApiOptions? options = null)
    {
        key.CheckNotNullOrEmpty(nameof(key));

        await Api.ChangeListAsync(PropertyName, ListOptionOperation.Remove, key, options).ConfigureAwait(false);
    }
}