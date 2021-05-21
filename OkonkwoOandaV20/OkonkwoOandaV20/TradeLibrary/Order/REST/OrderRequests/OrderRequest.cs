using Newtonsoft.Json;
using OkonkwoOandaV20.Framework;
using OkonkwoOandaV20.Framework.JsonConverters;
using OkonkwoOandaV20.TradeLibrary.Transaction;

namespace OkonkwoOandaV20.TradeLibrary.REST.OrderRequests
{
   [JsonConverter(typeof(PriceObjectConverter))]
   public abstract class OrderRequest : Parameters, IOrderRequest, IHasPrices
   {
	  public OrderRequest()
	  {
	  }

	  /// <summary>
	  /// The type of the Order.
	  /// </summary>
	  public string type { get; set; }

	  /// <summary>
	  /// The client extensions of the Order. Do not set, modify, or delete
	  /// clientExtensions if your account is associated with MT4.
	  /// </summary>
	  public ClientExtensions clientExtensions { get; set; }

	  /// <summary>
	  /// The ORder's price metadata.
	  /// </summary>
	  [JsonIgnore]
	  public PriceInformation priceInformation { get; set; }
   }
}
