using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using Xunit;
using Xunit.Abstractions;
using HanumanInstitute.Validators;

namespace HanumanInstitute.MpvIpcController.Tests
{
    public class MpvControllerTests
    {
        private readonly ITestOutputHelper _output;

        public MpvControllerTests(ITestOutputHelper output)
        {
            _output = output;
        }

        private const string CommandName = "loadfile";
        private string GetResponseSimple(int requestId = 1) => $@"{{ ""data"": null, ""error"": ""success"", ""request_id"": {requestId} }}";
        private string GetResponseWithInt(int requestId = 1) => $@"{{ ""data"": 15, ""error"": ""success"", ""request_id"": {requestId} }}";
        private string GetResponseWithArray(int requestId = 1) => $@"{{ ""data"": [""a"", ""b""], ""error"": ""success"", ""request_id"": {requestId} }}";
        private const string EventMessage = @"{ ""event"": ""property-change"", ""id"": 1, ""data"": ""52.000000"", ""name"": ""volume"" }";

        [Fact]
        public async Task EventReceived_Triggered_ContainsData()
        {
            using var app = TestSetup.Create();

            IDictionary<string, object?>? data = null;
            app.Controller.EventReceived += (s, e) =>
            {
                data = e.Event.Data;
            };
            await app.WriteServerMessageAsync(EventMessage);
            await Task.Delay(50);

            Assert.NotNull(data);
            Assert.NotEmpty(data);
        }

        [Fact]
        public async Task SendMessage_CommandSimple_ReceivesResponse()
        {
            using var app = TestSetup.Create();

            var requestTask = app.Controller.SendMessageAsync(null, CommandName);
            await app.WriteServerMessageAsync(GetResponseSimple());
            var response = await requestTask;

            Assert.Null(response);
        }

        [Fact]
        public async Task SendMessage_CommandWithParams_ReceivesResponseWithInt()
        {
            using var app = TestSetup.Create();

            var requestTask = app.Controller.SendMessageAsync(null, CommandName);
            await app.WriteServerMessageAsync(GetResponseWithInt());
            var response = await requestTask;

            Assert.IsType<int>(response);
        }

        [Fact]
        public async Task SendMessage_CommandWithParams_ReceivesResponseWithArray()
        {
            using var app = TestSetup.Create();

            var requestTask = app.Controller.SendMessageAsync(null, CommandName);
            await app.WriteServerMessageAsync(GetResponseWithArray());
            var response = await requestTask;

            Assert.NotEmpty((IEnumerable?)response);
        }

        [Fact]
        public async Task SendMessage_CommandWithNullParams_IgnoreNullParams()
        {
            using var app = TestSetup.Create();

            var requestTask = app.Controller.SendMessageAsync(null, CommandName, null, null);
            await app.WriteServerMessageAsync(GetResponseWithArray());
            await requestTask;

            Assert.DoesNotContain("null", app.ServerLog.ToString(), StringComparison.InvariantCultureIgnoreCase);
        }

        [Theory]
        [InlineData("abc")]
        public async Task SendMessage_CommandWithParamAndNull_ContainsParamButNotNull(string param)
        {
            using var app = TestSetup.Create();

            var requestTask = app.Controller.SendMessageAsync(null, CommandName, param, null);
            await app.WriteServerMessageAsync(GetResponseWithArray());
            await requestTask;

            var log = app.ServerLog.ToString();
            Assert.Contains(param, log, StringComparison.InvariantCulture);
            Assert.DoesNotContain("null", log, StringComparison.InvariantCultureIgnoreCase);
        }

        [Theory]
        [InlineData("abc")]
        public async Task SendMessage_CommandWithNullParamNull_ContainsNullAndParam(string param)
        {
            using var app = TestSetup.Create();

            var requestTask = app.Controller.SendMessageAsync(null, CommandName, null, param, null);
            await app.WriteServerMessageAsync(GetResponseWithArray());
            await requestTask;

            var log = app.ServerLog.ToString();
            Assert.Contains(param, log, StringComparison.InvariantCulture);
            Assert.Contains("null", log, StringComparison.InvariantCultureIgnoreCase);
        }

        [Theory]
        [InlineData("abc")]
        [InlineData("ABC")]
        [InlineData(1555)]
        [InlineData(-1555)]
        [InlineData(1.555)]
        [InlineData(-1.555)]
        public async Task SendMessage_CommandWithParam_ServerReceivesParam(object param)
        {
            using var app = TestSetup.Create();

            var requestTask = app.Controller.SendMessageAsync(null, CommandName, param);
            await app.WriteServerMessageAsync(GetResponseWithArray());
            await requestTask;

            Assert.Contains(param?.ToString(), app.ServerLog.ToString(), StringComparison.InvariantCulture);
        }

        [Fact]
        public async Task SendMessage_TimeoutNegative_DoesNotTimeout()
        {
            using var app = TestSetup.Create();
            app.Controller.ResponseTimeout = -1;

            var requestTask = app.Controller.SendMessageAsync(null, CommandName);
            await Task.Delay(50);
            await app.WriteServerMessageAsync(GetResponseSimple());
            await requestTask;
        }

        [Fact]
        public async Task SendMessage_TimeoutZero_ThrowsException()
        {
            using var app = TestSetup.Create();
            app.Controller.ResponseTimeout = 0;

            async Task Act()
            {
                var requestTask = app.Controller.SendMessageAsync(null, CommandName);
                await Task.Delay(50);
                await app.WriteServerMessageAsync(GetResponseSimple());
                var response = await requestTask;
            }

            await Assert.ThrowsAsync<TimeoutException>(Act);
        }

        [Fact]
        public async Task SendMessage_ConcurrentRequests_ReceivesAllResponses()
        {
            using var app = TestSetup.Create();
            app.Controller.LogEnabled = true;
            const int Concurrent = 10;
            var requests = new int[Concurrent];
            for (var i = 0; i < Concurrent; i++)
            {
                requests[i] = i + 1;
            }

            try
            {
                var tasksClient = requests.ForEachAsync(x => app.Controller.SendMessageAsync(null, CommandName));
                await Task.Delay(10);
                var tasksServer = requests.ForEachAsync(x => app.WriteServerMessageAsync(GetResponseSimple(x)));
                await Task.Delay(100);
                await Task.WhenAll(tasksClient, tasksServer).ConfigureAwait(false);
            }
            finally
            {
                _output.WriteLine(app.Controller?.Log?.ToString());
            }

            // Success: No freeze and no crash.
        }
    }
}
