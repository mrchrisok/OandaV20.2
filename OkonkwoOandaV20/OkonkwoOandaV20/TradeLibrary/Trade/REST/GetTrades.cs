using Newtonsoft.Json;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using System.Net.Http;
using Newtonsoft.Json.Linq;
using System;
using System.ComponentModel.DataAnnotations;


namespace OkonkwoOandaV20.TradeLibrary.REST
{
   public partial class Rest20
   {
      /// <summary>
      /// Get a list of Trades for an Account
      /// </summary>
      /// <param name="parameters">the parameters for the request</param>
      /// <param name="cancellation">a cancellation token that can cancel the operation</param>
      /// <returns>A list of TradeData objects (or empty list, if no trades)</returns>
      public static async Task<TradesResponse> GetTradesAsync(TradesParameters parameters, CancellationToken cancellation = default)
      {
         var requestParams = new HttpParameters(parameters)
         {
            Method = HttpMethod.Get,
            Uri = new Uri(ServerUri(EServer.Account) + $"accounts/{parameters.accountID}/trades"),
            Binding = HttpParametersBinding.QueryString
         };

         var response = await MakeRequestAsync<TradesResponse, TradesErrorResponse>(requestParams, cancellation);

         Rest20.TransformObjectValues(response.trades);

         return response;
      }

      public class TradesParameters : ApiParameters
      {
         /// <summary>
         /// The account ID
         /// </summary>
         [Required]
         [JsonIgnore]
         public string accountID { get; set; }

         /// <summary>
         /// Comma separated list of tradeIDs to retrieve
         /// </summary>
         public List<string> ids { get; set; }

         /// <summary>
         /// The state to filter the requested Trades by. [default=OPEN]
         /// Valid values are specified in the TradeStateFilter class
         /// </summary>
         public string state { get; set; }

         /// <summary>
         /// The instrument to filter the requested Trades by.
         /// </summary>
         public string instrument { get; set; }

         /// <summary>
         /// The maximum number of Trades to return. [default=50, maximum=500]
         /// </summary>
         public int? count { get; set; }

         /// <summary>
         /// The maximum Trade ID to return. If not provided the most recent Trades in the Account are returned.
         /// </summary>
         public long? beforeID { get; set; }
      }
   }

   /// <summary>
   /// The GET success response received from accounts/accountID/trades
   /// </summary>
   public class TradesResponse : Response
   {
      /// <summary>
      /// The list of Trade detail objects
      /// </summary>
      public List<Trade.Trade> trades { get; set; }
   }

   /// <summary>
   /// The GET error response received from accounts/accountID/trades
   /// </summary>
   public class TradesErrorResponse : ErrorResponse
   {
   }
}
