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
            app.Model.WaitForResponse = false;

            await app.Model.RequestLogMessages(LogLevel.Fatal);

            Assert.Contains("fatal", app.ServerLog.ToString(), StringComparison.InvariantCulture);
        }

        [Fact]
        public async Task RequestLogMessages_InvalidValue_ServerReceivesMessage()
        {
            using var app = TestSetup.Create();
            app.Model.WaitForResponse = false;

            var act = app!.Model.RequestLogMessages((LogLevel)9999);

            await Assert.ThrowsAsync<ArgumentException>(() => act);
        }
    }
}
