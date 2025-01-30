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
                var contracts = await _eveClient.GetContractsForRegion((int)request.RegionId, cancellationToken);
                if (!contracts.Any())
                {
                    return Error.NotFound("No contracts found");
                }

                return new ContractResponse(contracts);
            }
        }

        public record FromRegion(EveRegions.RegionEnum RegionId, double MaxVolume, int Budget) : IRequest<ErrorOr<ContractResponse>>;
        public record ContractResponse(IEnumerable<Contract> Contracts);
    }
}
