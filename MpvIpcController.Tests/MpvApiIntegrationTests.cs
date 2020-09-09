using System;
using System.Threading.Tasks;
using Xunit;
using Xunit.Abstractions;

namespace HanumanInstitute.MpvIpcController.Tests
{
    public class MpvApiIntegrationTests
    {
        private readonly ITestOutputHelper _output;

        public MpvApiIntegrationTests(ITestOutputHelper output)
        {
            _output = output;
        }

        [Fact]
        public async Task GetClientNameAsync_NoArg_ReturnsClientName()
        {
            using var app = await TestIntegrationSetup.CreateAsync();

            var response = await app.Api.GetClientNameAsync();

            Assert.NotEmpty(response);
            await app.LogAndQuitAsync(_output);
        }

        [Fact]
        public async Task GetVersionAsync_NoArg_ReturnsVersion()
        {
            using var app = await TestIntegrationSetup.CreateAsync();

            var response = await app.Api.GetVersionAsync();

            Assert.True(response > 0);
            await app.LogAndQuitAsync(_output);
        }

        [Fact]
        public async Task LoadFile_WithPrefix_ReturnsSuccess()
        {
            using var app = await TestIntegrationSetup.CreateAsync();

            await app.Api.LoadFileAsync(app.SampleClip, options: new MpvCommandOptions().NoOsd());

            await app.LogAndQuitAsync(_output);
        }

        [Fact]
        public async Task InvalidCommand_DoNotWait_DoesNotReceiveError()
        {
            using var app = await TestIntegrationSetup.CreateAsync();

            await app.Controller.SendMessageAsync(options: new MpvCommandOptions().DoNotWait(), "invalidcommand");

            await app.LogAndQuitAsync(_output);
        }
    }
}
