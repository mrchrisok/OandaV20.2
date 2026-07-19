using Newtonsoft.Json;
using OkonkwoOandaV20.Framework.JsonConverters;
using OkonkwoOandaV20.TradeLibrary.REST.Streaming;
using OkonkwoOandaV20.TradeLibrary.Transaction;
using System;
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
      /// <param name="accountID">the account ID you want to stream on</param>
      /// <returns>the HttpResponseMessage that can be used to retrieve the events as they stream</returns>
      public static async Task<HttpResponseMessage> GetTransactionsStream(string accountID, CancellationToken cancellation = default)
      {
         var parameters = new HttpParameters()
         {
            Method = HttpMethod.Get,
            Uri = new Uri(ServerUri(EServer.TransactionsStream) + $"accounts/{accountID}/transactions/stream"),
         };

         return await MakeStreamRequestAsync<TransactionsStreamErrorResponse>(parameters, cancellation);
      }
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
         return await Rest20.GetTransactionsStream(_accountID);
      }
   }
}
