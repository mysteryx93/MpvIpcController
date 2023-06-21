namespace HanumanInstitute.MpvIpcController;

public class MpvOptionWithInf<T> : MpvOptionWith<T>
    where T : struct
{
    public MpvOptionWithInf(MpvApi api, string name) :
        base(api, name)
    {
    }

    /// <summary>
    /// Sets the option to 'inf'.
    /// </summary>
    public Task SetInfAsync(ApiOptions? options = null) => SetValueAsync("inf", options);

    /// <summary>
    /// Gets whether the option is 'inf'.
    /// </summary>
    public Task<bool> GetInfAsync(ApiOptions? options = null) => GetValueAsync(new[] { "inf", "false" }, options);
}
