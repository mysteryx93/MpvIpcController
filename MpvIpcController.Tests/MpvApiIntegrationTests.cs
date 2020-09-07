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
        public async Task SendMessage_ClientName_ReturnsClientName()
        {
            using var app = await TestIntegrationSetup.CreateAsync();

            var response = await app.Api.GetClientName();

            await ValidateAsync(response, response.Length > 0, app);
        }

        [Fact]
        public async Task SendMessage_GetVersion_ReturnsVersion()
        {
            using var app = await TestIntegrationSetup.CreateAsync();

            var response = await app.Api.GetVersion();

            await ValidateAsync(response, response > 0, app);
        }

        private async Task ValidateAsync(object response, bool valid, TestIntegrationSetup app)
        {
            _output.WriteLine(response.ToString());
            Assert.True(valid);
            await app.QuitAsync();
        }
    }
}
