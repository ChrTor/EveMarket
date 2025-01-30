using System.Text.Json;
using static EveMarket.HttpClients.EveEntities.Contracts;
using static EveMarket.HttpClients.EveEntities.Locations;
using static EveMarket.HttpClients.EveEntities.Market;
using static EveMarket.HttpClients.EveEntities.Industry;
using Microsoft.Extensions.Options;
using EveMarket.Features.Market;
using System.Web;

namespace EveMarket.HttpClients
{
    public class EveClient
    {
        public const string EveDefaultDataSource = "tranquility";
        public HttpClient _httpClient { get; set; }
        public EveOptions _eveOptions { get; set; }

        public EveClient(HttpClient httpClient, IOptionsMonitor<EveOptions> optionsMonitor)
        { 
            this._httpClient = httpClient;
            this._httpClient.BaseAddress = new Uri("https://esi.evetech.net/latest/");
            _eveOptions = optionsMonitor.CurrentValue;
            optionsMonitor.OnChange(options =>
            {
                _eveOptions = options;
            });
        }

        public async Task<IEnumerable<SolarSystem>> GetRoute(int origin, int destination, CancellationToken cancellationToken)
        {
            string url = $"route/{origin}/{destination}/?datasource=tranquility&flag=shortest";

            var response = await _httpClient.GetAsync(url, cancellationToken);
            response.EnsureSuccessStatusCode();

            using var responseStream = await response.Content.ReadAsStreamAsync();
            var route = await JsonSerializer.DeserializeAsync<List<SolarSystem>>(responseStream, new JsonSerializerOptions { PropertyNameCaseInsensitive = true, PropertyNamingPolicy = JsonNamingPolicy.SnakeCaseLower});

            return route;
        }

        public async Task<FetchPricing.PricingResponse> GetOrdersForCommodity(string orderType, int regionId, int typeId, CancellationToken cancellationToken)
        {
            string url = $"markets/{regionId}/orders/?datasource=tranquility&order_type={orderType}&page=1&type_id={typeId}";

            var response = await _httpClient.GetAsync(url, cancellationToken);
            response.EnsureSuccessStatusCode();

            using var responseStream = await response.Content.ReadAsStreamAsync();
            var orders = await JsonSerializer.DeserializeAsync<IEnumerable<Order>>(responseStream, new JsonSerializerOptions { PropertyNameCaseInsensitive = true, PropertyNamingPolicy = JsonNamingPolicy.SnakeCaseLower});
            return new FetchPricing.PricingResponse(orders);
        }

        public async Task<SolarSystem> GetSystem(long system_Id, CancellationToken cancellationToken)
        {
            string url = $"universe/systems/{system_Id}/?datasource=tranquility";

            var response = await _httpClient.GetAsync(url, cancellationToken);
            response.EnsureSuccessStatusCode();

            using var responseStream = await response.Content.ReadAsStreamAsync();
            var system = await JsonSerializer.DeserializeAsync<SolarSystem>(responseStream, new JsonSerializerOptions { PropertyNameCaseInsensitive = true }, cancellationToken);

            return system;
        }

        public async Task<IEnumerable<Contract>> GetContractsForRegion(int regionId, CancellationToken cancellationToken)
        {
            string url = $"contracts/public/{regionId}/?datasource=tranquility";
            var response = await _httpClient.GetAsync(url, cancellationToken);
            response.EnsureSuccessStatusCode();

            using var responseStream = await response.Content.ReadAsStreamAsync();
            var contracts = await JsonSerializer.DeserializeAsync<IEnumerable<Contract>>(responseStream, new JsonSerializerOptions { PropertyNameCaseInsensitive = true, PropertyNamingPolicy = JsonNamingPolicy.SnakeCaseLower }, cancellationToken);

            return contracts;
        }

        public async Task<IEnumerable<Job>> GetJobsForCharacter(int characterId, CancellationToken cancellationToken)
        {
            string url = $"characters/{characterId}/industry/jobs/?datasource=tranquility&token={_eveOptions.Profile!.Code}";
            var response = await _httpClient.GetAsync(url, cancellationToken);
            response.EnsureSuccessStatusCode();

            using var responseStream = await response.Content.ReadAsStreamAsync();
            var jobs = await JsonSerializer.DeserializeAsync<IEnumerable<Job>>(responseStream, new JsonSerializerOptions { PropertyNameCaseInsensitive = true }, cancellationToken);

            return jobs;
        }

        public async Task AuthenticateCode(string code, CancellationToken cancellationToken)
        {
            _httpClient.BaseAddress = new Uri("https://login.eveonline.com/v2/oauth/authorize/");

            string endodedUrl = HttpUtility.UrlEncode(_eveOptions.AuthTokenCallbackUrl);
            string url = $"?response_type=code&redirect_uri={endodedUrl}&client_id={_eveOptions.ClientId}&scope={_eveOptions.EnabledScopes[0].Address}&state={_eveOptions.State}";

            var response = await _httpClient.GetAsync(url);
            response.EnsureSuccessStatusCode();


            using var responseStream = await response.Content.ReadAsStreamAsync();
            var responseData = JsonSerializer.DeserializeAsync<Dictionary<string, string>>(responseStream);

            _httpClient.BaseAddress = new Uri("https://esi.evetech.net/latest/");


        }
    }
}
