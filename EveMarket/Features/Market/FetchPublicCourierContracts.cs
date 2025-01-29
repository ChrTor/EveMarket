using ErrorOr;
using EveMarket.EveData;
using EveMarket.HttpClients;
using MediatR;
using static EveMarket.HttpClients.EveEntities.Contracts;

namespace EveMarket.Features.Market
{
    public static class FetchPublicCourierContracts
    {
        public class Handler(EveClient EveClient) : IRequestHandler<FromRegion, ErrorOr<ContractResponse>>
        {
            private readonly EveClient _eveClient = EveClient;

            public async Task<ErrorOr<ContractResponse>> Handle(FromRegion request, CancellationToken cancellationToken)
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

                if (!couriersContracts.Any()) return Error.NotFound("No contracts found");

                return new ContractResponse(couriersContracts);
            }
        }

        public record FromRegion(EveRegions.RegionEnum RegionId, double MaxVolume, int Budget) : IRequest<ErrorOr<ContractResponse>>;
        public record ContractResponse(IEnumerable<Contract> Contracts);
    }
}
