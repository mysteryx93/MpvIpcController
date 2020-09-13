﻿using System;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Threading.Tasks;
using Xunit.Abstractions;

namespace HanumanInstitute.MpvIpcController.Tests
{
    public class TestIntegrationSetup : IDisposable
    {
        private TestIntegrationSetup()
        { }

        public static async Task<TestIntegrationSetup> CreateAsync()
        {
            var result = new TestIntegrationSetup();
            await result.InitConnection();
            return result;
        }

        [NotNull]
        public MpvApi? Api { get; private set; }

        public IMpvController Controller => Api.Controller;

        public const string MpvPath = @"C:\Program Files\Mpv.net\mpvnet.exe";
        public string SampleClip => Path.Combine(Environment.CurrentDirectory, "SampleClip.mp4");

        private int _pipeId;
        private async Task InitConnection()
        {
            var factory = new MpvApiFactory();
            Api = await factory.StartAsync(MpvPath, $"mpvtest{_pipeId++}");
            Api.Controller.LogEnabled = true;
        }

        public async Task LogAndQuitAsync(ITestOutputHelper? output)
        {
            output?.WriteLine(Controller?.Log?.ToString());
            await Api.QuitAsync(options: new MpvCommandOptions() { WaitForResponse = false });
        }


        private bool _disposedValue;
        protected virtual void Dispose(bool disposing)
        {
            if (!_disposedValue)
            {
                if (disposing)
                {
                    Api?.Dispose();
                }
                _disposedValue = true;
            }
        }

        public void Dispose()
        {
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }
    }
}
