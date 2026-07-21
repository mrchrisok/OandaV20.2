using Newtonsoft.Json;
using System;
using System.ComponentModel.DataAnnotations;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace OkonkwoOandaV20.TradeLibrary.REST
{
   public partial class Rest20
   {
      /// <summary>
      /// Gets the details for a specific Trade in an Account
      /// </summary>
      /// <param name="parameters">the parameters for the request</param>
      /// <param name="cancellation">a cancellation token that can cancel the operation</param>
      /// <returns>TradeData object containing the details of the trade</returns>
      public static async Task<TradeResponse> GetTradeAsync(TradeParameters parameters, CancellationToken cancellation = default)
      {
         var requestParams = new HttpParameters(parameters)
         {
            Method = HttpMethod.Get,
            Uri = new Uri(ServerUri(EServer.Account) + $"accounts/{parameters.accountID}/trades/{parameters.tradeSpecifier}"),
         };

         var response = await MakeRequestAsync<TradeResponse, TradeErrorResponse>(requestParams, cancellation);

         return response;
      }
   }

   public class TradeParameters : ApiParameters
   {
      /// <summary>
      /// The account ID
      /// </summary>
      [JsonIgnore]
      [Required]
      public string accountID { get; set; }

      /// <summary>
      /// The account ID
      /// </summary>
      [JsonIgnore]
      [Required]
      public long tradeSpecifier { get; set; }
   }

   /// <summary>
   /// The GET success response received from accounts/accountID/trades/tradeSpecifier
   /// </summary>
   public class TradeResponse : Response
   {
      /// <summary>
      /// The details for the Trade requested
      /// </summary>
      public Trade.Trade trade { get; set; }
   }

   /// <summary>
   /// The GET error response received from accounts/accountID/trades/tradeSpecifier
   /// </summary>
   public class TradeErrorResponse : ErrorResponse
   {
   }
}
