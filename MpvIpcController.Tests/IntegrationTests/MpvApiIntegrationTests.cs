using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Xunit;
using Xunit.Abstractions;

namespace HanumanInstitute.MpvIpcController.IntegrationTests;

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

        Assert.NotEmpty(response?.Data);
        await app.LogAndQuitAsync(_output);
    }

    [Fact]
    public async Task GetVersionAsync_NoArg_ReturnsVersion()
    {
        using var app = await TestIntegrationSetup.CreateAsync();

        var response = await app.Api.GetVersionAsync();

        Assert.True(response?.Data > 0);
        await app.LogAndQuitAsync(_output);
    }

    [Fact]
    public async Task LoadFile_WithPrefix_ReturnsSuccess()
    {
        using var app = await TestIntegrationSetup.CreateAsync();

        await app.Api.LoadFileAsync(app.SampleClip, options: new ApiOptions() { NoOsd = true });

        await app.LogAndQuitAsync(_output);
    }

    [Fact]
    public async Task InvalidCommand_DoNotWait_DoesNotReceiveError()
    {
        using var app = await TestIntegrationSetup.CreateAsync();

        await app.Controller.SendMessageAsync(options: new ApiOptions() { WaitForResponse = false }, "invalidcommand");

        await app.LogAndQuitAsync(_output);
    }

    [Fact]
    public async Task IdleActive_Idle_ReturnsTrue()
    {
        using var app = await TestIntegrationSetup.CreateAsync();

        try
        {
            var result = await app.Api.IdleActive.GetAsync();

            Assert.True(result);
        }
        finally
        {
            await app.LogAndQuitAsync(_output);
        }
    }

    [Fact]
    public async Task Metadata_ValidFile_ReturnsDictionary()
    {
        using var app = await TestIntegrationSetup.CreateAsync();

        try
        {
            await app.Api.LoadFileAsync(app.SampleClip);
            var result = await app.Api.Metadata.Metadata.GetAsync();

            Assert.NotEmpty(result);
        }
        finally
        {
            await app.LogAndQuitAsync(_output);
        }
    }

    [Fact]
    public async Task DemuxerCacheTime_ValidFile_ReturnsValue()
    {
        using var app = await TestIntegrationSetup.CreateAsync();

        try
        {
            await app.Api.LoadFileAsync(app.SampleClip);
            var result = await app.Api.DemuxerCacheTime.GetAsync();

            Assert.NotNull(result);
        }
        finally
        {
            await app.LogAndQuitAsync(_output);
        }
    }

    [Fact]
    public async Task DemuxerCacheState_ValidFile_ReturnsParsedObject()
    {
        using var app = await TestIntegrationSetup.CreateAsync();

        try
        {
            await app.Api.LoadFileAsync(app.SampleClip);
            var result = await app.Api.DemuxerCacheState.GetAsync();

            Assert.NotNull(result);
            Assert.NotEqual(0, result?.RawInputRate);
        }
        finally
        {
            await app.LogAndQuitAsync(_output);
        }
    }

    [Fact]
    public async Task VideoParamsRotate_ValidFile_ReturnsValue()
    {
        using var app = await TestIntegrationSetup.CreateAsync();

        try
        {
            await app.Api.LoadFileAsync(app.SampleClip);
            await Task.Delay(50);
            var result = await app.Api.VideoParams.Rotate.GetAsync();

            Assert.NotNull(result);
        }
        finally
        {
            await app.LogAndQuitAsync(_output);
        }
    }

    [Fact]
    public async Task Z_RunCommand_ReturnsValue()
    {
        using var app = await TestIntegrationSetup.CreateAsync();
        // app.Controller.DefaultOptions.ResponseTimeout = -1;

        try
        {
            //await app.Api.LoadFileAsync(app.SampleClip);
            //await Task.Delay(100);
            var result = await app.Api.VideoSyncMaxVideoChange.GetAsync();
            //await app.Api.TerminalMsgLevel.SetAsync(new Dictionary<string, string>() { { "all", "warn" } });
            //var result = await app.Api.TerminalMsgLevel.GetAsync();
            //await Task.Delay(100);

            // Assert.NotNull(result);
        }
        finally
        {
            await app.LogAndQuitAsync(_output);
        }
    }
}