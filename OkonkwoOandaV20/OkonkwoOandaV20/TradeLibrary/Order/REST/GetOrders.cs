using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using OkonkwoOandaV20.Framework.JsonConverters;
using OkonkwoOandaV20.TradeLibrary.Order;
using System;
using System.Collections.Generic;
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
      /// Retrieves the list of orders belonging to the account
      /// </summary>
      /// <param name="parameters">the parameters for the request</param>
      /// <param name="cancellation">a cancellation token that can cancel the operation</param>
      /// <returns>a List of Order objects (or empty list, if no orders)</returns>
      public static async Task<OrdersResponse> GetOrdersAsync(OrdersParameters parameters, CancellationToken cancellation = default)
      {
         var requestParams = new HttpParameters(parameters)
         {
            Method = HttpMethod.Get,
            Uri = new Uri(ServerUri(EServer.Account) + $"accounts/{parameters.accountID}/orders"),
            Binding = HttpParametersBinding.QueryString,
         };

         var response = await MakeRequestAsync<OrdersResponse, OrdersErrorResponse>(requestParams, cancellation);

         return response;
      }

      public class OrdersParameters : ApiParameters
      {
         /// <summary>
         /// Account Identifier [required]
         /// </summary>
         [JsonIgnore]
         [Required]
         public string accountID { get; set; }

         /// <summary>
         /// Comma separated list of Order IDs to retrieve
         /// </summary>
         public List<string> ids { get; set; }

         /// <summary>
         /// The state to filter the requested Orders by [default=PENDING]
         /// Valid values are specified in the OrderStateFilter class
         /// </summary>
         public string state { get; set; }

         /// <summary>
         /// The instrument to filter the requested orders by
         /// </summary>
         public string instrument { get; set; }

         /// <summary>
         /// The maximum number of Orders to return [default=50, maximum=500]
         /// </summary>
         public int? count { get; set; }

         /// <summary>
         /// The maximum Order ID to return. If not provided the most recent Orders in the Account are returned
         /// </summary>
         public long? beforeID { get; set; }
      }
   }

   /// <summary>
   /// The success response received from accounts/accountID/orders
   /// </summary>
   public class OrdersResponse : Response
   {
      /// <summary>
      /// The list of Order detail objects
      /// </summary>
      //[JsonConverter(typeof(OrderConverter))]
      public List<IOrder> orders { get; set; }
   }

   /// <summary>
   /// The error response received from accounts/accountID/orders
   /// </summary>
   public class OrdersErrorResponse : ErrorResponse
   {
   }
}
