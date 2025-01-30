using ErrorOr;
using EveMarket.EveData;
using EveMarket.HttpClients;
using MediatR;
using static EveMarket.HttpClients.EveEntities.Locations;

namespace EveMarket.Features.Market
{
    public static class GetRoute
    {
        public class Handler(EveClient EveClient) : IRequestHandler<WithRegions, ErrorOr<RouteResponse>>
        {
            private readonly EveClient _eveClient = EveClient;

            public async Task<ErrorOr<RouteResponse>> Handle(WithRegions request, CancellationToken cancellationToken)
            {
                var route = await _eveClient.GetRoute((int)request.Origin, (int)request.Destination, cancellationToken);
                if (route == null)
                {
                    return Error.NotFound("No route found");
                }

                return new RouteResponse(route);
            }
        }

        public record WithRegions(EveRegions.RegionEnum Origin, EveRegions.RegionEnum Destination) : IRequest<ErrorOr<RouteResponse>>;
        public record RouteResponse(IEnumerable<SolarSystem> Contracts);
    }
}
