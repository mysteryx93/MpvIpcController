using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using HanumanInstitute.Validators;
using Xunit;
using Xunit.Abstractions;

namespace HanumanInstitute.MpvIpcController.IntegrationTests
{
    public class MpvControllerIntegrationTests
    {
        private readonly ITestOutputHelper _output;

        public MpvControllerIntegrationTests(ITestOutputHelper output)
        {
            _output = output;
        }

        [Fact]
        public async Task ConnectAsync_NoAction_QuitSucceeds()
        {
            using var app = await TestIntegrationSetup.CreateAsync();

            await app.LogAndQuitAsync(_output);
        }

        [Fact]
        public async Task ConnectAsync_LoadFile_ReceivesEvent()
        {
            using var app = await TestIntegrationSetup.CreateAsync();

            var received = new List<string>();
            app.Controller.EventReceived += (s, e) => received.Add(e.EventName);
            await app.Controller.SendMessageAsync(null, "loadfile", app.SampleClip);
            await Task.Delay(10);

            Assert.NotEmpty(received);
            await app.LogAndQuitAsync(_output);
        }

        [Fact]
        public async Task SendMessage_LoadFile_ReturnsSuccess()
        {
            using var app = await TestIntegrationSetup.CreateAsync();

            await app.Controller.SendMessageAsync(null, "loadfile", app.SampleClip);

            await app.LogAndQuitAsync(_output);
        }

        [Fact]
        public async Task SendMessage_ConcurrentRequests_ReceivesAllResponses()
        {
            using var app = await TestIntegrationSetup.CreateAsync();
            app.Controller.LogEnabled = true;
            app.Controller.DefaultOptions.ResponseTimeout = 5000;
            const int Concurrent = 20;
            var requests = new int[Concurrent];
            for (var i = 0; i < Concurrent; i++)
            {
                requests[i] = i;
            }

            try
            {
                var tasksClient = requests.ForEachOrderedAsync(x => app.Controller.SendMessageAsync(null, "client_name"));
                await tasksClient.ConfigureAwait(false);
            }
            finally
            {
                _output.WriteLine(app.Controller?.Log?.ToString());
            }

            // Success: No freeze and no crash.
            await app.LogAndQuitAsync(_output);
        }
    }
}
