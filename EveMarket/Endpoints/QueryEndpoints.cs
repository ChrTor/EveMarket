using ErrorOr;
using EveMarket.Features.Industry;
using EveMarket.Features.Market;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using static EveMarket.EveData.EveRegions;
using static EveMarket.HttpClients.EveEntities.Market;


namespace EvE.Endpoints
{
    public static class QueryEndpoints
    {
        public static WebApplication MapQueryEndpoints(this WebApplication app)
        {
            var group = app
                .MapGroup("EveOnline")
                .WithTags("Queries");

            group.MapPost("/pricing_CQRS", async (FetchPricing.ForCommodity request,ISender sender) =>
            {
                var result = await sender.Send(request);
                return result;
            }).Produces<ErrorOr<FetchPricing.PricingResponse>>();

            group.MapPost("/pricing", async (int TypeId, [FromQuery] RegionEnum Region, [FromQuery]OrderType OrderType, 
                ISender sender, CancellationToken cancellationToken) =>
            {
                var result = await sender.Send(new FetchPricing.ForCommodity(TypeId, Region, OrderType), cancellationToken);
                return result;
            }).Produces<ErrorOr<FetchPricing.PricingResponse>>();

            group.MapPost("/route", async ([FromQuery] RegionEnum OriginRegion, [FromQuery] RegionEnum DestinationRegion,
                ISender sender, CancellationToken cancellationToken) =>
            {
                var result = await sender.Send(new GetRoute.WithRegions(OriginRegion, DestinationRegion), cancellationToken);
                return result;
            }).Produces<ErrorOr<GetRoute.RouteResponse>>();

            group.MapPost("/bestroute", async (int TypeId, [FromQuery]OrderType OrderType, int CurrentSystem, 
                ISender sender, CancellationToken cancellationToken) =>
            {
                var result = await sender.Send(new DetermineHighestValueTradeStation.ForCommodity(TypeId, OrderType, CurrentSystem), cancellationToken);
                return result;
            }).Produces<ErrorOr<DetermineHighestValueTradeStation.HighestValueSales>>();

            group.MapPost("/contracts", async ([FromQuery] RegionEnum Region, double MaxVolume, int Budget, 
                ISender sender, CancellationToken cancellationToken) =>
            {
                var result = await sender.Send(new FetchPublicCourierContracts.FromRegion(Region, MaxVolume, Budget), cancellationToken);
                return result;
            }).Produces<ErrorOr<FetchPublicCourierContracts.ContractResponse>>();

            group.MapPost("/jobs", async (int CharacterId,
                ISender sender, CancellationToken cancellationToken) =>
            {
                await sender.Send(new NotifyOnFinishedJobs.ForCharacter(CharacterId), cancellationToken);
            });

            return app;
        }
    }
}
