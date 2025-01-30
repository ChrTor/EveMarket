using MediatR;
using EveMarket.HttpClients;
using static EveMarket.HttpClients.EveEntities.Market;
using EveMarket.EveData;
using System.Net.Mail;
using ErrorOr;

namespace EveMarket.Features.Market
{
    public static class DetermineHighestValueTradeStation
    {
        public class Handler(EveClient EveClient) : IRequestHandler<ForCommodity, ErrorOr<HighestValueSales>>
        {
            private readonly EveClient _eveClient = EveClient;

            public async Task<ErrorOr<HighestValueSales>> Handle(ForCommodity request, CancellationToken cancellationToken)
            {
                List<Order> AllOrders = [];
                foreach (var region in EveRegions.RegionList)
                {
                    var orders = await GetMostValuableOrdersFromRegion(request.OrderType.ToString(), region.Region_Id, request.TypeId, cancellationToken);
                    if (!orders.Any())
                    {
                        continue;
                    }

                    AllOrders.AddRange(orders);
                }
                if (!AllOrders.Any()) return Error.NotFound("No order found in any region");

                var highestValueOrders = await HighestPriceOrderPerSystem(AllOrders, request.CurrentSystem, cancellationToken);
                if (!highestValueOrders.Any()) return Error.NotFound("No routes found.");

                return new HighestValueSales(highestValueOrders);
            }

            private async Task<IEnumerable<SellValue>> HighestPriceOrderPerSystem(List<Order> allOrders, int currentSystem, CancellationToken cancellationToken)
            {
                var highestValueOrders = new List<SellValue>();
                foreach (var order in allOrders)
                {
                    var route = await _eveClient.GetRoute(currentSystem, (int)order.SystemId, cancellationToken);
                    if (route is null)
                    {
                        continue;
                    }

                    highestValueOrders.Add(new SellValue(await SetSystemName(order.SystemId, cancellationToken), order.Price / route.Count(), order.Price, route.Count()));
                }

                return highestValueOrders.OrderByDescending(x => x.PricePerJump);
            }
            private async Task<string> SetSystemName(long system_Id, CancellationToken cancellationToken)
            {
                var system = await _eveClient.GetSystem(system_Id, cancellationToken);
                return system.Name;
            }
            private async Task<IEnumerable<Order>> GetMostValuableOrdersFromRegion(string orderType, int regionId, int typeId, CancellationToken cancellationToken)
            {

                var response = await _eveClient.GetOrdersForCommodity(orderType, regionId, typeId, cancellationToken);
                if (!response.Orders.Any()) return response.Orders;

                response.Orders
                    .GroupBy(o => o.LocationId)
                    .Select(g => g.OrderByDescending(o => o.Price).First());

                return response.Orders;
            }

        }

        public record ForCommodity(int TypeId, OrderType OrderType, int CurrentSystem) : IRequest<ErrorOr<HighestValueSales>>;
        public record HighestValueSales(IEnumerable<SellValue> SellValues);
        public record SellValue(string SystemName, float PricePerJump, float Price, int Jumps);
    }
}
