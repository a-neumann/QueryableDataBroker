using Microsoft.AspNet.TestHost;
using System.Net.Http;
using Xunit;
using Microsoft.Extensions.PlatformAbstractions;
using Microsoft.Extensions.DependencyInjection;
using System.IO;
using QueryableDataBroker.MVCSample;
using System;
using System.Runtime.Versioning;
using System.Threading.Tasks;

namespace QueryableDataBroker.MVCSample.Tests
{
	internal class TestApplicationEnvironment : IApplicationEnvironment
	{
		public string ApplicationBasePath { get; set; }
		public string ApplicationName { get; set; }
		public string ApplicationVersion => PlatformServices.Default.Application.ApplicationVersion;
		public string Configuration => PlatformServices.Default.Application.Configuration;

		public FrameworkName RuntimeFramework
		{
			get
			{
				return PlatformServices.Default.Application.RuntimeFramework;
			}
		}

		public object GetData(string name)
		{
			return PlatformServices.Default.Application.GetData(name);
		}

		public void SetData(string name, object value)
		{
			PlatformServices.Default.Application.SetData(name, value);
		}
	}

	public class HttpTests
	{
		public HttpTests()
		{
			var builder = TestServer.CreateBuilder()
			.UseEnvironment("Development")
			.UseServices(services =>
			{
				// Change the application environment to the mvc project
				var env = new TestApplicationEnvironment();
				//env.ApplicationBasePath = Path.GetFullPath(Path.Combine(PlatformServices.Default.Application.ApplicationBasePath, "..", "..", "src", "MvcSite"));
				env.ApplicationBasePath = Path.GetFullPath(PlatformServices.Default.Application.ApplicationBasePath);
				env.ApplicationName = "QueryableDataBroker.MVCSample";

				services.AddInstance<IApplicationEnvironment>(env);
			})
			.UseStartup<Startup>();

			this.TestServer = new TestServer(builder);
		}

		public TestServer TestServer { get; set; }

		public async Task<HttpResponseMessage> SendRequest(HttpMethod method, string uri)
		{
			var client = this.TestServer.CreateClient();
			var request = new HttpRequestMessage(method, uri);
			return await client.SendAsync(request);
		}
		
		[Fact]
		public async void CanConnect()
		{
			var result = await this.SendRequest(HttpMethod.Get, "/unicorn/canrespond");

			Assert.True(result.StatusCode == System.Net.HttpStatusCode.OK);
		}
	}
}