using System;
using System.Threading.Tasks;

namespace HanumanInstitute.MpvIpcController
{
    public class MpvOptionWithNoAlways<T> : MpvOptionWithNo<T>
        where T : struct
    {
        public MpvOptionWithNoAlways(MpvApi api, string name) :
            base(api, name)
        {
        }

        /// <summary>
        /// Sets the option to 'always'.
        /// </summary>
        public Task SetAlwaysAsync(ApiOptions? options = null) => SetValueAsync("always", options);

        /// <summary>
        /// Gets whether the option is 'always'.
        /// </summary>
        public Task<bool> GetAlwaysAsync(ApiOptions? options = null) => GetValueAsync(new[] { "always", "true" }, options);
    }
}
