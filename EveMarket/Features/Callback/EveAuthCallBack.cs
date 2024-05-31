using EveMarket.EveData;
using EveMarket.HttpClients;
using MediatR;
using Microsoft.Extensions.Options;
using static EveMarket.HttpClients.EveEntities.Market;

namespace EveMarket.Features.Industry
{
    public class EveAuthCallBack
    {

        public class Handler(EveClient EveClient, IOptionsMonitor<EveOptions> optionsMonitor) : IRequestHandler<WithCredentials, object>
        {
            private readonly EveClient _eveClient = EveClient;
            private readonly EveOptions _options = optionsMonitor.CurrentValue;

            public async Task<object> Handle(WithCredentials request, CancellationToken cancellationToken)
            {
                await _eveClient.AuthenticateCode(request.code, cancellationToken);

                return Results.Redirect("/swagger/index.html");
            }
        }

        public record WithCredentials(string code) : IRequest<object>;
    }
}

