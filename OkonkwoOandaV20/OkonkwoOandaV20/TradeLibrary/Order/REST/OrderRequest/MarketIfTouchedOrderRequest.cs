using OkonkwoOandaV20.TradeLibrary.DataTypes.Instrument;
using OkonkwoOandaV20.TradeLibrary.DataTypes.Order;

namespace OkonkwoOandaV20.TradeLibrary.REST.OrderRequest
{
   public class MarketIfTouchedOrderRequest : PriceEntryOrderRequest
   {
      public MarketIfTouchedOrderRequest(Instrument oandaInstrument) 
         : base(oandaInstrument)
      {
         type = OrderType.MarketIfTouched;
      }
   }
}
