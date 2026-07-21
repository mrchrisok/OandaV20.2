using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Net.Http;
using System.Security.Cryptography;
using System.Threading;
using System.Threading.Tasks;

namespace OkonkwoOandaV20.TradeLibrary.REST
{
   public partial class Rest20
   {
      /// <summary>
      /// Get a list of open Trades for an Account
      /// </summary>
      /// <param name="parameters">the parameters for the request</param>
      /// <param name="cancellation">a cancellation token that can cancel the operation</param>
      /// <returns>A list of TradeData objects (or empty list, if no trades)</returns>
      public static async Task<OpenTradesResponse> GetOpenTradesAsync(OpenTradesParameters parameters, CancellationToken cancellation = default)
      {
         var requestParams = new HttpParameters(parameters)
         {
            Method = HttpMethod.Get,
            Uri = new Uri(ServerUri(EServer.Account) + "accounts/" + parameters.accountID + "/openTrades"),
         };

         var response = await MakeRequestAsync<OpenTradesResponse, OpenTradesErrorResponse>(requestParams, cancellation);

         return response;
      }
   }

   public class OpenTradesParameters : ApiParameters
   {
      /// <summary>
      /// Account Identifier [required]
      /// </summary>
      [JsonIgnore]
      [Required]
      public string accountID { get; set; }
   }


   /// <summary>
   /// The GET success response received from accounts/accountID/openTrades
   /// </summary>
   public class OpenTradesResponse : TradesResponse
   {
   }

   /// <summary>
   /// The GET error response received from accounts/accountID/openTrades
   /// </summary>
   public class OpenTradesErrorResponse : TradesErrorResponse
   {
   }
}
