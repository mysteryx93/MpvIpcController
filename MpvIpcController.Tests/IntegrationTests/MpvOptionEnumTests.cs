using System;
using System.Threading.Tasks;
using Xunit;
using Xunit.Abstractions;

namespace HanumanInstitute.MpvIpcController.IntegrationTests;

public class MpvOptionEnumTests
{
    private readonly ITestOutputHelper _output;

    public MpvOptionEnumTests(ITestOutputHelper output)
    {
        _output = output;
    }

    [Fact]
    public async Task ObserveAsync_HrSeekOption_RaisesPropertyChanged()
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
            await app.Api.HrSeek.ObserveAsync(observeId);
            await app.Api.HrSeek.SetAsync(HrSeekOption.Absolute);

            Assert.Equal(app.Api.HrSeek.PropertyName, changedName);
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
        await app.Api.HrSeek.ObserveAsync(observeId);

        try
        {
            await app.Api.UnobservePropertyAsync(observeId);
            await Task.Delay(100);

            app.Api.PropertyChanged += (s, e) =>
            {
                changedName = e.Name;
            };
            await app.Api.HrSeek.SetAsync(HrSeekOption.Absolute);

            Assert.Null(changedName);
        }
        finally
        {
            await app.LogAndQuitAsync(_output);
        }
    }

    [Fact]
    public async Task GetAsync_HrSeek_ReturnsNull()
    {
        using var app = await TestIntegrationSetup.CreateAsync();

        try
        {
            var result = await app.Api.HrSeek.GetAsync();

            Assert.Equal(HrSeekOption.Default, result);
        }
        finally
        {
            await app.LogAndQuitAsync(_output);
        }
    }


    [Theory]
    [InlineData(HrSeekOption.Default)]
    [InlineData(HrSeekOption.Absolute)]
    [InlineData(HrSeekOption.Yes)]
    [InlineData(HrSeekOption.No)]
    public async Task SetAsync_HrSeek_HasNewValue(HrSeekOption value)
    {
        using var app = await TestIntegrationSetup.CreateAsync();

        try
        {
            await app.Api.HrSeek.SetAsync(value);

            var result = await app.Api.HrSeek.GetAsync();
            Assert.Equal(value, result);
        }
        finally
        {
            await app.LogAndQuitAsync(_output);
        }
    }

    [Fact]
    public async Task AddAsync_HrSeek_ThrowsException()
    {
        using var app = await TestIntegrationSetup.CreateAsync();

        try
        {
            Task Act() => app.Api.HrSeek.AddAsync(HrSeekOption.Yes);

            await Assert.ThrowsAsync<NotImplementedException>(Act);
        }
        finally
        {
            await app.LogAndQuitAsync(_output);
        }
    }

    [Fact]
    public async Task MultiplyAsync_HrSeek_ThrowsException()
    {
        using var app = await TestIntegrationSetup.CreateAsync();

        try
        {
            Task Act() => app.Api.HrSeek.MultiplyAsync(1);

            await Assert.ThrowsAsync<NotImplementedException>(Act);
        }
        finally
        {
            await app.LogAndQuitAsync(_output);
        }
    }

    [Fact]
    public async Task CycleAsync_HrSeekUp_HasGreaterValue()
    {
        using var app = await TestIntegrationSetup.CreateAsync();

        try
        {
            await app.Api.HrSeek.CycleAsync();

            var result = await app.Api.HrSeek.GetAsync();
            Assert.NotEqual(HrSeekOption.Default, result);
        }
        finally
        {
            await app.LogAndQuitAsync(_output);
        }
    }

    [Fact]
    public async Task CycleAsync_HrSeekDown_HasLowerValue()
    {
        using var app = await TestIntegrationSetup.CreateAsync();

        try
        {
            await app.Api.HrSeek.CycleAsync(CycleDirection.Down);

            var result = await app.Api.HrSeek.GetAsync();
            Assert.NotEqual(HrSeekOption.Default, result);
        }
        finally
        {
            await app.LogAndQuitAsync(_output);
        }
    }
}