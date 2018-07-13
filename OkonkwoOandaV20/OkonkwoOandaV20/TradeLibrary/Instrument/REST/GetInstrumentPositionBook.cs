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
      /// Fetch a position book for an instrument
      /// </summary>
      /// <param name="instrument">Name of the Instrument [required]</param>
      /// <param name="parameters">the parameters for the request</param>
      /// <returns>a PositionBook object</returns>
      public static async Task<PositionBook> GetInstrumentPositionBookAsync(string instrument, InstrumentPositionBookParameters parameters)
      {
         string uri = ServerUri(EServer.Account) + "instruments/" + instrument + "/positionBook";
         var requestParams = ConvertToDictionary(parameters);

         var response = await MakeRequestAsync<InstrumentPositionBookResponse>(uri, "GET", requestParams);

         return response.positionBook;
      }

      public class InstrumentPositionBookParameters
      {
         /// <summary>
         /// The time of the snapshot to fetch. If not specified, then the most recent snapshot 
         /// is fetched.
         /// </summary>
         public string time { get; set; }
      }
   }

   public class InstrumentPositionBookResponse : Response
   {
      /// <summary>
      /// The instrument’s position book
      /// </summary>
      public PositionBook positionBook;
   }
}