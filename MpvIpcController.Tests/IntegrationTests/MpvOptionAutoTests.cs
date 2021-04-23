using System;
using System.Threading.Tasks;
using Xunit;
using Xunit.Abstractions;

namespace HanumanInstitute.MpvIpcController.IntegrationTests
{
    public class MpvOptionAutoTests
    {
        private readonly ITestOutputHelper _output;

        public MpvOptionAutoTests(ITestOutputHelper output)
        {
            _output = output;
        }

        [Fact]
        public async Task GetAsync_VideoId_ReturnsFalse()
        {
            using var app = await TestIntegrationSetup.CreateAsync();

            try
            {
                await app.LoadVideoAsync();
                var response = await app.Api.VideoId.GetAsync();

                Assert.Equal(1, response);
            }
            finally
            {
                await app.LogAndQuitAsync(_output);
            }
        }

        [Fact]
        public async Task GetAutoAsync_VideoId_ReturnsFalse()
        {
            using var app = await TestIntegrationSetup.CreateAsync();

            try
            {
                await app.Api.VideoId.SetAsync(0);
                var response = await app.Api.VideoId.GetAutoAsync();

                Assert.False(response);
            }
            finally
            {
                await app.LogAndQuitAsync(_output);
            }
        }

        [Fact]
        public async Task GetNoAsync_VideoId_ReturnsFalse()
        {
            using var app = await TestIntegrationSetup.CreateAsync();

            try
            {
                await app.Api.VideoId.SetAsync(0);
                var response = await app.Api.VideoId.GetNoAsync();

                Assert.False(response);
            }
            finally
            {
                await app.LogAndQuitAsync(_output);
            }
        }

        [Fact]
        public async Task SetAutoAsync_VideoId_VideoIdSet()
        {
            using var app = await TestIntegrationSetup.CreateAsync();

            try
            {
                await app.Api.VideoId.SetAutoAsync();

                var result = await app.Api.VideoId.GetAutoAsync();
                Assert.True(result);
            }
            finally
            {
                await app.LogAndQuitAsync(_output);
            }
        }

        [Fact]
        public async Task SetNoAsync_VideoId_VideoIdSet()
        {
            using var app = await TestIntegrationSetup.CreateAsync();

            try
            {
                await app.Api.VideoId.SetNoAsync();

                var result = await app.Api.VideoId.GetNoAsync();
                Assert.True(result);
            }
            finally
            {
                await app.LogAndQuitAsync(_output);
            }
        }

        [Fact]
        public async Task GetAsync_ValueAuto_ReturnsNull()
        {
            using var app = await TestIntegrationSetup.CreateAsync();

            try
            {
                await app.Api.VideoId.SetAutoAsync();

                var result = await app.Api.VideoId.GetAsync();
                Assert.Null(result);
            }
            finally
            {
                await app.LogAndQuitAsync(_output);
            }
        }

        [Fact]
        public async Task GetAsync_ValueNo_ReturnsNull()
        {
            using var app = await TestIntegrationSetup.CreateAsync();

            try
            {
                await app.Api.VideoId.SetNoAsync();

                var result = await app.Api.VideoId.GetAsync();
                Assert.Null(result);
            }
            finally
            {
                await app.LogAndQuitAsync(_output);
            }
        }
    }
}
