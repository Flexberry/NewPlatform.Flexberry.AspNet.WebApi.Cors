namespace NewPlatform.Flexberry.AspNet.WebApi.Cors.Tests
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Net.Http;
    using System.Web.Cors;
    using System.Web.Http;
    using System.Web.Http.SelfHost;
    using Xunit;

    public class DynamicCorsPolicyProviderTest
    {
        /// <summary>
        /// Test default behavior.
        /// </summary>
        [Fact]
        public void TestOrigin()
        {
            // Arrange.
            using var config = new HttpSelfHostConfiguration(new Uri("http://localhost:9898/"));
            using var server = new HttpSelfHostServer(config);
            using var client = new HttpClient(server, false) { BaseAddress = new Uri("http://localhost:8989/") };
            config.EnableCors(new DynamicCorsPolicyProvider());

            string requestOrigin = $"http://localhost:{new Random().Next(1000, 9999)}/";
            client.DefaultRequestHeaders.Add("Origin", requestOrigin);

            try
            {
                server.OpenAsync().Wait();

                // Act.
                var message = client.GetAsync(new Uri("http://localhost:8989")).Result;

                // Assert.
                IEnumerable<string> origins;
                Assert.True(message.Headers.TryGetValues("Access-Control-Allow-Origin", out origins));
                Assert.Equal(requestOrigin, origins.First());
            }
            finally
            {
                server.CloseAsync().Wait();
            }
        }

        /// <summary>
        /// Test the constructor with enabled <see cref="CorsPolicy.SupportsCredentials"/>.
        /// </summary>
        [Fact]
        public void TestSupportsCredentials()
        {
            // Arrange.
            using var config = new HttpSelfHostConfiguration(new Uri("http://localhost:6500/"));
            using var server = new HttpSelfHostServer(config);
            using var client = new HttpClient(server, false);
            config.EnableCors(new DynamicCorsPolicyProvider(true));

            client.DefaultRequestHeaders.Add("Origin", "http://localhost:4200/");

            try
            {
                server.OpenAsync().Wait();

                // Act.
                var message = client.GetAsync(new Uri("http://localhost:6500")).Result;

                // Assert.
                IEnumerable<string> credentials;
                Assert.True(message.Headers.TryGetValues("Access-Control-Allow-Credentials", out credentials));
                Assert.Equal("true", credentials.First());
            }
            finally
            {
                server.CloseAsync().Wait();
            }
        }

        /// <summary>
        /// Test the constructor with not enabled <see cref="CorsPolicy.SupportsCredentials"/>.
        /// </summary>
        [Fact]
        public void TestUnsupportedCredentials()
        {
            // Arrange.
            using var config = new HttpSelfHostConfiguration(new Uri("http://localhost:6500/"));
            using var server = new HttpSelfHostServer(config);
            using var client = new HttpClient(server, false);
            config.EnableCors(new DynamicCorsPolicyProvider());

            client.DefaultRequestHeaders.Add("Origin", "http://localhost:4200/");

            try
            {
                server.OpenAsync().Wait();

                // Act.
                var message = client.GetAsync(new Uri("http://localhost:6500")).Result;

                // Assert.
                IEnumerable<string> credentials;
                Assert.False(message.Headers.TryGetValues("Access-Control-Allow-Credentials", out credentials));
            }
            finally
            {
                server.CloseAsync().Wait();
            }
        }
    }
}
