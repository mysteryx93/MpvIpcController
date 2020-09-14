using System;
using System.Diagnostics.CodeAnalysis;
using System.Threading.Tasks;

namespace HanumanInstitute.MpvIpcController
{
    public class MpvOptionAutoNo<T> : MpvOptionAuto<T>
    {
        public MpvOptionAutoNo(MpvApi api, string name, CustomParser<T>? parser = null, CustomFormatter<T>? formatter = null) :
            base(api, name, parser ?? MpvFormatters.ParseDefaultNull<T>, formatter)
        {
        }

        /// <summary>
        /// Sets the option to 'no'.
        /// </summary>
        public Task SetNoAsync(ApiOptions? options = null) => Api.SetPropertyAsync(PropertyName, "no", options);
    }
}
