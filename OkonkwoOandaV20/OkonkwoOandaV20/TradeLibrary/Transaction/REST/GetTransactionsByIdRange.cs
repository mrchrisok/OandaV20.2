using Newtonsoft.Json;
using OkonkwoOandaV20.Framework.JsonConverters;
using OkonkwoOandaV20.TradeLibrary.Transaction;
using System.Collections.Generic;
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
      public static async Task<List<ITransaction>> GetTransactionsByIdRangeAsync(string accountID, TransactionsByIdRangeParameters parameters)
      {
         string uri = null;
         TransactionsResponse response = null;

         if (!string.IsNullOrEmpty(parameters.page))
         {
            uri = parameters.page;

            if (parameters?.type.Count > 0)
               uri += "&" + GetCommaSeparatedString(parameters.type);

            response = await MakeRequestAsync<TransactionsResponse>(uri);
         }
         else
         {
            uri = ServerUri(EServer.Account) + "accounts/" + accountID + "/transactions/idrange";

            var requestParams = ConvertToDictionary(parameters);
            if (parameters?.type.Count > 0)
               requestParams.Add("type", GetCommaSeparatedString(parameters.type));

            response = await MakeRequestAsync<TransactionsResponse>(uri, "GET", requestParams);
         }

         return response.transactions;
      }

      public class TransactionsByIdRangeParameters
      {
         /// <summary>
         /// The URI that represents the idrange query providing the data for the query results
         /// </summary>
         public string page { get; set; }

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
         [JsonIgnore]
         public List<string> type { get; set; }
      }
   }

   /// <summary>
   /// The requested range of Transactions
   /// http://developer.oanda.com/rest-live-v20/transaction-ep/
   /// </summary>
   public class TransactionsResponse : Response
   {
      /// <summary>
      /// The list of Transactions that satisfy the request.
      /// </summary>
      [JsonConverter(typeof(TransactionConverter))]
      public List<ITransaction> transactions;
   }
}
