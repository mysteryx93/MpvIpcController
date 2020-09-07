using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using HanumanInstitute.Validators;
using Xunit;
using Xunit.Abstractions;

namespace HanumanInstitute.MpvIpcController.Tests
{
    public class MpvControllerIntegrationTests
    {
        private readonly ITestOutputHelper _output;
        private readonly TestIntegrationSetup _app = new TestIntegrationSetup();

        public MpvControllerIntegrationTests(ITestOutputHelper output)
        {
            _output = output;
        }

        [Fact]
        public async Task ConnectAsync_NoAction_QuitSucceeds()
        {
            var model = await _app.SetupModel();

            await _app.QuitAsync(model);
        }

        [Fact]
        public async Task ConnectAsync_LoadFile_ReceivesEvent()
        {
            var model = await _app.SetupModel();

            var received = new List<MpvEvent>();
            model.EventReceived += (s, e) => received.Add(e.Event);
            await model.SendMessageAsync("loadfile", _app.SampleClip);
            await Task.Delay(10);

            Assert.NotEmpty(received);
            await _app.QuitAsync(model);
        }

        [Fact]
        public async Task SendMessage_LoadFile_ReturnsSuccess()
        {
            var model = await _app.SetupModel();

            await model.SendMessageAsync("loadfile", _app.SampleClip);

            await _app.QuitAsync(model);
        }

        [Fact]
        public async Task SendMessage_ConcurrentRequests_ReceivesAllResponses()
        {
            var model = await _app.SetupModel();
            model.LogEnabled = true;
            model.ResponseTimeout = 5000;
            const int Concurrent = 20;
            var requests = new int[Concurrent];
            for (var i = 0; i < Concurrent; i++)
            {
                requests[i] = i;
            }

            try
            {
                var tasksClient = requests.ForEachOrderedAsync(x => model.SendMessageAsync("client_name"));
                await tasksClient.ConfigureAwait(false);
            }
            finally
            {
                _output.WriteLine(model?.Log?.ToString());
            }

            // Success: No freeze and no crash.
            await _app.QuitAsync(model);
        }
    }
}
