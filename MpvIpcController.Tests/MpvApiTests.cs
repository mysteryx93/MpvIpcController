using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Xunit;
using Xunit.Abstractions;

namespace HanumanInstitute.MpvIpcController.Tests
{
    public class MpvApiTests
    {
        //private readonly ITestOutputHelper _output;

        //public MpvApiTests(ITestOutputHelper output)
        //{
        //    _output = output;
        //}

        [Fact]
        public async Task RequestLogMessages_ValidValue_ServerReceivesMessage()
        {
            using var app = TestSetup.Create();

            await app.Api.RequestLogMessagesAsync(LogLevel.Fatal, new ApiOptions() { WaitForResponse = false });

            Assert.Contains("fatal", app.ServerLog.ToString(), StringComparison.InvariantCulture);
        }

        [Fact]
        public async Task RequestLogMessages_InvalidValue_ServerReceivesMessage()
        {
            using var app = TestSetup.Create();

            var act = app!.Api.RequestLogMessagesAsync((LogLevel)9999, new ApiOptions() { WaitForResponse = false });

            await Assert.ThrowsAsync<ArgumentException>(() => act);
        }
    }
}
