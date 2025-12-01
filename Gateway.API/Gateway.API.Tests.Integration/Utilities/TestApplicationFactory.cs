using Gateway.API.Web.Contracts;
using Gateway.API.Web.Services;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Gateway.API.Tests.Integration.Utilities
{
    public class TestApplicationFactory : WebApplicationFactory<Program>
    {
        public string? LastForwardedUrl { get; set; }
        public string? LastForwardedBody { get; set; }
        public HttpMethod? LastForwardedMethod { get; set; }

        protected override void ConfigureWebHost(IWebHostBuilder builder)
        {
            builder.ConfigureAppConfiguration((context, config) =>
            {
                config.AddJsonFile("appsettings.json");
            });

            builder.ConfigureTestServices(services =>
            {
                var proxyRegs = services
                    .Where(s => s.ServiceType == typeof(IProxyService))
                    .ToList();

                foreach (var r in proxyRegs)
                    services.Remove(r);

                services.AddHttpClient<IProxyService, ProxyService>()
                    .ConfigurePrimaryHttpMessageHandler(_ => new FakeHandler(this));
            });
        }
    }
}
