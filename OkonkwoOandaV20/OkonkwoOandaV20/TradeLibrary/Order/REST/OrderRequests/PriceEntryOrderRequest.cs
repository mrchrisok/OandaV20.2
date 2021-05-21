﻿using OkonkwoOandaV20.TradeLibrary.Order;

namespace OkonkwoOandaV20.TradeLibrary.REST.OrderRequests
{
   /// <summary>
   /// Base class for non-Market entry orders
   /// </summary>
   public abstract class PriceEntryOrderRequest : EntryOrderRequest
   {
	  public PriceEntryOrderRequest(Instrument.Instrument instrument)
		 : base(instrument)
	  {
		 timeInForce = TimeInForce.GoodUntilCancelled;
		 triggerCondition = OrderTriggerCondition.Default;
	  }

	  /// <summary>
	  /// The date/time when the MarketIfTouched Order will be cancelled if its
	  /// timeInForce is “GTD”.
	  /// </summary>
	  public string gtdTime { get; set; }

	  /// <summary>
	  /// Specification of which price component should be used when determining if
	  /// an Order should be triggered and filled. This allows Orders to be
	  /// triggered based on the bid, ask, mid, default (ask for buy, bid for sell)
	  /// or inverse (ask for sell, bid for buy) price depending on the desired
	  /// behaviour. Orders are always filled using their default price component.
	  /// This feature is only provided through the REST API. Clients who choose to
	  /// specify a non-default trigger condition will not see it reflected in any
	  /// of OANDA’s proprietary or partner trading platforms, their transaction
	  /// history or their account statements. OANDA platforms always assume that
	  /// an Order’s trigger condition is set to the default value when indicating
	  /// the distance from an Order’s trigger price, and will always provide the
	  /// default trigger condition when creating or modifying an Order. A special
	  /// restriction applies when creating a guaranteed Stop Loss Order. In this
	  /// case the TriggerCondition value must either be “DEFAULT”, or the
	  /// “natural” trigger side “DEFAULT” results in. So for a Stop Loss Order for
	  /// a long trade valid values are “DEFAULT” and “BID”, and for short trades
	  /// “DEFAULT” and “ASK” are valid.
	  /// </summary>
	  public string triggerCondition { get; set; }
   }
}
