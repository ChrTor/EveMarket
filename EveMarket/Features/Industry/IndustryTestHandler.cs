﻿using EveMarket.EveData;
using EveMarket.HttpClients;
using MediatR;
using static EveMarket.HttpClients.EveEntities.Market;

namespace EveMarket.Features.Industry
{
    public class IndustryTestHandler
    {

        public class Handler(EveClient EveClient) : IRequestHandler<ForCharacter, bool>
        {
            private readonly EveClient _eveClient = EveClient;

            public async Task<bool> Handle(ForCharacter request, CancellationToken cancellationToken)
            {
                var notifyTest = new NotifyByEmailWhenIndustryIsDone(_eveClient);
                var result = await notifyTest.Handle(request.CharacterId, cancellationToken);

                return result;
            }
        }

        public record ForCharacter(int CharacterId) : IRequest<bool>;
    }
}

