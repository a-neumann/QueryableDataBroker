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
using System.Collections.Generic;
using Newtonsoft.Json;
using QueryableDataBroker.MVCSample.Models;
using System.Linq;

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
			var response = await this.SendRequest(HttpMethod.Get, "/unicorn/canrespond");

			Assert.True(response.StatusCode == System.Net.HttpStatusCode.OK);
		}

		[Fact]
		public async void CanGetByIdList()
		{
			var list = new List<string>() {
				"66fc62cf-27d8-439e-bb6a-e786db9f1a2a",
				"6acc7787-ce29-4f4e-802a-8954ff65bf00",
				"d7f2c96e-f2ec-4155-bb78-e127f213dd6c"
			};
			var response = await this.SendRequest(HttpMethod.Get, "/unicorn/" + String.Join(",", list));

			Assert.True(response.IsSuccessStatusCode);

			string body = await response.Content.ReadAsStringAsync();
			var unicorns = JsonConvert.DeserializeObject<IEnumerable<Unicorn>>(body);

			Assert.NotEmpty(unicorns);

			var unicornWithFirstId = unicorns.FirstOrDefault(u => u.Id.ToString() == list[0]);
			Assert.NotNull(unicornWithFirstId);
		}

		[Fact]
		public async void CanGetByQuery()
		{
			var response = await this.SendRequest(HttpMethod.Get, "/unicorn?hornlength=113~119");

			Assert.True(response.IsSuccessStatusCode);

			string body = await response.Content.ReadAsStringAsync();
			var unicorns = JsonConvert.DeserializeObject<IEnumerable<Unicorn>>(body);

			Assert.NotEmpty(unicorns);

			var unicorn = unicorns.FirstOrDefault(u => u.HornLenght >= 113 && u.HornLenght <= 119);
			Assert.NotNull(unicorn);
		}
	}
}