using OkonkwoOandaV20.TradeLibrary.DataTypes.Instrument;
using OkonkwoOandaV20.TradeLibrary.DataTypes.Order;

namespace OkonkwoOandaV20.TradeLibrary.REST.OrderRequest
{
   public class MarketOrderRequest : EntryOrderRequest
   {
      public MarketOrderRequest(Instrument oandaInstrument)
         : base(oandaInstrument)
      {
         type = OrderType.Market;
         timeInForce = TimeInForce.FillOrKill;
      }
   }
}
