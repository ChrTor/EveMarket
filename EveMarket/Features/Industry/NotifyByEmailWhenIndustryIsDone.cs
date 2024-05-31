using EveMarket.Common.SendEmail;
using EveMarket.EveData;
using EveMarket.HttpClients;
using MediatR;

namespace EveMarket.Features.Industry
{
    public class NotifyByEmailWhenIndustryIsDone
    {
        private readonly EveClient _eveClient;

        public NotifyByEmailWhenIndustryIsDone(EveClient eveClient)
        {
            _eveClient = eveClient;
        }
        public async Task<bool> Handle(CancellationToken cancellationToken)
        {
            var emailSender = new SendEmail();
            emailSender.Handle();

            return false;
        }
    }
}
