using System;
using System.Net.Http;
using System.Text.Json.Serialization;
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
      /// <param name="parameters">the parameters for the request</param>
      /// <param name="cancellation">a cancellation token that can cancel the operation</param>
      /// <returns>Position object with the details of the position</returns>
      public static async Task<PositionResponse> GetPositionAsync(PositionParameters parameters, CancellationToken cancellation = default)
      {
         var requestParams = new HttpParameters(parameters)
         {
            Method = HttpMethod.Get,
            Uri = new Uri(ServerUri(EServer.Account) + "accounts/" + parameters.accountID + "/positions/" + parameters.instrument),

         };

         var response = await MakeRequestAsync<PositionResponse, PositionErrorResponse>(requestParams, cancellation);

         return response;
      }
   }

   public class PositionParameters : ApiParameters
   {
      /// <summary>
      /// The account ID
      /// </summary>
      [JsonIgnore]
      [JsonRequired]
      public string accountID { get; set; }

      /// <summary>
      /// The account ID
      /// </summary>
      [JsonIgnore]
      [JsonRequired]
      public string instrument { get; set; }
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
