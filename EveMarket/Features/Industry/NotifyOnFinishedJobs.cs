using EveMarket.Common.SendEmail;
using EveMarket.EveData;
using EveMarket.HttpClients;
using MediatR;
using static EveMarket.HttpClients.EveEntities.Market;

namespace EveMarket.Features.Industry
{
    public class NotifyOnFinishedJobs
    {

        public class Handler(EveClient EveClient) : IRequestHandler<ForCharacter>
        {
            private readonly EveClient _eveClient = EveClient;

            public async Task Handle(ForCharacter request, CancellationToken cancellationToken)
            {
                var jobs = await _eveClient.GetJobsForCharacter(request.CharacterId, cancellationToken);


                var emailSender = new EmailSender();
                emailSender.Handle();

                // Perhaps store the attempt?
            }
        }

        public record ForCharacter(int CharacterId) : IRequest;
    }
}

