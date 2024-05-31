using EveMarket.EveData;
using EveMarket.HttpClients;
using EveMarket.HttpClients.EveEntities;
using MediatR;
using System.Collections.Immutable;
using static EveMarket.HttpClients.EveEntities.Contracts;

namespace EveMarket.Features.Market
{
    public static class FetchPublicCourierContracts
    {
        public class Handler(EveClient EveClient) : IRequestHandler<FromRegion, ContractResponse>
        {
            private readonly EveClient _eveClient = EveClient;

            public async Task<ContractResponse> Handle(FromRegion request, CancellationToken cancellationToken)
            {
                var couriersContracts = new List<Contract>();
                foreach (var region in EveRegions.RegionList)
                {
                    var contracts = await _eveClient.GetContractsForRegion((int)request.RegionId, cancellationToken);
                    if (!contracts.Any())
                    {
                        continue;
                    }

                    couriersContracts.AddRange(contracts.Where(x =>
                    x.Type == Types.courier.ToString()
                    && x.Volume <= request.MaxVolume
                    && x.Collateral <= request.Budget));
                }

                return new ContractResponse(couriersContracts);
            }
        }

        public record FromRegion(EveRegions.RegionEnum RegionId, double MaxVolume, int Budget) : IRequest<ContractResponse>;
        public record ContractResponse(IEnumerable<Contract> Contracts);
    }
}
