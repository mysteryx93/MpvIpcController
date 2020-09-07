using System;
using System.IO;
using System.Threading.Tasks;
using HanumanInstitute.Validators;

namespace HanumanInstitute.MpvIpcController.Tests
{
    public class TestIntegrationSetup
    {
        public const string MpvPath = @"C:\Program Files\Mpv.net\mpvnet.exe";
        public string SampleClip => Path.Combine(Environment.CurrentDirectory, "SampleClip.mp4");

        private int _pipeId;
        public async Task<IMpvApi> SetupModel()
        {
            var factory = new MpvApiFactory();
            return await factory.StartAsync(MpvPath, $"mpvtest{_pipeId++}");
        }

        public async Task QuitAsync(IMpvController mpv)
        {
            mpv.CheckNotNull(nameof(mpv));

            await mpv.SendMessageAsync("quit");
            await Task.Delay(100);
        }
    }
}
