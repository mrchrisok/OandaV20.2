using OkonkwoOandaV20.TradeLibrary.DataTypes.Instrument;
using OkonkwoOandaV20.TradeLibrary.DataTypes.Order;

namespace OkonkwoOandaV20.TradeLibrary.REST.OrderRequest
{
   public class LimitOrderRequest : PriceEntryOrderRequest
   {
      public LimitOrderRequest(Instrument instrument) : base(instrument)
      {
         type = OrderType.Limit;
      }
   }
}
