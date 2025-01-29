using EveMarket.HttpClients;
using MediatR;
using Microsoft.Extensions.Options;

namespace EveMarket.Features.Industry
{
    public class SetProfileCode
    {
        public class Handler(EveClient EveClient, IOptionsMonitor<EveOptions> optionsMonitor) : IRequestHandler<WithCredentials>
        {
            private readonly EveClient _eveClient = EveClient;
            private readonly EveOptions _options = optionsMonitor.CurrentValue;

            public async Task Handle(WithCredentials request, CancellationToken cancellationToken)
            {
                _eveClient.Profile = new Profile{Code = request.code};
            }
        }

        public record WithCredentials(string code) : IRequest;
    }
}

