using OkonkwoOandaV20.TradeLibrary.DataTypes.Instrument;
using OkonkwoOandaV20.TradeLibrary.DataTypes.Order;

namespace OkonkwoOandaV20.TradeLibrary.REST.OrderRequest
{
   public class TrailingStopLossOrderRequest : ExitOrderRequest
   {
      public TrailingStopLossOrderRequest(Instrument oandaInstrument)
         : base(oandaInstrument)
      {
         type = OrderType.TrailingStopLoss;
      }

      /// <summary>
      /// The price distance (in price units) specified for the TrailingStopLoss 
      /// Order.
      /// </summary>
      public decimal distance { get; set; }
   }
}
