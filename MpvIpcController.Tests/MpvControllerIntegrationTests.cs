using System;
using System.Collections.Generic;
using System.Text;
using Xunit;
using Moq;
using System.Threading.Tasks;
using System.IO;

namespace HanumanInstitute.MpvIpcController.Tests
{
    public class MpvControllerIntegrationTests
    {
        private const string MpvPath = @"C:\Program Files\Mpv.net\mpvnet.exe";
        private string SampleClip => Path.Combine(Environment.CurrentDirectory, "SampleClip.mp4");

        private static int s_pipeId;
        private async Task<IMpvController> SetupModel()
        {
            var factory = new MpvControllerFactory();
            return await factory.StartAsync(MpvPath, $"mpvtest{s_pipeId++}");
        }

        private async Task QuitAsync(IMpvController mpv)
        {
            await mpv.SendMessageAsync("quit");
        }

        [Fact]
        public async Task ConnectAsync_NoAction_QuitSucceeds()
        {
            var model = await SetupModel();

            await QuitAsync(model);
        }

        [Fact]
        public async Task ConnectAsync_NoAction_ReceivesEvent()
        {
            var model = await SetupModel();

            var received = new List<MpvEvent>();
            model.EventReceived += (s, e) => received.Add(e.Event);
            await Task.Delay(200);

            Assert.NotEmpty(received);
            await QuitAsync(model);
        }

        [Fact]
        public async Task SendMessage_LoadFile_ReturnsSuccess()
        {
            var model = await SetupModel();

            var response = await model.SendMessageAsync("loadfile", SampleClip);

            Assert.Equal("success", response.Error);
            await QuitAsync(model);
        }
    }
}
