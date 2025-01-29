using EveMarket.HttpClients;
using MediatR;
using System.Runtime.Serialization;
using ErrorOr;
using static EveMarket.HttpClients.EveEntities.Market;
using static EveMarket.EveData.EveRegions;

namespace EveMarket.Features.Market
{
    public static class FetchPricing
    {
        public class Handler(EveClient EveClient) : IRequestHandler<ForCommodity, ErrorOr<PricingResponse>>
        {
            private readonly EveClient _eveClient = EveClient;

            public async Task<ErrorOr<PricingResponse>> Handle(ForCommodity request, CancellationToken cancellationToken)
            {
                var regionId = (int)request.RegionId;
                var response = await _eveClient.GetOrdersForCommodity(request.OrderType.ToString(), regionId, request.TypeId, cancellationToken);
                if (!response.Orders.Any()) return Error.NotFound(description: "Pricing information not found");

                return response;
            }
        }

        [DataContract(Name = "For Commodity")]
        public record ForCommodity(int TypeId, RegionEnum RegionId, OrderType OrderType) : IRequest<ErrorOr<PricingResponse>>;

        public record PricingResponse(IEnumerable<Order> Orders);
    }
}
