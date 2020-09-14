using System;
using System.Threading.Tasks;

namespace HanumanInstitute.MpvIpcController
{
    public class MpvOptionAuto<T> : MpvOption<T>
    {
        public MpvOptionAuto(MpvApi api, string name, CustomParser<T>? parser = null, CustomFormatter<T>? formatter = null) :
            base(api, name, parser ?? MpvFormatters.ParseDefaultNull<T>, formatter)
        {
        }

        /// <summary>
        /// Sets the option to 'auto'.
        /// </summary>
        public Task SetAutoAsync(ApiOptions? options = null) => Api.SetPropertyAsync(PropertyName, "auto", options);
    }
}
