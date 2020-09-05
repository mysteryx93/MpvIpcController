using System;
using System.Collections.Generic;
using System.Text;
using Xunit;
using Moq;
using System.Threading.Tasks;
using System.IO;
using Xunit.Abstractions;

namespace HanumanInstitute.MpvIpcController.Tests
{
    public class MpvControllerIntegrationTests
    {
        private readonly ITestOutputHelper _output;

        public MpvControllerIntegrationTests(ITestOutputHelper output)
        {
            _output = output;
        }

        private const string MpvPath = @"C:\Program Files\Mpv.net\mpvnet.exe";
        private string SampleClip => Path.Combine(Environment.CurrentDirectory, "SampleClip.mp4");

        private static int s_pipeId;
        private async Task<IMpvApi> SetupModel()
        {
            var factory = new MpvApiFactory();
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

            await model.SendMessageAsync("loadfile", SampleClip);

            await QuitAsync(model);
        }

        [Fact]
        public async Task SendMessage_ClientName_ReturnsClientName()
        {
            var model = await SetupModel();

            var response = await model.GetClientName();

            Assert.NotEmpty(response);
            await QuitAsync(model);
        }

        [Fact]
        public async Task SendMessage_ConcurrentRequests_ReceivesAllResponses()
        {
            var model = await SetupModel();
            model.LogEnabled = true;
            model.ResponseTimeout = -1;
            const int Concurrent = 10;
            var requests = new int[Concurrent];
            for (var i = 0; i < Concurrent; i++)
            {
                requests[i] = i;
            }

            try
            {
                var tasksClient = requests.AsyncParallelForEach(x => model.SendMessageAsync("client_name"));
                await tasksClient.ConfigureAwait(false);
            }
            finally
            {
                _output.WriteLine(model?.Log?.ToString());
            }

            // Success: No freeze and no crash.
            await QuitAsync(model);
        }
    }
}
