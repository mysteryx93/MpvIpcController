namespace HanumanInstitute.MpvIpcController;

public class MpvOptionWithDefault<T> : MpvOptionWith<T>
    where T : struct
{
    public MpvOptionWithDefault(MpvApi api, string name) :
        base(api, name)
    {
    }

    /// <summary>
    /// Sets the option to 'default'.
    /// </summary>
    public Task SetDefaultAsync(ApiOptions? options = null) => SetValueAsync("default", options);

    /// <summary>
    /// Gets whether the option is 'default'.
    /// </summary>
    public Task<bool> GetDefaultAsync(ApiOptions? options = null) => GetValueAsync("default", options);
}