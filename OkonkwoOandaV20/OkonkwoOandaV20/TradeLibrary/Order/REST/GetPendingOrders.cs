using OkonkwoOandaV20.TradeLibrary.Order;
using System;
using System.Collections.Generic;
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
      /// Retrieves the list of pending orders belonging to the account
      /// </summary>
      /// <param name="accountID">the identifier of the account to retrieve the list for</param>
      /// <returns>a List of Order objects (or empty list, if no orders)</returns>
      public static async Task<PendingOrdersResponse> GetPendingOrdersAsync(PendingOrdersParameters parameters, CancellationToken cancellation = default)
      {
         var requestParams = new HttpParameters()
         {
            Method = HttpMethod.Get,
            Uri = new Uri(ServerUri(EServer.Account) + "accounts/" + parameters.accountID + "/pendingOrders"),
         };

         var response = await MakeRequestAsync<PendingOrdersResponse, PendingOrdersErrorResponse>(requestParams, cancellation);

         return response;
      }
   }

   public class PendingOrdersParameters : OrderParameters_
   {

   }

   /// <summary>
   /// The GET success response received from accounts/accountID/pendingOrders
   /// </summary>
   public class PendingOrdersResponse : OrdersResponse
   {
   }

   /// <summary>
   /// The GET error response received from accounts/accountID/pendingOrders
   /// </summary>
   public class PendingOrdersErrorResponse : OrdersErrorResponse
   {
   }
}