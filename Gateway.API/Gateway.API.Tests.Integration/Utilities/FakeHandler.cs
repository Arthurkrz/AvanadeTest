
using System.Net;
using System.Text;

namespace Gateway.API.Tests.Integration.Utilities
{
    public class FakeHandler : HttpMessageHandler
    {
        private readonly TestApplicationFactory _factory;

        public FakeHandler(TestApplicationFactory factory)
        {
            _factory = factory;
        }

        protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            _factory.LastForwardedUrl = request.RequestUri!.ToString();
            _factory.LastForwardedMethod = request.Method;

            if (request.Content != null)
            {
                _factory.LastForwardedBody = request.Content
                    .ReadAsStringAsync().Result;
            }

            return Task.FromResult(new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = new StringContent("{\"ok\":true}")
            });
        }
    }
}
