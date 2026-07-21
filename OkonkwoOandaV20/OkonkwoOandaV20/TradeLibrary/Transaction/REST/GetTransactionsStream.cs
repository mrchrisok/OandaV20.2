using Newtonsoft.Json;
using OkonkwoOandaV20.Framework.JsonConverters;
using OkonkwoOandaV20.TradeLibrary.REST.Streaming;
using OkonkwoOandaV20.TradeLibrary.Transaction;
using System;
using System.ComponentModel.DataAnnotations;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading;
using System.Threading.Tasks;

namespace OkonkwoOandaV20.TradeLibrary.REST
{
   public partial class Rest20
   {
      /// <summary>
      /// Get a stream of Transactions for an Account starting from when the request is made
      /// </summary>
      /// <param name="parameters">the parameters for the request</param>
      /// <param name="cancellation">a cancellation token that can cancel the operation</param>
      /// <returns>the HttpResponseMessage that can be used to retrieve the events as they stream</returns>
      public static async Task<HttpResponseMessage> GetTransactionsStream(TransactionsStreamParameters parameters, CancellationToken cancellation = default)
      {
         var requestParameters = new HttpParameters(parameters)
         {
            Method = HttpMethod.Get,
            Uri = new Uri(ServerUri(EServer.TransactionsStream) + $"accounts/{parameters.accountID}/transactions/stream"),
         };

         return await MakeStreamRequestAsync<TransactionsStreamErrorResponse>(requestParameters, cancellation);
      }
   }

   public class TransactionsStreamParameters : ApiParameters
   {
      /// <summary>
      /// Account Identifier [required]
      /// </summary>
      [JsonIgnore]
      [Required]
      public string accountID { get; set; }
   }

   //[JsonConverter(typeof(TransactionsStreamResponseConverter))]
   public class TransactionsStreamResponse : StreamResponse
   {
      /// <summary>
      /// The most recent Transaction completed for the Account
      /// </summary>
      public ITransaction transaction { get; set; }
   }

   public class TransactionsStreamErrorResponse : ErrorResponse
   {
   }

   public class TransactionsHeartbeat : Heartbeat
   {
      /// <summary>
      /// The ID of the most recent Transaction created for the Account
      /// </summary>
      public long? lastTransactionID { get; set; }
   }

   public class TransactionsSession : StreamSession<TransactionsStreamResponse>
   {
      public TransactionsSession(string accountID) : base(accountID)
      {
      }

      protected override async Task<HttpResponseMessage> GetSession()
      {
         return await Rest20.GetTransactionsStream(new TransactionsStreamParameters() { 
            accountID = _accountID 
         });
      }
   }
}
