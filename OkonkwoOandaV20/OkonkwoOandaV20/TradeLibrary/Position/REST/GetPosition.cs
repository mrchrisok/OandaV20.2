using System;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace OkonkwoOandaV20.TradeLibrary.REST
{
   public partial class Rest20
   {
      /// <summary>
      /// Retrieves the current position for the given instrument and account
      /// This will cause an error if there is no position for that instrument 
      /// </summary>
      /// <param name="accountID">the account for which to get the position</param>
      /// <param name="instrument">the instrument for which to get the position</param>
      /// <returns>Position object with the details of the position</returns>
      public static async Task<Position.Position> GetPositionAsync(string accountID, string instrument, CancellationToken cancellation = default)
      {
         var requestParams = new HttpParameters()
         {
            Method = HttpMethod.Get,
            Uri = new Uri(ServerUri(EServer.Account) + "accounts/" + accountID + "/positions/" + instrument),
            ForInternalResponse = true
         };

         var response = await MakeRequestAsync<PositionResponse, PositionErrorResponse>(requestParams, cancellation);

         Rest20.TransformObjectValues(response.position);

         return response.position;
      }
   }

   /// <summary>
   /// The GET success response received from accounts/accountID/positions/instrument
   /// </summary>
   public class PositionResponse : Response
   {
      /// <summary>
      /// The requested Position
      /// </summary>
      public Position.Position position { get; set; }
   }

   /// <summary>
   /// The GET error response received from accounts/accountID/positions/instrument
   /// </summary>
   public class PositionErrorResponse : ErrorResponse
   {
   }
}
