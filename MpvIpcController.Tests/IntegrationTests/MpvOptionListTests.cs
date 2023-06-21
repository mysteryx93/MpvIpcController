using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xunit;
using Xunit.Abstractions;

namespace HanumanInstitute.MpvIpcController.IntegrationTests;

public class MpvOptionListTests
{
    private readonly ITestOutputHelper _output;
    // private const string Prop = "volume";
    private readonly IEnumerable<string> propList = new[] { "volume", "vid", "aid" };

    public MpvOptionListTests(ITestOutputHelper output)
    {
        _output = output;
    }

    private static object[] GetProp() => new[]
    {
        new[] { "%6%VOLume" },
        new[] { "m\"&\"n" },
        new[] { "漢字" },
        new[] { "Me, Myself & I" },
        new[] { "%1%2%3%4%5" },
        new[] { "[ABC]abc" }
    };

    [Fact]
    public async Task GetAsync_ResetOnNextFile_ReturnsEmptyList()
    {
        using var app = await TestIntegrationSetup.CreateAsync();

        try
        {
            var result = await app.Api.ResetOnNextFile.GetAsync();

            Assert.NotNull(result);
            Assert.Empty(result);
        }
        finally
        {
            await app.LogAndQuitAsync(_output);
        }
    }

    [Theory]
    [MemberData(nameof(GetProp))]
    public async Task SetAsync_SingleValue_HasSingleValue(string prop)
    {
        using var app = await TestIntegrationSetup.CreateAsync();

        try
        {
            await app.Api.ResetOnNextFile.SetAsync(prop);

            var result = await app.Api.ResetOnNextFile.GetAsync();
            Assert.Single(result);
            Assert.Equal(prop, result.First());
        }
        finally
        {
            await app.LogAndQuitAsync(_output);
        }
    }

    [Fact]
    public async Task SetAsync_MultipleValues_HasMultipleValues()
    {
        using var app = await TestIntegrationSetup.CreateAsync();

        try
        {
            await app.Api.ResetOnNextFile.SetAsync(propList);

            var result = await app.Api.ResetOnNextFile.GetAsync();
            Assert.Equal(propList, result);
        }
        finally
        {
            await app.LogAndQuitAsync(_output);
        }
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    public async Task SetAsync_Empty_ListEmpty(string prop)
    {
        using var app = await TestIntegrationSetup.CreateAsync();

        try
        {
            await app.Api.ResetOnNextFile.SetAsync(prop);

            var result = await app.Api.ResetOnNextFile.GetAsync();
            Assert.Empty(result);
        }
        finally
        {
            await app.LogAndQuitAsync(_output);
        }
    }

    [Theory]
    [MemberData(nameof(GetProp))]
    public async Task AddAsync_SingleValue_ReturnsValue(string prop)
    {
        using var app = await TestIntegrationSetup.CreateAsync();

        try
        {
            await app.Api.ResetOnNextFile.AddAsync(prop);

            var result = await app.Api.ResetOnNextFile.GetAsync();
            Assert.Single(result);
            Assert.Equal(prop, result.First());
        }
        finally
        {
            await app.LogAndQuitAsync(_output);
        }
    }

    [Fact]
    public async Task AddAsync_MultipleValues_ReturnsValue()
    {
        using var app = await TestIntegrationSetup.CreateAsync();

        try
        {
            await app.Api.ResetOnNextFile.AddAsync(propList);

            var result = await app.Api.ResetOnNextFile.GetAsync();
            Assert.Equal(propList, result);
        }
        finally
        {
            await app.LogAndQuitAsync(_output);
        }
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    public async Task AddAsync_Empty_ThrowsException(string prop)
    {
        using var app = await TestIntegrationSetup.CreateAsync();

        try
        {
            Task Act() => app.Api.ResetOnNextFile.AddAsync(prop);

            await Assert.ThrowsAnyAsync<ArgumentException>(Act);
        }
        finally
        {
            await app.LogAndQuitAsync(_output);
        }
    }

    [Theory]
    [MemberData(nameof(GetProp))]
    public async Task RemoveAsync_SingleValue_ListEmpty(string prop)
    {
        using var app = await TestIntegrationSetup.CreateAsync();
        await app.Api.ResetOnNextFile.AddAsync(prop);

        try
        {
            await app.Api.ResetOnNextFile.RemoveAsync(prop);

            var result = await app.Api.ResetOnNextFile.GetAsync();
            Assert.Empty(result);
        }
        finally
        {
            await app.LogAndQuitAsync(_output);
        }
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    public async Task RemoveAsync_Empty_ThrowsException(string prop)
    {
        using var app = await TestIntegrationSetup.CreateAsync();

        try
        {
            Task Act() => app.Api.ResetOnNextFile.RemoveAsync(prop);

            await Assert.ThrowsAnyAsync<ArgumentException>(Act);
        }
        finally
        {
            await app.LogAndQuitAsync(_output);
        }
    }

    [Theory]
    [InlineData("volume")]
    public async Task ClearAsync_SingleValue_ListEmpty(string prop)
    {
        using var app = await TestIntegrationSetup.CreateAsync();
        await app.Api.ResetOnNextFile.AddAsync(prop);

        try
        {
            await app.Api.ResetOnNextFile.ClearAsync();

            var result = await app.Api.ResetOnNextFile.GetAsync();
            Assert.Empty(result);
        }
        finally
        {
            await app.LogAndQuitAsync(_output);
        }
    }
}