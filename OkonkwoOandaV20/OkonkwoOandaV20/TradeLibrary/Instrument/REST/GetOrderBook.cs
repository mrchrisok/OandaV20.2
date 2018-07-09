using OkonkwoOandaV20.TradeLibrary.DataTypes.Instrument;
using System.Threading.Tasks;

namespace OkonkwoOandaV20.TradeLibrary.REST
{
   /// <summary>
   /// http://developer.oanda.com/rest-live-v20/account-ep/
   /// </summary>
   public partial class Rest20
   {
      /// <summary>
      /// Retrieves the order book for an instrument
      /// </summary>
      /// <param name="parameters">the parameters for the request</param>
      /// <returns>a PositionBook object</returns>
      public static async Task<PositionBook> GetOrderBookAsync(OrderBookParameters parameters)
      {
         string uri = ServerUri(Server.Account) + "instruments/" + parameters.instrument + "/orderBook";
         var requestParams = ConvertToDictionary(parameters);

         var response = await MakeRequestAsync<PositionBookResponse>(uri, "GET", requestParams);

         return response.positionBook;
      }
   }

   public class OrderBookParameters : InstrumentParameters
   {
      /// <summary>
      /// The time of the snapshot to fetch. If not specified, then the most recent snapshot 
      /// is fetched.
      /// </summary>
      public string time { get; set; }
   }

   public class OrderBookResponse : Response
   {
      /// <summary>
      /// The instrument’s order book
      /// </summary>
      public OrderBook orderBook;
   }
}