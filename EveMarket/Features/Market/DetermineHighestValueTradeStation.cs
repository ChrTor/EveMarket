using MediatR;
using EveMarket.HttpClients;
using static EveMarket.HttpClients.EveEntities.Market;
using EveMarket.EveData;
using System.Net.Mail;

namespace EveMarket.Features.Market
{
    public static class DetermineHighestValueTradeStation
    {
        public class Handler(EveClient EveClient) : IRequestHandler<ForCommodity, IEnumerable<OrderResponse>>
        {
            private readonly EveClient _eveClient = EveClient;

            public async Task<IEnumerable<OrderResponse>> Handle(ForCommodity request, CancellationToken cancellationToken)
            {
                List<Order> AllOrders = [];
                foreach (var region in EveRegions.RegionList)
                {
                    var orders = await GetMostValuableOrdersFromRegion(request.OrderType.ToString(), region.Region_Id, request.TypeId, cancellationToken);
                    if (!orders.Any())
                    {
                        // Implement ErrorOr?
                        continue;
                    }

                    AllOrders.AddRange(orders);
                }

                var HighestValueOrders = await HighestPriceOrderPerSystem(AllOrders, request.CurrentSystem, cancellationToken);
                var sortedList = HighestValueOrders.OrderByDescending(x => x.Price_Per_Jump);

                return sortedList;
            }

            private async Task<IEnumerable<OrderResponse>> HighestPriceOrderPerSystem(List<Order> allOrders, int currentSystem, CancellationToken cancellationToken)
            {
                var highestValueOrders = new List<OrderResponse>();
                foreach (var order in allOrders)
                {
                    var route = await _eveClient.GetRoute(currentSystem, order.System_Id, cancellationToken);
                    if (route is null)
                    {
                        continue;
                    }

                    highestValueOrders.Add(new OrderResponse(await SetSystemName(order.System_Id, cancellationToken), order.Price / route.Count(), order.Price, route.Count()));
                }

                return highestValueOrders;
            }

            private async Task<string> SetSystemName(long system_Id, CancellationToken cancellationToken)
            {
                var system = await _eveClient.GetSystem(system_Id, cancellationToken);
                return system.Name;
            }

            private async Task<IEnumerable<Order>> GetMostValuableOrdersFromRegion(string orderType, int regionId, int typeId, CancellationToken cancellationToken)
            {
                var orders = await _eveClient.GetOrdersForCommodity(orderType, regionId, typeId, cancellationToken);
                if (!orders.Any()) return orders;

                var cheapestOrdersByLocation = orders
                    .GroupBy(o => o.Location_Id)
                    .Select(g => g.OrderByDescending(o => o.Price).First());

                return cheapestOrdersByLocation;
            }

        }

        public record ForCommodity(int TypeId, OrderType OrderType, int CurrentSystem) : IRequest<IEnumerable<OrderResponse>>;
        public record OrderResponse(string SystemName, float Price_Per_Jump, float Price, int Jumps);
    }
}
