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
      /// Retrieves the position book for an instrument
      /// </summary>
      /// <param name="parameters">the parameters for the request</param>
      /// <returns>a PositionBook object</returns>
      public static async Task<PositionBook> GetPositionBookAsync(PositionBookParameters parameters)
      {
         string uri = ServerUri(Server.Account) + "instruments/" + parameters.instrument + "/positionBook";
         var requestParams = ConvertToDictionary(parameters);

         var response = await MakeRequestAsync<PositionBookResponse>(uri, "GET", requestParams);

         return response.positionBook;
      }
   }

   public class PositionBookParameters : InstrumentParameters
   {
      /// <summary>
      /// The time of the snapshot to fetch. If not specified, then the most recent snapshot 
      /// is fetched.
      /// </summary>
      public string time { get; set; }
   }

   public class PositionBookResponse : Response
   {
      /// <summary>
      /// The instrument’s position book
      /// </summary>
      public PositionBook positionBook;
   }
}