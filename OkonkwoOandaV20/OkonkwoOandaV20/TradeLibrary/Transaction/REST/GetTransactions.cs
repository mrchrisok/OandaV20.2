using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using OkonkwoOandaV20.Framework;

using OkonkwoOandaV20.TradeLibrary.Transaction;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace OkonkwoOandaV20.TradeLibrary.REST
{
   public partial class Rest20
   {
      /// <summary>
      /// Get a list of Transactions pages that satisfy a time-based Transaction query.
      /// </summary>
      /// <param name="accountID">Account identifier</param>
      /// <param name="parameters">The parameters for the request</param>
      /// <returns></returns>
      public static async Task<List<ITransaction>> GetTransactionsAsync(string accountID, TransactionsParameters parameters, CancellationToken cancellation = default)
      {
         var requestParams = new HttpParameters(parameters)
         {
            Method = HttpMethod.Get,
            Uri = new Uri(ServerUri(EServer.Account) + $"accounts/{accountID}/transactions"),
            Binding = HttpParametersBinding.QueryString,
            ForInternalRequest = true
         };

         var pagesResponse = await MakeRequestAsync
            <TransactionsPageResponse, TransactionsPageErrorResponse>(requestParams, cancellation);

         var transactions = new List<ITransaction>();
         foreach (string page in pagesResponse.pages)
         {
            var parameters_ = new HttpParameters() { Uri = new Uri(page), ForInternalRequest = true };
            var transactions_ = await MakeRequestAsync<TransactionsResponse, TransactionsErrorResponse>(parameters_, cancellation);
            transactions.AddRange(transactions_.transactions);

            await Task.Delay(parameters.pagingDelayMilliSeconds); // throttle these a bit
         }

         Rest20.TransformObjectValues(transactions);

         return transactions;
      }

      public class TransactionsParameters : ApiParameters
      {
         /// <summary>
         /// The starting time (inclusive) of the time range for the Transactions being queried. 
         /// [default=Account Creation Time]
         /// </summary>
         public string from { get; set; }

         /// <summary>
         /// The ending time (inclusive) of the time range for the Transactions being queried. 
         /// [default=Request Time]
         /// </summary>
         public string to { get; set; }

         /// <summary>
         /// The number of Transactions to include in each page of the results. [default=100, maximum=1000]
         /// </summary>
         public int? pageSize { get; set; }

         /// <summary>
         /// A list of TransactionFilter values that restricts the types of Transactions to retreive.
         /// The valid values are defined in the TransactionFilter class.
         /// </summary>
         [JsonIgnore]
         public List<string> type { get; set; }

         /// <summary>
         /// Number of milliSeconds by which to throttle the transactions page requests. Throttled retrieval
         /// may be used optionally in test or resource-constrained production environments.
         /// </summary>
         [JsonIgnore]
         public int pagingDelayMilliSeconds { get; set; }
      }
   }

   #region TransactionsPageResponse, TransactionsPageErrorResponse

   /// <summary>
   /// The GET success response received from accounts/accountID/transactions
   /// </summary>
   public class TransactionsPageResponse : Response
   {
      /// <summary>
      /// The starting time provided in the request.
      /// </summary>
      public string from { get; set; }

      /// <summary>
      /// The ending time provided in the request.
      /// </summary>
      public string to { get; set; }

      /// <summary>
      /// The pageSize provided in the requests
      /// </summary>
      public int pageSize { get; set; }

      /// <summary>
      /// The Transaction-type filter provided in the request
      /// </summary>
      public List<string> type { get; set; }

      /// <summary>
      /// The number of Transactions that are contained in the pages returned
      /// </summary>
      public int count { get; set; }

      /// <summary>
      /// The list of URLs that represent idrange queries providing the data for
      /// each page in the query results
      /// </summary>
      public List<string> pages { get; set; }
   }

   /// <summary>
   /// The GET error response received from accounts/accountID/transactions
   /// </summary>
   public class TransactionsPageErrorResponse : ErrorResponse
   {
   }

   #endregion

   #region TransactionsResponse, TransactionsErrorResponse

   /// <summary>
   /// The GET success response received from accounts/accountID/transactions/idrange or similar
   /// </summary>
   public class TransactionsResponse : Response
   {
      /// <summary>
      /// The list of Transaction objects returned by the request
      /// </summary>
      public List<ITransaction> transactions { get; set; }
   }

   /// <summary>
   /// The GET error response received from accounts/accountID/transactions endpoints
   /// </summary>
   public class TransactionsErrorResponse : ErrorResponse
   {
   }

   #endregion
}
