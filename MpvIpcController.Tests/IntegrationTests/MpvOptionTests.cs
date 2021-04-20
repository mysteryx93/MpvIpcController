using System;
using System.Threading.Tasks;
using Xunit;
using Xunit.Abstractions;

namespace HanumanInstitute.MpvIpcController.IntegrationTests
{
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
                await app.Api.LoadFileAsync(app.SampleClip);
                await Task.Delay(100);

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
                await app.Api.LoadFileAsync(app.SampleClip);
                await Task.Delay(100);

                Assert.Null(changedName);
            }
            finally
            {
                await app.LogAndQuitAsync(_output);
            }
        }

        [Fact]
        public async Task GetAsync_AudioVolume_ReturnsValue()
        {
            using var app = await TestIntegrationSetup.CreateAsync();

            try
            {
                var result = await app.Api.AudioVolume.GetAsync();

                Assert.True(result > 0);
            }
            finally
            {
                await app.LogAndQuitAsync(_output);
            }
        }


        [Fact]
        public async Task SetAsync_AudioVolume_HasNewValue()
        {
            using var app = await TestIntegrationSetup.CreateAsync();
            var volume = .5;

            try
            {
                await app.Api.AudioVolume.SetAsync(volume);

                var result = await app.Api.AudioVolume.GetAsync();
                Assert.Equal(volume, result);
            }
            finally
            {
                await app.LogAndQuitAsync(_output);
            }
        }

        [Fact]
        public async Task AddAsync_AudioVolume_HasAddedValue()
        {
            using var app = await TestIntegrationSetup.CreateAsync();
            var volume = .5;
            var volumeAdd = .1;
            await app.Api.AudioVolume.SetAsync(volume);

            try
            {
                await app.Api.AudioVolume.AddAsync(volumeAdd);

                var result = await app.Api.AudioVolume.GetAsync();
                Assert.Equal(volume + volumeAdd, result);
            }
            finally
            {
                await app.LogAndQuitAsync(_output);
            }
        }

        [Fact]
        public async Task MultiplyAsync_AudioVolume_HasMultipliedValue()
        {
            using var app = await TestIntegrationSetup.CreateAsync();
            var volume = .5;
            var volumeMul = 1.1;
            await app.Api.AudioVolume.SetAsync(volume);

            try
            {
                await app.Api.AudioVolume.MultiplyAsync(volumeMul);

                var result = await app.Api.AudioVolume.GetAsync();
                Assert.Equal(volume * volumeMul, result);
            }
            finally
            {
                await app.LogAndQuitAsync(_output);
            }
        }

        [Fact]
        public async Task CycleAsync_AudioVolumeUp_HasGreaterValue()
        {
            using var app = await TestIntegrationSetup.CreateAsync();
            var volume = .5;
            await app.Api.AudioVolume.SetAsync(volume);

            try
            {
                await app.Api.AudioVolume.CycleAsync();

                var result = await app.Api.AudioVolume.GetAsync();
                Assert.True(result > volume);
            }
            finally
            {
                await app.LogAndQuitAsync(_output);
            }
        }

        [Fact]
        public async Task CycleAsync_AudioVolumeDown_HasLowerValue()
        {
            using var app = await TestIntegrationSetup.CreateAsync();
            var volume = .5;
            await app.Api.AudioVolume.SetAsync(volume);

            try
            {
                await app.Api.AudioVolume.CycleAsync(CycleDirection.Down);

                var result = await app.Api.AudioVolume.GetAsync();
                Assert.True(result < volume);
            }
            finally
            {
                await app.LogAndQuitAsync(_output);
            }
        }
    }
}
