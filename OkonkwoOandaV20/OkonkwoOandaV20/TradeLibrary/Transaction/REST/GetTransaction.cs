using Newtonsoft.Json;
using OkonkwoOandaV20.Framework.JsonConverters;
using OkonkwoOandaV20.TradeLibrary.Transaction;
using System;
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
      /// Get the details of a single Account Transaction
      /// http://developer.oanda.com/rest-live-v20/transaction-ep/#_collapse_endpoint_3
      /// </summary>
      /// <param name="parameters">the parameters for the request</param>
      /// <param name="cancellation">a cancellation token that can cancel the operation</param>
      /// <returns>A Transaction object with the details of the transaction</returns>
      public static async Task<TransactionResponse> GetTransactionAsync(TransactionParameters parameters, CancellationToken cancellation = default)
      {
         var requestParams = new HttpParameters(parameters)
         {
            Method = HttpMethod.Get,
            Uri = new Uri(ServerUri(EServer.Account) + "accounts/" + parameters.accountID + "/transactions/" + parameters.transactionID)
         };

         var response = await MakeRequestAsync<TransactionResponse, TransactionErrorResponse>(requestParams, cancellation);

         return response;
      }
   }

   public class TransactionParameters : ApiParameters
   {
      /// <summary>
      /// The account ID
      /// </summary>
      [JsonIgnore]
      [Required]
      public string accountID { get; set; }

      /// <summary>
      /// The transaction ID
      /// </summary>
      [JsonIgnore]
      [Required]
      public long transactionID { get; set; }
   }

   /// <summary>
   /// The GET success response received from accounts/accountID/transactions/transactionID
   /// </summary>
   public class TransactionResponse : Response
   {
      /// <summary>
      /// The details of the Transaction requested
      /// </summary>
      //[JsonConverter(typeof(TransactionConverter))]
      public ITransaction transaction { get; set; }
   }

   /// <summary>
   /// The GET error response received from accounts/accountID/transactions/transactionID
   /// </summary>
   public class TransactionErrorResponse : ErrorResponse
   {
   }
}

