namespace HanumanInstitute.MpvIpcController;

public class MpvOptionWithAutoNo<T> : MpvOptionWithAuto<T>
    where T : struct
{
    public MpvOptionWithAutoNo(MpvApi api, string name) :
        base(api, name)
    {
    }

    /// <summary>
    /// Sets the option to 'no'.
    /// </summary>
    public Task SetNoAsync(ApiOptions? options = null) => SetValueAsync("no", options);

    /// <summary>
    /// Gets whether the option is 'no'.
    /// </summary>
    public Task<bool> GetNoAsync(ApiOptions? options = null) => GetValueAsync(new[] { "no", "false" }, options);
}