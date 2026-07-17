using Newtonsoft.Json;
using OkonkwoOandaV20.Framework.JsonConverters;
using OkonkwoOandaV20.TradeLibrary.REST.Streaming;
using OkonkwoOandaV20.TradeLibrary.Transaction;
using System;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;

namespace OkonkwoOandaV20.TradeLibrary.REST
{
   public partial class Rest20
   {
      /// <summary>
      /// Get a stream of Transactions for an Account starting from when the request is made
      /// </summary>
      /// <param name="accountID">the account ID you want to stream on</param>
      /// <returns>the WebResponse object that can be used to retrieve the events as they stream</returns>
      public static async Task<HttpResponseMessage> GetTransactionsStream(string accountID)
      {
         string uri = ServerUri(EServer.TransactionsStream) + "accounts/" + accountID + "/transactions/stream";

         var request = new HttpRequestMessage(new HttpMethod("GET"), uri);
         //
         request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", AccessToken);
         request.Headers.Accept.Add(MediaTypeWithQualityHeaderValue.Parse("application/json"));
         request.Headers.AcceptEncoding.Add(StringWithQualityHeaderValue.Parse("gzip"));
         request.Headers.AcceptEncoding.Add(StringWithQualityHeaderValue.Parse("deflate"));

         try
         {
            HttpResponseMessage response = await _streamsClient.SendAsync(request, HttpCompletionOption.ResponseHeadersRead);
            return response;
         }
         catch (HttpRequestException ex)
         {
            throw ex;
         }
      }
   }


   /// <summary>
   /// Events are authorized transactions posted to the subject account.
   /// http://developer.oanda.com/rest-live-v20/transaction-ep/#collapse_endpoint_6
   /// </summary>
   //[JsonConverter(typeof(TransactionsStreamResponseConverter))]
   public class TransactionsStreamResponse : StreamResponse
   {
      /// <summary>
      /// The most recent Transaction completed for the Account
      /// </summary>
      public ITransaction transaction { get; set; }
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
