﻿namespace OkonkwoOandaV20.TradeLibrary.Transaction
{
   public class TakeProfitOrderRejectTransaction : ExitOrderRejectTransaction
   {
	  /// <summary>
	  /// The price threshold specified for the TakeProfit Order. The associated
	  /// Trade will be closed by a market price that is equal to or better than
	  /// this threshold.
	  /// </summary>
	  public decimal price { get; set; }
   }
}
