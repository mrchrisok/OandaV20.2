using Newtonsoft.Json;
using OkonkwoOandaV20.Framework.JsonConverters;
using OkonkwoOandaV20.TradeLibrary.REST.OrderRequests;
using OkonkwoOandaV20.TradeLibrary.Transaction;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace OkonkwoOandaV20.TradeLibrary.REST
{
   public partial class Rest20
   {
      /// <summary>
      /// Posts an order on the given account with the given parameters
      /// </summary>
      /// <param name="parameters">the parameters for the request</param>
      /// <param name="cancellation">a cancellation token that can cancel the operation</param>
      /// <returns>PostOrderResponse with details of the results (throws if if fails)</returns>
      public static async Task<PostOrderResponse> PostOrderAsync(PostOrderParameters parameters, CancellationToken cancellation = default)
      {
         var requestParams = new HttpParameters(parameters)
         {
            Method = HttpMethod.Post,
            Uri = new Uri(ServerUri(EServer.Account) + $"accounts/{parameters.accountID}/orders"),
            Binding = HttpParametersBinding.Body,
         };

         var response = await MakeRequestAsync<PostOrderResponse, PostOrderErrorResponse>(requestParams, cancellation);

         return response;
      }
   }

   public class PostOrderParameters : ApiParameters
   {
      /// <summary>
      /// The account ID
      /// </summary>
      [JsonIgnore]
      [Required]
      public string accountID { get; set; }

      /// <summary>
      /// the order request to post
      /// </summary>
      public IOrderRequest order { get; set; }
   }

   /// <summary>
   /// The POST success response received from accounts/accountID/orders
   /// </summary>
   public class PostOrderResponse : Response
   {
      /// <summary>
      /// The Transaction that created the Order specified by the request.
      /// </summary>
      [JsonConverter(typeof(TransactionConverter))]
      public ITransaction orderCreateTransaction { get; set; }

      /// <summary>
      /// The Transaction that filled the newly created Order. Only provided when 
      /// the Order was immediately filled.
      /// </summary>
      public OrderFillTransaction orderFillTransaction { get; set; }

      /// <summary>
      /// The Transaction that cancelled the newly created Order. Only provided
      /// when the Order was immediately cancelled.
      /// </summary>
      public OrderCancelTransaction orderCancelTransaction { get; set; }

      /// <summary>
      /// The Transaction that reissues the Order. Only provided when the Order is
      /// configured to be reissued for its remaining units after a partial fill
      /// and the reissue was successful.
      /// </summary>
      public Transaction.Transaction orderReissueTransaction { get; set; }

      /// <summary>
      /// The Transaction that rejects the reissue of the Order. Only provided when
      /// the Order is configured to be reissued for its remaining units after a
      /// partial fill and the reissue was rejected.
      /// </summary>
      public Transaction.Transaction orderReissueRejectTransaction { get; set; }
   }

   /// <summary>
   /// The POST error response received from accounts/accountID/orders
   /// </summary>
   public class PostOrderErrorResponse : ErrorResponse
   {
      /// <summary>
      /// The Transaction that rejected the creation of the Order as requested
      /// </summary>
      [JsonConverter(typeof(TransactionConverter))]
      public ITransaction orderRejectTransaction { get; set; }
   }
}
