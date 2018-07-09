using OkonkwoOandaV20.TradeLibrary.DataTypes.Instrument;
using OkonkwoOandaV20.TradeLibrary.DataTypes.Order;

namespace OkonkwoOandaV20.TradeLibrary.REST.OrderRequest
{
   public class TakeProfitOrderRequest : ExitOrderRequest
   {
      public TakeProfitOrderRequest(Instrument oandaInstrument)
         : base(oandaInstrument)
      {
         type = OrderType.TakeProfit;
      }
   }
}
