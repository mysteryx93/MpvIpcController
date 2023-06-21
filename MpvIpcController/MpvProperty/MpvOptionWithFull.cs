namespace HanumanInstitute.MpvIpcController;

public class MpvOptionWithFull<T> : MpvOptionWith<T>
    where T : struct
{
    public MpvOptionWithFull(MpvApi api, string name) :
        base(api, name)
    {
    }

    /// <summary>
    /// Sets the option to 'full'.
    /// </summary>
    public Task SetFullAsync(ApiOptions? options = null) => SetValueAsync("full", options);

    /// <summary>
    /// Gets whether the option is 'full'.
    /// </summary>
    public Task<bool> GetFullAsync(ApiOptions? options = null) => GetValueAsync("full", options);
}