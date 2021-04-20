using System;
using System.Threading.Tasks;

namespace HanumanInstitute.MpvIpcController
{
    public class MpvOptionAutoNo<T> : MpvOptionAuto<T>
        where T : struct
    {
        public MpvOptionAutoNo(MpvApi api, string name) :
            base(api, name)
        {
        }

        /// <summary>
        /// Sets the option to 'no'.
        /// </summary>
        public Task SetNoAsync(ApiOptions? options = null) => Api.SetPropertyAsync(PropertyName, "no", options);

        /// <summary>
        /// Gets whether the option is 'no'.
        /// </summary>
        public async Task<bool> GetNoAxync(ApiOptions? options = null)
        {
            var result = await Api.GetPropertyAsync(PropertyName, options).ConfigureAwait(false);
            return result != null && result.HasValue && result.Value() == "no";
        }
    }

    public class MpvOptionAutoNoRef<T> : MpvOptionAutoRef<T>
        where T : class
    {
        public MpvOptionAutoNoRef(MpvApi api, string name) :
            base(api, name)
        {
        }

        /// <summary>
        /// Sets the option to 'no'.
        /// </summary>
        public Task SetNoAsync(ApiOptions? options = null) => Api.SetPropertyAsync(PropertyName, "no", options);

        /// <summary>
        /// Gets whether the option is 'no'.
        /// </summary>
        public async Task<bool> GetNoAxync(ApiOptions? options = null)
        {
            var result = await Api.GetPropertyAsync(PropertyName, options).ConfigureAwait(false);
            return result != null && result.HasValue && result.Value() == "no";
        }
    }
}
