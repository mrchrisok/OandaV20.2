using Newtonsoft.Json;
using OkonkwoOandaV20.TradeLibrary.Transaction;
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
      /// Cancel a pending order in an Account
      /// http://developer.oanda.com/rest-live-v20/order-ep/#_collapse_endpoint_7
      /// </summary>
      /// <param name="parameters">the parameters for the request</param>
      /// <param name="cancellation">a cancellation token that can cancel the operation</param>
      /// <returns>a PutCancelOrderResponse with details of the cancelled order</returns>
      public static async Task<OrderCancelResponse> PutOrderCancelAsync(OrderCancelParameters parameters, CancellationToken cancellation = default)
      {
         var requestParams = new HttpParameters(parameters)
         {
            Method = HttpMethod.Put,
            Uri = new Uri(ServerUri(EServer.Account) + $"accounts/{parameters.accountID}/orders/{parameters.orderSpecifier}/cancel"),
         };

         var response = await MakeRequestAsync<OrderCancelResponse, OrderCancelErrorResponse>(requestParams, cancellation);

         return response;
      }
   }

   public class OrderCancelParameters : ApiParameters
   {
      /// <summary>
      /// Account Identifier [required]
      /// </summary>
      [JsonIgnore]
      [Required]
      public string accountID { get; set; }

      /// <summary>
      /// The order ID
      /// </summary>
      [JsonIgnore]
      [Required]
      public long orderSpecifier { get; set; }
   }

   /// <summary>
   /// The PUT response received from accounts/accountID/orders/orderSpecifier/cancel
   /// </summary>
   public class OrderCancelResponse : Response
   {
      /// <summary>
      /// The Transaction that cancelled the Order
      /// </summary>
      public OrderCancelTransaction orderCancelTransaction { get; set; }
   }

   /// <summary>
   /// The PUT error response received from accounts/accountID/orders/orderSpecifier/cancel
   /// </summary>
   public class OrderCancelErrorResponse : ErrorResponse
   {
      /// <summary>
      /// The Transaction that rejected the cancellation of the Order. Only present
      /// if the Account exists.
      /// </summary>
      public OrderCancelRejectTransaction orderCancelRejectTransaction { get; set; }
   }
}
