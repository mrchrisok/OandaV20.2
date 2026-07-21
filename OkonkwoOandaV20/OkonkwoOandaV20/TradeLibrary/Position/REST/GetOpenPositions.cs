using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace OkonkwoOandaV20.TradeLibrary.REST
{
   /// <summary>
   /// http://developer.oanda.com/rest-live-v20/account-ep/
   /// </summary>
   public partial class Rest20
   {
      /// <summary>
      /// Retrieves the current open positions for a given account
      /// </summary>
      /// <param name="parameters">the parameters for the request</param>
      /// <param name="cancellation">a cancellation token that can cancel the operation</param>
      /// <returns>A list of Position objects with the details for each position (or empty list if no positions)</returns>
      public static async Task<OpenPositionsResponse> GetOpenPositionsAsync(OpenPositionsParameters parameters, CancellationToken cancellation = default)
      {
         var requestParams = new HttpParameters(parameters)
         {
            Method = HttpMethod.Get,
            Uri = new Uri(ServerUri(EServer.Account) + "accounts/" + parameters.accountID + "/openPositions"),
         };

         var response = await MakeRequestAsync<OpenPositionsResponse, OpenPositionsErrorResponse>(requestParams, cancellation);

         return response;
      }
   }

   public class OpenPositionsParameters : ApiParameters
   {
      /// <summary>
      /// The account ID
      /// </summary>
      [JsonIgnore]
      [Required]
      public string accountID { get; set; }
   }

   /// <summary>
   /// The GET success response received from accounts/accountID/openPositions
   /// </summary>
   public class OpenPositionsResponse : PositionsResponse
   {
   }

   /// <summary>
   /// The GET error response received from accounts/accountID/openPositions
   /// </summary>
   public class OpenPositionsErrorResponse : PositionsErrorResponse
   {
   }
}
