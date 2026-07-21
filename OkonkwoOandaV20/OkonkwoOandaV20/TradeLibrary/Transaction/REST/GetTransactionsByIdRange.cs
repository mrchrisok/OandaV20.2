using Newtonsoft.Json;
using OkonkwoOandaV20.Framework.JsonConverters;

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
      /// Get a range of Transactions for an Account based on the Transaction IDs
      /// http://developer.oanda.com/rest-live-v20/transaction-ep/
      /// </summary>
      /// <param name="accountID">Account identifier</param>
      /// <param name="parameters">The parameters for the request</param>
      /// <returns></returns>
      public static async Task<List<ITransaction>> GetTransactionsByIdRangeAsync(string accountID, TransactionsByIdRangeParameters parameters, CancellationToken cancellation = default)
      {
         var requestParams = new HttpParameters(parameters)
         {
            Method = HttpMethod.Get,
            Uri = new Uri(ServerUri(EServer.Account) + "accounts/" + accountID + "/transactions/idrange"),
            ForInternalResponse = true,
         };

         var response = await MakeRequestAsync<TransactionsByIdRangeResponse, TransactionsByIdRangeErrorResponse>(requestParams, cancellation);

         Rest20.TransformObjectValues(response.transactions);

         return response.transactions;
      }

      public class TransactionsByIdRangeParameters : ApiParameters
      {
         /// <summary>
         /// The starting Transacion ID (inclusive) to fetch. [required]
         /// </summary>
         public long from { get; set; }

         /// <summary>
         /// The ending Transaction ID (inclusive) to fetch. [required]
         /// </summary>
         public long to { get; set; }

         /// <summary>
         /// A comma separated list (csv) that restricts the types of Transactions to retreive.
         /// The valid values are defined in the TransactionFilter class.
         /// </summary>
         public List<string> type { get; set; }
      }
   }

   /// <summary>
   /// The GET success response received from accounts/accountID/transactions/idrange
   /// </summary>
   public class TransactionsByIdRangeResponse : TransactionsResponse
   {
   }

   /// <summary>
   /// The GET error response received from accounts/accountID/transactions/idrange
   /// </summary>
   public class TransactionsByIdRangeErrorResponse : TransactionsErrorResponse
   {
   }
}
