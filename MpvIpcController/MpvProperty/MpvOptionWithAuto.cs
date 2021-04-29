using System;
using System.Threading.Tasks;

namespace HanumanInstitute.MpvIpcController
{
    public class MpvOptionWithAuto<T> : MpvOptionWith<T>
        where T : struct
    {
        public MpvOptionWithAuto(MpvApi api, string name) :
            base(api, name)
        {
        }

        /// <summary>
        /// Sets the option to 'auto'.
        /// </summary>
        public Task SetAutoAsync(ApiOptions? options = null) => SetValueAsync("auto", options);

        /// <summary>
        /// Gets whether the option is 'auto'.
        /// </summary>
        public Task<bool> GetAutoAsync(ApiOptions? options = null) => GetValueAsync("auto", options);
    }
}
