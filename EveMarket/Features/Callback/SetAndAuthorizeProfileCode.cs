using EveMarket.HttpClients;
using MediatR;
using Microsoft.Extensions.Options;

namespace EveMarket.Features.Industry
{
    public class SetAndAuthorizeProfileCode
    {
        public class Handler(EveClient eveClient, IOptionsMonitor<EveOptions> optionsMonitor) : IRequestHandler<WithCredentials>
        {
            private readonly EveClient _eveClient = eveClient;
            private readonly EveOptions _eveOptions = optionsMonitor.CurrentValue;

            public async Task Handle(WithCredentials request, CancellationToken cancellationToken)
            {
                _eveOptions.Profile = new Profile{Code = request.code};

                var results = _eveClient.AuthenticateCode(_eveOptions.Profile.Code, cancellationToken);



            }
        }

        public record WithCredentials(string code) : IRequest;
    }
}

