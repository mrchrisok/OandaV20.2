using OkonkwoOandaV20.TradeLibrary.Order;
using System.Collections.Generic;
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
      public static async Task<List<IOrder>> GetPendingOrdersAsync(string accountID)
      {
         string uri = ServerUri(Server.Account) + "accounts/" + accountID + "/pendingOrders";

         var response = await MakeRequestAsync<OrdersResponse>(uri);

         return new List<IOrder>(response.orders);
      }
   }
}