namespace HanumanInstitute.MpvIpcController;

/// <summary>
/// Exposes the Metadata sub-properties.
/// </summary>
public class MetadataProperties
{
    private readonly MpvApi _api;
    private readonly string _prefix;

    public MetadataProperties(MpvApi api, string propertyName)
    {
        _api = api;
        _prefix = propertyName;
    }

    /// <summary>
    /// Metadata key/value pairs.
    /// </summary>
    public MpvPropertyReadRef<IDictionary<string, string>> Metadata => new(_api, _prefix);

    /// <summary>
    /// Value of metadata entry 'key'.
    /// </summary>
    public MpvPropertyIndexReadRef<string, string> MetadataByKey => new(_api, _prefix + "/by-key/{0}");

    /// <summary>
    /// Number of metadata entries.
    /// </summary>
    public MpvPropertyRead<int> MetadataListCount => new(_api, _prefix + "/list/count");

    /// <summary>
    /// Key name of the Nth metadata entry. (The first entry is 0).
    /// </summary>
    public MpvPropertyIndexReadRef<int, string> MetadataListKey => new(_api, _prefix + "/list/{0}/key");

    /// <summary>
    /// Value of the Nth metadata entry.
    /// </summary>
    public MpvPropertyIndexReadRef<int, string> MetadataListValue => new(_api, _prefix + "/list/{0}/value");
}