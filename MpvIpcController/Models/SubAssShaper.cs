using System;

namespace HanumanInstitute.MpvIpcController
{
    /// <summary>
    /// Set the text layout engine used by libass.
    /// </summary>
    public enum SubAssShaper
    {
        /// <summary>
        /// Uses Fribidi only, fast, doesn't render some languages correctly.
        /// </summary>
        Simple,
        /// <summary>
        /// Uses HarfBuzz, slower, wider language support.
        /// </summary>
        Complex
    }
}
