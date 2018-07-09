﻿using OkonkwoOandaV20.TradeLibrary.Order;

namespace OkonkwoOandaV20.TradeLibrary.REST.OrderRequest
{
   public class LimitOrderRequest : PriceEntryOrderRequest
   {
      public LimitOrderRequest(Instrument.Instrument instrument) : base(instrument)
      {
         type = OrderType.Limit;
      }
   }
}
