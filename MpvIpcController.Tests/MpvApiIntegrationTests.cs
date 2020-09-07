using System;
using System.Threading.Tasks;
using Xunit;
using Xunit.Abstractions;

namespace HanumanInstitute.MpvIpcController.Tests
{
    public class MpvApiIntegrationTests
    {
        private readonly ITestOutputHelper _output;
        private readonly TestIntegrationSetup _app = new TestIntegrationSetup();

        public MpvApiIntegrationTests(ITestOutputHelper output)
        {
            _output = output;
        }

        [Fact]
        public async Task SendMessage_ClientName_ReturnsClientName()
        {
            var model = await _app.SetupModel();

            var response = await model.GetClientName();

            await ValidateAsync(response, response.Length > 0, model);
        }

        [Fact]
        public async Task SendMessage_GetVersion_ReturnsVersion()
        {
            var model = await _app.SetupModel();

            var response = await model.GetVersion();

            await ValidateAsync(response, response > 0, model);
        }

        private async Task ValidateAsync(object response, bool valid, IMpvApi model)
        {
            _output.WriteLine(response.ToString());
            Assert.True(valid);
            await _app.QuitAsync(model);
        }
    }
}
