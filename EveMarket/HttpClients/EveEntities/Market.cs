namespace EveMarket.HttpClients.EveEntities
{
    public class Market
    {
        public record Order(int Duration, bool Is_Buy_Order, DateTime Issued, long Location_Id, int Min_Volume, long Order_Id, float Price, string Range, long System_Id, int Type_Id, int Volume_Remain, int Volume_Total, int Jumps, IEnumerable<int> Route, float Price_Per_Jump, string SystemName);
        public enum OrderType
        {
            buy,
            sell,
            all
        }
    }
}
