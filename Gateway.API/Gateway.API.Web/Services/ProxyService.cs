using Gateway.API.Web.Contracts;
using Microsoft.AspNetCore.Mvc;
using System.Text;

namespace Gateway.API.Web.Services
{
    public class ProxyService : IProxyService
    {
        private readonly HttpClient _httpClient;
        private readonly IConfiguration _config;

        public ProxyService(HttpClient httpClient, IConfiguration config)
        {
            _httpClient = httpClient;
            _config = config;
        }

        public async Task<IActionResult> ForwardAsync(HttpContext context, string targetService, string? overridePath = null)
        {
            var requestMessage = new HttpRequestMessage
                { Method = new HttpMethod(context.Request.Method) };

            var baseUrl = _config[$"Services:{targetService}"];
            var path = overridePath ?? context.Request.Path.ToString();
            var targetUrl = $"{baseUrl}{path}{context.Request.QueryString}";

            requestMessage.RequestUri = new Uri(targetUrl);

            if (context.Request.ContentLength > 0)
            {
                context.Request.EnableBuffering();
                
                using var reader = new StreamReader(context.Request.Body, leaveOpen: true);
                var bodyString = await reader.ReadToEndAsync();

                context.Request.Body.Position = 0;

                requestMessage.Content = new StringContent(
                    bodyString,
                    Encoding.UTF8,
                    context.Request.ContentType!
                );
            }

            foreach (var header in context.Request.Headers)
            {
                if (!requestMessage.Headers.TryAddWithoutValidation(header.Key, header.Value.ToArray()))
                    requestMessage.Content?.Headers.TryAddWithoutValidation(header.Key, header.Value.ToArray());
            }

            var response = await _httpClient.SendAsync(requestMessage);
            var responseBody = await response.Content.ReadAsStringAsync();

            return new ContentResult
            {
                StatusCode = (int)response.StatusCode,
                Content = responseBody,
                ContentType = response.Content.Headers.ContentType?.ToString()
            };
        }
    }
}
