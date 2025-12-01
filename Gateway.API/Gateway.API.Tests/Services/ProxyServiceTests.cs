using Gateway.API.Tests.Utilities;
using Gateway.API.Web.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using System.Net;
using System.Text;

namespace Gateway.API.Tests.Services
{
    public class ProxyServiceTests
    {
        private static ProxyService CreateProxy(HttpMessageHandler handler, Dictionary<string, string?>? configValues = null)
        {
            var httpClient = new HttpClient(handler);

            var config = new ConfigurationBuilder()
                .AddInMemoryCollection(configValues ?? new Dictionary<string, string?>())
                .Build();

            return new ProxyService(httpClient, config);
        }

        [Fact]
        public async Task ForwardAsync_ShouldForwardToCorrectUrl()
        {
            // Arrange
            string? actualUrl = null;

            var handler = new FakeHttpMessageHandler(req =>
            {
                actualUrl = req.RequestUri!.ToString();
                return new HttpResponseMessage(HttpStatusCode.OK)
                {
                    Content = new StringContent("OK")
                };
            });

            var proxy = CreateProxy(handler, new()
            {
                { "Services:Stock", "http://localhost:7151" }
            });

            var context = new DefaultHttpContext();
            context.Request.Method = "GET";
            context.Request.Path = "/api/product/10";

            // Act
            var result = await proxy.ForwardAsync(context, "Stock");

            // Assert
            Assert.Equal("http://localhost:7151/api/product/10", actualUrl);
        }

        [Fact]
        public async Task ForwardAsync_ShouldOverridePath_WhenProvided()
        {
            // Arrange
            string? actualUrl = null;

            var handler = new FakeHttpMessageHandler(req =>
            {
                actualUrl = req.RequestUri!.ToString();
                return new HttpResponseMessage(HttpStatusCode.OK)
                {
                    Content = new StringContent("OK")
                };
            });

            var proxy = CreateProxy(handler, new()
            {
                { "Services:Identity", "http://localhost:7084" }
            });

            var context = new DefaultHttpContext();
            context.Request.Method = "POST";

            // Act
            await proxy.ForwardAsync(context, "Identity", "/api/admin/login");

            // Assert
            Assert.Equal("http://localhost:7084/api/admin/login", actualUrl);
        }

        [Fact]
        public async Task ForwardAsync_ShouldForwardHttpMethod()
        {
            // Arrange
            HttpMethod? actualMethod = null;

            var handler = new FakeHttpMessageHandler(req =>
            {
                actualMethod = req.Method;
                return new HttpResponseMessage(HttpStatusCode.OK);
            });

            var proxy = CreateProxy(handler, new()
            {
                { "Services:Sales", "http://localhost:7120" }
            });

            var context = new DefaultHttpContext();
            context.Request.Method = "PUT";
            context.Request.Path = "/api/sales/5";

            // Act
            await proxy.ForwardAsync(context, "Sales");

            // Assert
            Assert.Equal(HttpMethod.Put, actualMethod);
        }

        [Fact]
        public async Task ForwardAsync_ShouldForwardBody()
        {
            // Arrange
            string? actualBody = null;

            var handler = new FakeHttpMessageHandler(req =>
            {
                actualBody = req.Content!.ReadAsStringAsync().Result;
                return new HttpResponseMessage(HttpStatusCode.OK);
            });

            var proxy = CreateProxy(handler, new()
            {
                { "Services:Stock", "http://localhost:7151" }
            });

            var context = new DefaultHttpContext();
            context.Request.Method = "POST";
            context.Request.Path = "/api/product/create";

            var json = "{\"name\":\"Test Product\"}";
            var bodyBytes = Encoding.UTF8.GetBytes(json);
            context.Request.Body = new MemoryStream(bodyBytes);
            context.Request.ContentLength = bodyBytes.Length;
            context.Request.ContentType = "application/json";

            // Act
            await proxy.ForwardAsync(context, "Stock");

            // Assert
            Assert.Equal(json, actualBody);
        }

        [Fact]
        public async Task ForwardAsync_ShouldForwardHeaders()
        {
            // Arrange
            string? actualHeader = null;

            var handler = new FakeHttpMessageHandler(req =>
            {
                actualHeader = req.Headers.Authorization?.ToString();
                return new HttpResponseMessage(HttpStatusCode.OK);
            });

            var proxy = CreateProxy(handler, new()
            {
                { "Services:Stock", "http://localhost:7151" }
            });

            var context = new DefaultHttpContext();
            context.Request.Method = "GET";
            context.Request.Path = "/api/product/all";

            context.Request.Headers["Authorization"] = "Bearer token";

            // Act
            await proxy.ForwardAsync(context, "Stock");

            // Assert
            Assert.Equal("Bearer token", actualHeader);
        }

        [Fact]
        public async Task ForwardAsync_ShouldReturnResponseContent()
        {
            // Arrange
            var handler = new FakeHttpMessageHandler(req =>
            {
                return new HttpResponseMessage(HttpStatusCode.OK)
                {
                    Content = new StringContent("{\"success\":true}", Encoding.UTF8, "application.json")
                };
            });

            var proxy = CreateProxy(handler, new()
            {
                { "Services:Sales", "http://localhost:7120" }
            });

            var context = new DefaultHttpContext();
            context.Request.Method = "GET";
            context.Request.Path = "/api/sales/all";

            // Act
            var result = await proxy.ForwardAsync(context, "Sales");

            // Assert
            var asContent = Assert.IsType<ContentResult>(result);
            Assert.Equal("{\"success\":true}", asContent.Content);
            Assert.Equal("application/json", asContent.ContentType);
        }

        [Fact]
        public async Task ForwardAsync_ShouldThrowException_WhenServiceNotConfigured()
        {
            // Arrange
            var handler = new FakeHttpMessageHandler(req =>
                new HttpResponseMessage(HttpStatusCode.OK));

            var proxy = CreateProxy(handler);

            var context = new DefaultHttpContext();
            context.Request.Method = "GET";
            context.Request.Path = "/api/test";

            // Act & Assert
            await Assert.ThrowsAsync<Exception>(async () =>
                await proxy.ForwardAsync(context, "UnknownService"));
        }
    }
}