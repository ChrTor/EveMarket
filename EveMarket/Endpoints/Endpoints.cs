using EveMarket.Features.Industry;
using EveMarket.Features.Market;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using static EveMarket.EveData.EveRegions;
using static EveMarket.HttpClients.EveEntities.Market;


namespace EvE.Endpoints
{
    public static class Endpoints
    {
        public static WebApplication MapEndpoints(this WebApplication app)
        {
            var group = app.MapGroup("/api");

            group.MapPost("/pricing", async (int TypeId, [FromQuery] RegionEnum Region, [FromQuery]OrderType OrderType, 
                ISender sender, CancellationToken cancellationToken) =>
            {
                var result = await sender.Send(new FetchPricing.ForCommodity(TypeId, Region, OrderType), cancellationToken);
                return result;
            });

            group.MapPost("/bestroute", async (int TypeId, [FromQuery]OrderType OrderType, int CurrentSystem, 
                ISender sender, CancellationToken cancellationToken) =>
            {
                var result = await sender.Send(new DetermineHighestValueTradeStation.ForCommodity(TypeId, OrderType, CurrentSystem), cancellationToken);
                return result;
            });

            group.MapPost("/contracts", async ([FromQuery] RegionEnum Region, double MaxVolume, int Budget, 
                ISender sender, CancellationToken cancellationToken) =>
            {
                var result = await sender.Send(new FetchPublicCourierContracts.FromRegion(Region, MaxVolume, Budget), cancellationToken);
                return result;
            });

            group.MapPost("/jobs", async (int CharacterId,
                ISender sender, CancellationToken cancellationToken) =>
            {
                var result = await sender.Send(new IndustryTestHandler.ForCharacter(CharacterId), cancellationToken);
                return result;
            });


            return app;
        }
    }
}
