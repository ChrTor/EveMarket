using System.Net;
using System.Text.Json;
using static EveMarket.HttpClients.EveEntities.Contracts;
using static EveMarket.HttpClients.EveEntities.Locations;
using static EveMarket.HttpClients.EveEntities.Market;
using static EveMarket.HttpClients.EveEntities.Industry;
using Microsoft.Extensions.Options;

namespace EveMarket.HttpClients
{
    public class EveClient
    {
        public HttpClient httpClient { get; set; }
        public EveOptions EveOptions { get; set; }    
        public const string EveDefaultDataSource = "tranquility";

        public EveClient(HttpClient httpClient, IOptionsMonitor<EveOptions> optionsMonitor)
        { 
            this.httpClient = httpClient;
            this.httpClient.BaseAddress = new Uri("https://esi.evetech.net/latest/");
            EveOptions = optionsMonitor.CurrentValue;
            optionsMonitor.OnChange(options =>
            {
                EveOptions = options;
            });
        }

        public async Task<IEnumerable<int>> GetRoute(int origin, long destination, CancellationToken cancellationToken)
        {
            string url = $"route/{origin}/{destination}?datasource=tranquility&flag=secure";

            var response = await httpClient.GetAsync(url, cancellationToken);
            if (response.StatusCode == HttpStatusCode.NotFound)
            {
                return null;
            }
            response.EnsureSuccessStatusCode();

            using var responseStream = await response.Content.ReadAsStreamAsync();
            var route = await JsonSerializer.DeserializeAsync<List<int>>(responseStream, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

            return route;
        }

        public async Task<IEnumerable<Order>> GetOrdersForCommodity(string orderType, int regionId, int typeId, CancellationToken cancellationToken)
        {
            string url = $"markets/{regionId}/orders/?datasource=tranquility&order_type={orderType}&page=1&type_id={typeId}";

            var response = await httpClient.GetAsync(url, cancellationToken);
            response.EnsureSuccessStatusCode();

            using var responseStream = await response.Content.ReadAsStreamAsync();
            var orders = await JsonSerializer.DeserializeAsync<List<Order>>(responseStream, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

            return orders;
        }

        public async Task<SolarSystem> GetSystem(long system_Id, CancellationToken cancellationToken)
        {
            string url = $"universe/systems/{system_Id}?datasource=tranquility";

            var response = await httpClient.GetAsync(url, cancellationToken);
            response.EnsureSuccessStatusCode();

            using var responseStream = await response.Content.ReadAsStreamAsync();
            var system = await JsonSerializer.DeserializeAsync<SolarSystem>(responseStream, new JsonSerializerOptions { PropertyNameCaseInsensitive = true }, cancellationToken);

            return system;
        }

        public async Task<IEnumerable<Contract>> GetContractsForRegion(int regionId, CancellationToken cancellationToken)
        {
            string url = $"contracts/public/{regionId}/?datasource=tranquility";
            var response = await httpClient.GetAsync(url, cancellationToken);
            response.EnsureSuccessStatusCode();

            using var responseStream = await response.Content.ReadAsStreamAsync();
            var contracts = await JsonSerializer.DeserializeAsync<IEnumerable<Contract>>(responseStream, new JsonSerializerOptions { PropertyNameCaseInsensitive = true }, cancellationToken);

            return contracts;
        }

        public async Task<IEnumerable<Job>> GetJobsForCharacter(int characterId, CancellationToken cancellationToken)
        {
            string url = $"characters/{characterId}/industry/jobs/?datasource=tranquility&token={EveOptions.Code}";
            var response = await httpClient.GetAsync(url, cancellationToken);
            response.EnsureSuccessStatusCode();

            using var responseStream = await response.Content.ReadAsStreamAsync();
            var jobs = await JsonSerializer.DeserializeAsync<IEnumerable<Job>>(responseStream, new JsonSerializerOptions { PropertyNameCaseInsensitive = true }, cancellationToken);

            return jobs;
        }

        public async Task AuthenticateCode(string code, CancellationToken cancellationToken)
        {
            string url = $"oauth/token";
            httpClient.BaseAddress = new Uri("https://login.eveonline.com/");

            try
            {
                var response = await httpClient.GetAsync(url, cancellationToken);

                response.EnsureSuccessStatusCode();

                httpClient.BaseAddress = new Uri("https://esi.evetech.net/latest/");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Exception during HTTP request: {ex.Message}");
                throw;
            }
        }
    }
}
