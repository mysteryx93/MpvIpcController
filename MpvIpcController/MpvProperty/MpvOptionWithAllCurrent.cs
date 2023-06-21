namespace HanumanInstitute.MpvIpcController;

public class MpvOptionWithAllCurrent<T> : MpvOptionWith<T>
    where T : struct
{
    public MpvOptionWithAllCurrent(MpvApi api, string name) :
        base(api, name)
    {
    }

    /// <summary>
    /// Sets the option to 'all'.
    /// </summary>
    public Task SetAllAsync(ApiOptions? options = null) => SetValueAsync("all", options);

    /// <summary>
    /// Gets whether the option is 'all'.
    /// </summary>
    public Task<bool> GetAllAsync(ApiOptions? options = null) => GetValueAsync("all", options);

    /// <summary>
    /// Sets the option to 'current'.
    /// </summary>
    public Task SetCurrentAsync(ApiOptions? options = null) => SetValueAsync("current", options);

    /// <summary>
    /// Gets whether the option is 'current'.
    /// </summary>
    public Task<bool> GetCurrentAsync(ApiOptions? options = null) => GetValueAsync("current", options);
}