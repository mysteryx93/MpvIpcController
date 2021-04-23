using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;
using Xunit.Abstractions;

namespace HanumanInstitute.MpvIpcController.IntegrationTests
{
    public class MpvOptionDictionaryTests
    {
        private readonly ITestOutputHelper _output;

        public MpvOptionDictionaryTests(ITestOutputHelper output)
        {
            _output = output;
        }

        private static object[] GetKeyValues() => new[]
{
            new[] { "key1", "value1" },
            new[] { "m\"&\"n", "漢字" },
            new[] { "漢字", null },
            new[] { "Me, Myself & I", "-=<->=-" },
            new[] { "%1%2%3%4%5", "%1%2%3%4%5" },
            new[] { "[ABC]abc", "Val[ABC]abc" }
        };

        private static IDictionary<string, string> GetKeyValuesList()
        {
            var result = new Dictionary<string, string>();
            foreach (IList<string> item in GetKeyValues())
            {
                result.Add(item[0], item[1]);
            }
            return result;
        }

        [Fact]
        public async Task GetAsync_YouTubeDlRawOptions_ReturnsEmptyList()
        {
            using var app = await TestIntegrationSetup.CreateAsync();

            try
            {
                var result = await app.Api.YouTubeDlRawOptions.GetAsync();

                Assert.NotNull(result);
                Assert.Empty(result);
            }
            finally
            {
                await app.LogAndQuitAsync(_output);
            }
        }

        [Theory]
        [MemberData(nameof(GetKeyValues))]
        public async Task SetAsync_SingleValue_HasSingleValue(string key, string value)
        {
            using var app = await TestIntegrationSetup.CreateAsync();

            try
            {
                await app.Api.YouTubeDlRawOptions.AddAsync(key, value);

                var result = await app.Api.YouTubeDlRawOptions.GetAsync();

                Assert.Single(result);
                Assert.Equal(new[] { new KeyValuePair<string, string>(key, value ?? "") }, result);
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
            var values = GetKeyValuesList();

            try
            {
                await app.Api.YouTubeDlRawOptions.SetAsync(values);

                var result = await app.Api.YouTubeDlRawOptions.GetAsync();
                Assert.Equal(values.ToList(), result?.ToList());
            }
            finally
            {
                await app.LogAndQuitAsync(_output);
            }
        }

        [Theory]
        [MemberData(nameof(GetKeyValues))]
        public async Task AddAsync_SingleValue_ReturnsValue(string key, string value)
        {
            using var app = await TestIntegrationSetup.CreateAsync();

            try
            {
                await app.Api.YouTubeDlRawOptions.AddAsync(key, value);

                var result = await app.Api.YouTubeDlRawOptions.GetAsync();
                Assert.Single(result);
                Assert.Equal(new[] { new KeyValuePair<string, string>(key, value ?? "") }, result);
            }
            finally
            {
                await app.LogAndQuitAsync(_output);
            }
        }

        //[Fact]
        //public async Task AddAsync_MultipleValues_ReturnsValue()
        //{
        //    using var app = await TestIntegrationSetup.CreateAsync();

        //    try
        //    {
        //        await app.Api.ResetOnNextFile.AddAsync(PropMulti);

        //        var result = await app.Api.ResetOnNextFile.GetAsync();
        //        Assert.Equal(propList, result);
        //    }
        //    finally
        //    {
        //        await app.LogAndQuitAsync(_output);
        //    }
        //}

        [Theory]
        [MemberData(nameof(GetKeyValues))]
        public async Task RemoveAsync_SingleValue_ListEmpty(string key, string value)
        {
            using var app = await TestIntegrationSetup.CreateAsync();
            await app.Api.YouTubeDlRawOptions.AddAsync(key, value);

            try
            {
                await app.Api.YouTubeDlRawOptions.RemoveAsync(key);

                var result = await app.Api.YouTubeDlRawOptions.GetAsync();
                Assert.Empty(result);
            }
            finally
            {
                await app.LogAndQuitAsync(_output);
            }
        }
    }
}
