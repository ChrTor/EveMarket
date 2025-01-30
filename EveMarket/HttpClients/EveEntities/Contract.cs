namespace EveMarket.HttpClients.EveEntities
{
    public class Contracts
    {
        public record Contract(
        double BuyOut,
        double Collateral,
        int ContractId,
        DateTime DateExpired,
        DateTime DateIssued,
        int DaysToComplete,
        long EndLocationId,
        bool ForCorporation,
        int IssuerCorporationId,
        int IssuerId,
        double Price,
        long StartLocationId,
        string Title,
        string Type,
        double Volume);

        public enum Types
        {
            courier,
            item_exchange,
        }

    }
    
}
