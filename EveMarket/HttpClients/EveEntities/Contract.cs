namespace EveMarket.HttpClients.EveEntities
{
    public class Contracts
    {
        public record Contract(
        double BuyOut,
        double Collateral,
        int Contract_id,
        DateTime Date_expired,
        DateTime Date_issued,
        int Days_to_complete,
        long End_location_id,
        bool For_corporation,
        int Issuer_corporation_id,
        int Issuer_id,
        double Price,
        long Start_location_id,
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
