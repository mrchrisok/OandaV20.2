using Newtonsoft.Json;
using OkonkwoOandaV20.TradeLibrary.Order;
using System;
using System.ComponentModel.DataAnnotations;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace OkonkwoOandaV20.TradeLibrary.REST
{
   /// <summary>
   /// http://developer.oanda.com/rest-live-v20/account-ep/
   /// </summary>
   public partial class Rest20
   {
      /// <summary>
      /// Retrieves an order belonging to the account
      /// </summary>
      /// <param name="parameters">the parameters for the request</param>
      /// <param name="cancellation">a cancellation token that can cancel the operation</param>
      /// <returns>a List of Order objects (or empty list, if no orders)</returns>
      public static async Task<OrderResponse> GetOrderAsync(OrderParameters parameters, CancellationToken cancellation = default)
      {
         var requestParams = new HttpParameters(parameters)
         {
            Method = HttpMethod.Get,
            Uri = new Uri(ServerUri(EServer.Account) + $"accounts/{parameters.accountID}/orders/{parameters.orderSpecifier}"),
         };

         var response = await MakeRequestAsync<OrderResponse, OrderErrorResponse>(requestParams, cancellation);

         return response;
      }
   }

   public class OrderParameters : ApiParameters
   {
      /// <summary>
      /// The account ID
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
   /// The GET success response received from accounts/accountID/orders/orderSpecifier
   /// </summary>
   public class OrderResponse : Response
   {
      //[JsonConverter(typeof(OrderConverter))]
      public IOrder order { get; set; }
   }

   /// <summary>
   /// The GET error response received accounts/accountID/orders/orderSpecifier
   /// </summary>
   public class OrderErrorResponse : ErrorResponse
   {
   }
}