using OkonkwoOandaV20.TradeLibrary.REST;
using OkonkwoOandaV20.TradeLibrary.Transaction;
using System;

namespace OkonkwoOandaV20.TradeLibrary.Order
{
   public class Order : Response, IOrder
   {
	  /// <summary>
	  /// The type of the Order.
	  /// Valid values are specified in the OrderType class.
	  /// </summary>
	  public string type { get; set; }

	  /// <summary>
	  /// The Order’s identifier, unique within the Order’s Account.
	  /// </summary>
	  public long id { get; set; }

	  /// <summary>
	  /// The time when the Order was created.
	  /// </summary>
	  public DateTime createTime { get; set; }

	  /// <summary>
	  /// The current state of the Order.
	  /// Valid values are specified in the OrderState class.
	  /// </summary>
	  public string state { get; set; }

	  /// <summary>
	  /// The client extensions of the Order. Do not set, modify, or delete 
	  /// clientExtensions if your account is associated with MT4.
	  /// </summary>
	  public ClientExtensions clientExtensions { get; set; }
   }
}
