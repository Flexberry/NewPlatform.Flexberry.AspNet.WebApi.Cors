namespace NewPlatform.Flexberry.AspNet.WebApi.Cors.Tests
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Net.Http;
    using System.Web.Http;
    using System.Web.Http.SelfHost;

    using Microsoft.VisualStudio.TestTools.UnitTesting;

    [TestClass]
    public class DynamicCorsPolicyProviderTest
    {
        /// <summary>
        /// Test default behavior.
        /// </summary>
        [TestMethod]
        public void TestOrigin()
        {
            // Arrange.
            using (var config = new HttpSelfHostConfiguration(new Uri("http://localhost:1234/")))
            using (var server = new HttpSelfHostServer(config))
            using (var client = new HttpClient(server, false) { BaseAddress = new Uri("http://localhost:8989/") })
            {
                config.EnableCors(new DynamicCorsPolicyProvider());

                var requestOrigin = $"http://localhost:{new Random().Next(1000, 9999)}/";
                client.DefaultRequestHeaders.Add("Origin", requestOrigin);

                try
                {
                    server.OpenAsync().Wait();

                    // Act.
                    var message = client.GetAsync(new Uri("http://localhost:8989")).Result;

                    // Assert.
                    IEnumerable<string> origins;
                    Assert.IsTrue(message.Headers.TryGetValues("Access-Control-Allow-Origin", out origins));
                    Assert.AreEqual(requestOrigin, origins.First());
                }
                finally
                {
                    server.CloseAsync().Wait();
                }
            }
        }

        /// <summary>
        /// Test the constructor with enabled <see cref="CorsPolicy.SupportsCredentials"/>.
        /// </summary>
        [TestMethod]
        public void TestSupportsCredentials()
        {
            // Arrange.
            using (var config = new HttpSelfHostConfiguration(new Uri("http://localhost:6500/")))
            using (var server = new HttpSelfHostServer(config))
            using (var client = new HttpClient(server, false))
            {
                config.EnableCors(new DynamicCorsPolicyProvider(true));

                client.DefaultRequestHeaders.Add("Origin", "http://localhost:4200/");

                try
                {
                    server.OpenAsync().Wait();

                    // Act.
                    var message = client.GetAsync(new Uri("http://localhost:6500")).Result;

                    // Assert.
                    IEnumerable<string> credentials;
                    Assert.IsTrue(message.Headers.TryGetValues("Access-Control-Allow-Credentials", out credentials));
                    Assert.AreEqual("true", credentials.First());
                }
                finally
                {
                    server.CloseAsync().Wait();
                }
            }
        }

        /// <summary>
        /// Test the constructor with not enabled <see cref="CorsPolicy.SupportsCredentials"/>.
        /// </summary>
        [TestMethod]
        public void TestUnsupportedCredentials()
        {
            // Arrange.
            using (var config = new HttpSelfHostConfiguration(new Uri("http://localhost:6500/")))
            using (var server = new HttpSelfHostServer(config))
            using (var client = new HttpClient(server, false))
            {
                config.EnableCors(new DynamicCorsPolicyProvider());

                client.DefaultRequestHeaders.Add("Origin", "http://localhost:4200/");

                try
                {
                    server.OpenAsync().Wait();

                    // Act.
                    var message = client.GetAsync(new Uri("http://localhost:6500")).Result;

                    // Assert.
                    IEnumerable<string> credentials;
                    Assert.IsFalse(message.Headers.TryGetValues("Access-Control-Allow-Credentials", out credentials));
                }
                finally
                {
                    server.CloseAsync().Wait();
                }
            }
        }
    }
}
