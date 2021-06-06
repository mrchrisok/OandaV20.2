﻿using Newtonsoft.Json;
using OkonkwoOandaV20.Framework;
using OkonkwoOandaV20.TradeLibrary.Transaction;

namespace OkonkwoOandaV20.TradeLibrary.REST.OrderRequests
{
   public abstract class OrderRequest : Request, IOrderRequest, IHasPrices
   {

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
	  /// The Order's price metadata.
	  /// </summary>
	  [JsonIgnore]
	  public PriceInformation priceInformation { get; set; }
   }
}
