using System;
using System.Threading.Tasks;
using Xunit;
using Xunit.Abstractions;

namespace HanumanInstitute.MpvIpcController.IntegrationTests;

public class MpvOptionTests
{
    private readonly ITestOutputHelper _output;

    public MpvOptionTests(ITestOutputHelper output)
    {
        _output = output;
    }

    [Fact]
    public async Task ObserveAsync_VideoIdProperty_RaisesPropertyChanged()
    {
        using var app = await TestIntegrationSetup.CreateAsync();
        var observeId = 1;
        string? changedName = null;

        try
        {
            app.Api.PropertyChanged += (s, e) =>
            {
                changedName = e.Name;
            };
            await app.Api.VideoId.ObserveAsync(observeId);
            await app.LoadVideoAsync();

            Assert.Equal(app.Api.VideoId.PropertyName, changedName);
        }
        finally
        {
            await app.LogAndQuitAsync(_output);
        }
    }

    [Fact]
    public async Task UnobserveAsync_VideoIdProperty_DoesNotRaisePropertyChanged()
    {
        using var app = await TestIntegrationSetup.CreateAsync();
        var observeId = 1;
        string? changedName = null;
        await app.Api.VideoId.ObserveAsync(observeId);

        try
        {
            app.Api.PropertyChanged += (s, e) =>
            {
                changedName = e.Name;
            };
            await app.Api.UnobservePropertyAsync(observeId);
            await Task.Delay(100);
            await app.LoadVideoAsync();

            Assert.Null(changedName);
        }
        finally
        {
            await app.LogAndQuitAsync(_output);
        }
    }

    [Fact]
    public async Task GetAsync_Volume_ReturnsValue()
    {
        using var app = await TestIntegrationSetup.CreateAsync();

        try
        {
            var result = await app.Api.Volume.GetAsync();

            Assert.True(result > 0);
        }
        finally
        {
            await app.LogAndQuitAsync(_output);
        }
    }

    [Fact]
    public async Task SetAsync_Volume_HasNewValue()
    {
        using var app = await TestIntegrationSetup.CreateAsync();
        var volume = 100;

        try
        {
            await app.Api.Volume.SetAsync(volume);

            var result = await app.Api.Volume.GetAsync();
            Assert.Equal(volume, result);
        }
        finally
        {
            await app.LogAndQuitAsync(_output);
        }
    }

    [Fact]
    public async Task AddAsync_Volume_HasAddedValue()
    {
        using var app = await TestIntegrationSetup.CreateAsync();
        var volume = 50;
        var volumeAdd = 10;
        await app.Api.Volume.SetAsync(volume);

        try
        {
            await app.Api.Volume.AddAsync(volumeAdd);

            var result = await app.Api.Volume.GetAsync();
            Assert.Equal(volume + volumeAdd, result);
        }
        finally
        {
            await app.LogAndQuitAsync(_output);
        }
    }

    [Fact]
    public async Task MultiplyAsync_Volume_HasMultipliedValue()
    {
        using var app = await TestIntegrationSetup.CreateAsync();
        var volume = 50;
        var volumeMul = 1.1;
        await app.Api.Volume.SetAsync(volume);

        try
        {
            await app.Api.Volume.MultiplyAsync(volumeMul);

            var result = await app.Api.Volume.GetAsync();
            Assert.Equal((int)(volume * volumeMul), result);
        }
        finally
        {
            await app.LogAndQuitAsync(_output);
        }
    }

    [Fact]
    public async Task CycleAsync_VolumeUp_HasGreaterValue()
    {
        using var app = await TestIntegrationSetup.CreateAsync();
        var volume = 50;
        await app.Api.Volume.SetAsync(volume);

        try
        {
            await app.Api.Volume.CycleAsync();

            var result = await app.Api.Volume.GetAsync();
            Assert.True(result > volume);
        }
        finally
        {
            await app.LogAndQuitAsync(_output);
        }
    }

    [Fact]
    public async Task CycleAsync_VolumeDown_HasLowerValue()
    {
        using var app = await TestIntegrationSetup.CreateAsync();
        var volume = 50;
        await app.Api.Volume.SetAsync(volume);

        try
        {
            await app.Api.Volume.CycleAsync(CycleDirection.Down);

            var result = await app.Api.Volume.GetAsync();
            Assert.True(result < volume);
        }
        finally
        {
            await app.LogAndQuitAsync(_output);
        }
    }
}