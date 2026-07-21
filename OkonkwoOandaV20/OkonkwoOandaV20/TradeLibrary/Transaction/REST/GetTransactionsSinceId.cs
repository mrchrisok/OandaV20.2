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
      /// Get a range of Transactions for an Account starting at (but not including) a provided Transaction ID.
      /// http://developer.oanda.com/rest-live-v20/transaction-ep/#_collapse_endpoint_5
      /// </summary>
      /// <param name="parameters">the parameters for the request</param>
      /// <param name="cancellation">a cancellation token that can cancel the operation</param>
      /// <returns>A list of transaction objects</returns>
      public static async Task<TransactionsSinceIdRangeResponse> GetTransactionsSinceIdAsync(TransactionsSinceIdParameters parameters
         , CancellationToken cancellation = default)
      {
         var requestParams = new HttpParameters(parameters)
         {
            Method = HttpMethod.Get,
            Uri = new Uri(ServerUri(EServer.Account) + "accounts/" + parameters.accountID + "/transactions/sinceid"),
         };

         var response = await MakeRequestAsync
            <TransactionsSinceIdRangeResponse, TransactionsSinceIdRangeErrorResponse>(requestParams, cancellation);

         return response;
      }
   }

   public class TransactionsSinceIdParameters : ApiParameters
   {
      /// <summary>
      /// Account Identifier [required]
      /// </summary>
      [JsonIgnore]
      [Required]
      public string accountID { get; set; }

      /// <summary>
      /// The ID of the last Transaction fetched. 
      /// This query will return all Transactions newer than the TransactionID. [required]
      /// </summary>
      [Required]
      public long id { get; set; }

      /// <summary>
      /// A filter for restricting the types of Transactions to retrieve.
      /// Valid values are listed in the TransactionFilter class
      /// /// </summary>
      public string type { get; set; }
   }

   /// <summary>
   /// The GET success response received from accounts/accountID/transactions/sinceid
   /// </summary>
   public class TransactionsSinceIdRangeResponse : TransactionsResponse
   {
   }

   /// <summary>
   /// The GET error response received from accounts/accountID/transactions/sinceid
   /// </summary>
   public class TransactionsSinceIdRangeErrorResponse : TransactionsErrorResponse
   {
   }
}
