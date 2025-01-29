
namespace Application.Tests.Common
{
    public class FakeHttpClientFactory : IHttpClientFactory
    {
        private readonly Dictionary<string, Func<HttpClient>> _clients = new();

        public HttpClient CreateClient(string name)
        {
            var client = _clients[name]?.Invoke();

            return client?.AsTestClient() ?? throw new ArgumentException($"Client with name '{name}' is not available.");

        }

        public void AddMockClient(string name, Func<HttpClient> clientFactory)
        {
            _clients.Add(name, clientFactory);
        }
    }

    public static class HttpClientTestBuilder
    {
        public static readonly Uri TestBaseUrl = new Uri("http://test");

        public static HttpClient AsTestClient(this HttpClient client)
        {
            client.BaseAddress = TestBaseUrl;

            return client;
        }
    }
}
