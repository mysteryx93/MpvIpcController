using System;
using System.Collections.Generic;
using System.Text;
using Xunit;
using Xunit.Abstractions;

namespace HanumanInstitute.MpvIpcController.IntegrationTests
{
    public class MpvScriptOptionTests
    {
        private readonly ITestOutputHelper _output;

        public MpvScriptOptionTests(ITestOutputHelper output)
        {
            _output = output;
        }

        //[Fact]
        //public async Task GetClientNameAsync_NoArg_ReturnsClientName()
        //{
        //    using var app = await TestIntegrationSetup.CreateAsync();

        //    var response = await app.Api.VideoLanguage.GetAsync

        //    Assert.NotEmpty(response?.Data);
        //    await app.LogAndQuitAsync(_output);
        //}
    }
}
