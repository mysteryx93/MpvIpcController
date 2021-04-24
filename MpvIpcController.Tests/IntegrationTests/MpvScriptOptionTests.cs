using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
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

        private static object[] GetValues() => new[]
        {
            new[] { "%6%VOLume" },
            new[] { "m\"&\"n" },
            new[] { "漢字" },
            new[] { "Me, Myself & I" },
            new[] { "%1%2%3%4%5" },
            new[] { "[ABC]abc" }
        };

        [Fact]
        public async Task GetAsync_YouTubeDlTryFirst_ReturnsNull()
        {
            using var app = await TestIntegrationSetup.CreateAsync();

            try
            {
                var result = await app.Api.YouTubeDlTryFirst.GetAsync();

                Assert.Null(result);
            }
            finally
            {
                await app.LogAndQuitAsync(_output);
            }
        }

        [Theory]
        [MemberData(nameof(GetValues))]
        public async Task SetAsync_YouTubeDlTryFirst_ReturnsValue(string value)
        {
            using var app = await TestIntegrationSetup.CreateAsync();

            try
            {
                await app.Api.YouTubeDlTryFirst.SetAsync(value);

                var result = await app.Api.YouTubeDlTryFirst.GetAsync();
                Assert.Equal(value, result);
            }
            finally
            {
                await app.LogAndQuitAsync(_output);
            }
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        public async Task SetAsync_Empty_ReturnsValue(string value)
        {
            using var app = await TestIntegrationSetup.CreateAsync();

            try
            {
                await app.Api.YouTubeDlTryFirst.SetAsync(value);

                var result = await app.Api.YouTubeDlTryFirst.GetAsync();
                Assert.Null(result);
            }
            finally
            {
                await app.LogAndQuitAsync(_output);
            }
        }

        [Theory]
        [MemberData(nameof(GetValues))]
        public async Task RemoveAsync_Value_ValueReturnsNull(string value)
        {
            using var app = await TestIntegrationSetup.CreateAsync();

            try
            {
                await app.Api.YouTubeDlTryFirst.SetAsync(value);
                await app.Api.YouTubeDlTryFirst.RemoveAsync();

                var result = await app.Api.YouTubeDlTryFirst.GetAsync();
                Assert.Null(result);
            }
            finally
            {
                await app.LogAndQuitAsync(_output);
            }
        }
    }
}
