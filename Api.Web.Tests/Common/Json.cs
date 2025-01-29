
namespace Application.Tests.Common
{
    public class Json
    {
        public const string nullResponse = null;

        public const string EmptyPricingInformation = @"{
            ""orders"": []
        }";

        public const string SinglePricingInformation = @"{""orders"": [
        {
            ""duration"": 7,
            ""is_buy_order"": false,
            ""issued"": ""2025-01-28T13:49:13Z"",
            ""location_id"": 60004588,
            ""min_volume"": 1,
            ""order_id"": 6974446321,
            ""price"": 2001,
            ""range"": ""region"",
            ""system_id"": 30002510,
            ""type_id"": 245,
            ""volume_remain"": 1,
            ""volume_total"": 1,
            ""jumps"": 0,
            ""route"": null,
            ""price_per_jump"": 0,
            ""system_name"": null
        }
       ]}";

        public const string PricingInformation = @"{""orders"": [
        {
          ""duration"": 7,
          ""is_buy_order"": false,
          ""issued"": ""2025-01-28T13:49:13Z"",
          ""location_id"": 60004588,
          ""min_volume"": 1,
          ""order_id"": 6974446321,
          ""price"": 2001,
          ""range"": ""region"",
          ""system_id"": 30002510,
          ""type_id"": 245,
          ""volume_remain"": 1,
          ""volume_total"": 1,
          ""jumps"": 0,
          ""route"": null,
          ""price_per_jump"": 0,
          ""system_name"": null
        },
        {
          ""duration"": 90,
          ""is_buy_order"": false,
          ""issued"": ""2025-01-26T01:00:06Z"",
          ""location_id"": 60004588,
          ""min_volume"": 1,
          ""order_id"": 6934192796,
          ""price"": 8359,
          ""range"": ""region"",
          ""system_id"": 30002510,
          ""type_id"": 245,
          ""volume_remain"": 1699,
          ""volume_total"": 1911,
          ""jumps"": 0,
          ""route"": null,
          ""price_per_jump"": 0,
          ""system_name"": null
        },
        {
          ""duration"": 3,
          ""is_buy_order"": false,
          ""issued"": ""2025-01-29T02:47:37Z"",
          ""location_id"": 60004588,
          ""min_volume"": 1,
          ""order_id"": 6974856039,
          ""price"": 1902,
          ""range"": ""region"",
          ""system_id"": 30002510,
          ""type_id"": 245,
          ""volume_remain"": 1,
          ""volume_total"": 1,
          ""jumps"": 0,
          ""route"": null,
          ""price_per_jump"": 0,
          ""system_name"": null
        }
      ]}";
    }
}
