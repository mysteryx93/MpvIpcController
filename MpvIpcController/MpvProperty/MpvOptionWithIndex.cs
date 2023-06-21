namespace HanumanInstitute.MpvIpcController;

public class MpvOptionWithIndex<T> : MpvOptionWith<T>
    where T : struct
{
    public MpvOptionWithIndex(MpvApi api, string name) :
        base(api, name)
    {
    }

    /// <summary>
    /// Sets the option to 'index'.
    /// </summary>
    public Task SetIndexAsync(ApiOptions? options = null) => SetValueAsync("index", options);

    /// <summary>
    /// Gets whether the option is 'index'.
    /// </summary>
    public Task<bool> GetIndexAsync(ApiOptions? options = null) => GetValueAsync("index", options);
}