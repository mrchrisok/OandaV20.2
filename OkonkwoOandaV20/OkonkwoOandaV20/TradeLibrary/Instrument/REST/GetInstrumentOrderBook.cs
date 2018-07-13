using OkonkwoOandaV20.TradeLibrary.Instrument;
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
      /// <param name="instrument">Name of the Instrument [required]</param>
      /// <param name="parameters">the parameters for the request</param>
      /// <returns>an OrderBook object</returns>
      public static async Task<OrderBook> GetInstrumentOrderBookAsync(string instrument, InstrumentOrderBookParameters parameters)
      {
         string uri = ServerUri(EServer.Account) + "instruments/" + instrument + "/orderBook";
         var requestParams = ConvertToDictionary(parameters);

         var response = await MakeRequestAsync<InstrumentOrderBookResponse>(uri, "GET", requestParams);

         return response.orderBook;
      }

      public class InstrumentOrderBookParameters
      {
         /// <summary>
         /// The time of the snapshot to fetch. If not specified, then the most recent snapshot 
         /// is fetched.
         /// </summary>
         public string time { get; set; }
      }
   }

   public class InstrumentOrderBookResponse : Response
   {
      /// <summary>
      /// The instrument’s order book
      /// </summary>
      public OrderBook orderBook;
   }
}