﻿namespace OkonkwoOandaV20.TradeLibrary.Transaction
{
   public class StopOrderRejectTransaction : PriceEntryOrderRejectTransaction
   {
	  /// <summary>
	  /// The worst market price that may be used to fill this Stop Order. If the
	  /// market gaps and crosses through both the price and the priceBound, the
	  /// Stop Order will be cancelled instead of being filled.
	  /// </summary>
	  public decimal? priceBound { get; set; }
   }
}
