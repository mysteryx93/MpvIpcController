using System;

namespace HanumanInstitute.MpvIpcController
{
    /// <summary>
    /// Exposes VideoParams sub-properties.
    /// </summary>
    public class VideoProperties
    {
        private readonly MpvApi _api;
        private readonly string _prefix;

        public VideoProperties(MpvApi api, string propertyName)
        {
            _api = api;
            _prefix = propertyName;
        }

        /// <summary>
        /// The pixel format as string. This uses the same names as used in other places of mpv.
        /// </summary>
        public MpvPropertyRead<string?> PixelFormat => _pixelFormat ??= new MpvPropertyRead<string?>(_api, _prefix + "/pixelformat");
        private MpvPropertyRead<string?>? _pixelFormat;

        /// <summary>
        /// Average bits-per-pixel as integer. Subsampled planar formats use a different resolution, which is the reason this value can sometimes be odd or confusing. Can be unavailable with some formats.
        /// </summary>
        public MpvPropertyRead<int?> AverageBitPerPixel => _averageBitPerPixel ??= new MpvPropertyRead<int?>(_api, _prefix + "/average-bpp");
        private MpvPropertyRead<int?>? _averageBitPerPixel;

        /// <summary>
        /// Bit depth for each color component as integer. This is only exposed for planar or single-component formats, and is unavailable for other formats.
        /// </summary>
        public MpvPropertyRead<int?> PlaneDepth => _planeDepth ??= new MpvPropertyRead<int?>(_api, _prefix + "/plane-depth");
        private MpvPropertyRead<int?>? _planeDepth;

        /// <summary>
        /// Video width, with no aspect correction applied.
        /// </summary>
        public MpvPropertyRead<int?> Width => _width ??= new MpvPropertyRead<int?>(_api, _prefix + "/w");
        private MpvPropertyRead<int?>? _width;

        /// <summary>
        /// Video height, with no aspect correction applied.
        /// </summary>
        public MpvPropertyRead<int?> Height => _height ??= new MpvPropertyRead<int?>(_api, _prefix + "/h");
        private MpvPropertyRead<int?>? _height;

        /// <summary>
        /// Video width, scaled for correct aspect ratio.
        /// </summary>
        public MpvPropertyRead<int?> DisplayWidth => _displayWidth ??= new MpvPropertyRead<int?>(_api, _prefix + "/dw");
        private MpvPropertyRead<int?>? _displayWidth;

        /// <summary>
        /// Video height, scaled for correct aspect ratio.
        /// </summary>
        public MpvPropertyRead<int?> DisplayHeight => _displayHeight ??= new MpvPropertyRead<int?>(_api, _prefix + "/dh");
        private MpvPropertyRead<int?>? _displayHeight;

        /// <summary>
        /// Display aspect ratio.
        /// </summary>
        public MpvPropertyRead<float?> AspectRatio => _aspectRatio ??= new MpvPropertyRead<float?>(_api, _prefix + "/aspect");
        private MpvPropertyRead<float?>? _aspectRatio;

        /// <summary>
        /// Pixel aspect ratio.
        /// </summary>
        public MpvPropertyRead<string?> PixelAspectRatio => _pixelAspectRatio ??= new MpvPropertyRead<string?>(_api, _prefix + "/par");
        private MpvPropertyRead<string?>? _pixelAspectRatio;

        /// <summary>
        /// The colormatrix in use as string. (Exact values subject to change.)
        /// </summary>
        public MpvPropertyRead<string?> ColorMatrix => _colorMatrix ??= new MpvPropertyRead<string?>(_api, _prefix + "/colormatrix");
        private MpvPropertyRead<string?>? _colorMatrix;

        /// <summary>
        /// The colorlevels as string. (Exact values subject to change.)
        /// </summary>
        public MpvPropertyRead<string?> ColorLevels => _colorLevels ??= new MpvPropertyRead<string?>(_api, _prefix + "/colorlevels");
        private MpvPropertyRead<string?>? _colorLevels;

        /// <summary>
        /// The primaries in use as string. (Exact values subject to change.)
        /// </summary>
        public MpvPropertyRead<string?> Primaries => _primaries ??= new MpvPropertyRead<string?>(_api, _prefix + "/primaries");
        private MpvPropertyRead<string?>? _primaries;

        /// <summary>
        /// The gamma function in use as string. (Exact values subject to change.)
        /// </summary>
        public MpvPropertyRead<string?> Gamma => _gamma ??= new MpvPropertyRead<string?>(_api, _prefix + "/gamma");
        private MpvPropertyRead<string?>? _gamma;

        /// <summary>
        /// The video file's tagged signal peak as float.
        /// </summary>
        public MpvPropertyRead<float?> SignalPeak => _signalPeak ??= new MpvPropertyRead<float?>(_api, _prefix + "/sig-peak");
        private MpvPropertyRead<float?>? _signalPeak;

        /// <summary>
        /// The light type in use as a string. (Exact values subject to change.)
        /// </summary>
        public MpvPropertyRead<string?> Light => _light ??= new MpvPropertyRead<string?>(_api, _prefix + "/light");
        private MpvPropertyRead<string?>? _light;

        /// <summary>
        /// Chroma location as string. (Exact values subject to change.)
        /// </summary>
        public MpvPropertyRead<string?> ChromaLocation => _chromaLocation ??= new MpvPropertyRead<string?>(_api, _prefix + "/chroma-location");
        private MpvPropertyRead<string?>? _chromaLocation;

        /// <summary>
        /// Intended display rotation in degrees (clockwise).
        /// </summary>
        public MpvPropertyRead<int?> Rotate => _rotate ??= new MpvPropertyRead<int?>(_api, _prefix + "/rotate");
        private MpvPropertyRead<int?>? _rotate;

        /// <summary>
        /// Source file stereo 3D mode. (See the format video filter's stereo-in option.)
        /// </summary>
        public MpvPropertyRead<string?> StereoIn => _stereoIn ??= new MpvPropertyRead<string?>(_api, _prefix + "/stereo-in");
        private MpvPropertyRead<string?>? _stereoIn;



        /// <summary>
        /// Number of audio channels.
        /// </summary>
        public MpvPropertyRead<int?> ChannelCount => _channelCount ??= new MpvPropertyRead<int?>(_api, _prefix + "/channel-count");
        private MpvPropertyRead<int?>? _channelCount;
    }
}
