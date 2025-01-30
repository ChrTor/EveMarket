using ErrorOr;
using EveMarket.Features.Industry;
using EveMarket.Features.Market;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using static EveMarket.EveData.EveRegions;
using static EveMarket.HttpClients.EveEntities.Market;


namespace EvE.Endpoints
{
    public static class CallBackEndpoint
    {
        public static WebApplication MapCallBackEndpoint(this WebApplication app)
        {
            var group = app
                .MapGroup("")
                .WithTags("External");

            group.MapGet("/api/oauth-callback", async (HttpContext context) =>
            {
                // Used by CCP to return profile code.
                var authCode = context.Request.Query["code"];

                ISender sender = context.RequestServices.GetRequiredService<ISender>();
                await sender.Send(new SetAndAuthorizeProfileCode.WithCredentials(code: authCode!));
                return Results.Redirect("/swagger/index.html");
            });

            group.MapGet("/api/authcode-callback", async (HttpContext context) =>
            {
                // Used by CCP to return profile code.
                var authCode = context.Request;

                //ISender sender = context.RequestServices.GetRequiredService<ISender>();
                //await sender.Send(new SetAndAuthorizeProfileCode.WithCredentials(code: authCode!));
                return Results.Redirect("/swagger/index.html");
            });

            return app;
        }
    }
}
