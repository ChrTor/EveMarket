using EveMarket.EveData;
using EveMarket.HttpClients;
using MediatR;
using System.Runtime.Serialization;
using static EveMarket.HttpClients.EveEntities.Market;
using static EveMarket.EveData.EveRegions;
using System.Net.Mail;
using System.Net;



namespace EveMarket.Features.Market
{
    public static class FetchPricing
    {
        public class Handler(EveClient EveClient) : IRequestHandler<ForCommodity, PricingResponse>
        {
            private readonly EveClient _eveClient = EveClient;

            public async Task<PricingResponse> Handle(ForCommodity request, CancellationToken cancellationToken)
            {
                var orders = await GetOrdersFromRegion(request, cancellationToken);
                if (!orders.Any())
                {
                    // Implement ErrorOr?
                }

                return new PricingResponse(orders);
            }

            private async Task<IEnumerable<Order>> GetOrdersFromRegion(ForCommodity request, CancellationToken cancellationToken)
            {
                var regionId = (int)request.RegionId;
                var orders = await _eveClient.GetOrdersForCommodity(request.OrderType.ToString(), regionId, request.TypeId, cancellationToken);

                return orders;
            }
        }

        [DataContract(Name = "For Commodity")]
        public record ForCommodity(int TypeId, RegionEnum RegionId, OrderType OrderType) : IRequest<PricingResponse>;

        public record PricingResponse(IEnumerable<Order> Orders);
    }
}
