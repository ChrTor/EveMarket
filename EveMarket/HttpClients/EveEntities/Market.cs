using System.Text.Json.Serialization;

namespace EveMarket.HttpClients.EveEntities
{
    public class Market
    {
        public record Order(
            int Duration,
            bool IsBuyOrder, 
            DateTime Issued, 
            long LocationId, 
            int MinVolume, 
            long OrderId, 
            float Price, 
            string Range, 
            long SystemId, 
            int TypeId, 
            int VolumeRemain, 
            int VolumeTotal, 
            int Jumps, 
            IEnumerable<int> Route, 
            float PricePerJump, 
            string SystemName);
        
        [JsonConverter(typeof(JsonStringEnumConverter))]
        public enum OrderType
        {
            buy,
            sell,
            all
        }
    }
}
