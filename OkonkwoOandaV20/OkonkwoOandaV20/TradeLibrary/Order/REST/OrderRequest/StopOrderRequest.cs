﻿using OkonkwoOandaV20.TradeLibrary.Order;

namespace OkonkwoOandaV20.TradeLibrary.REST.OrderRequest
{
   public class StopOrderRequest : PriceEntryOrderRequest
   {
      public StopOrderRequest(Instrument.Instrument oandaInstrument)
         : base(oandaInstrument)
      {
         type = OrderType.Stop;
      }
   }
}
