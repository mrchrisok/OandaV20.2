using Newtonsoft.Json;
using OkonkwoOandaV20.Framework.JsonConverters;
using OkonkwoOandaV20.TradeLibrary.Order;
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
      /// <param name="accountID">the identifier of the account to retrieve the list for</param>
      /// <param name="orderSpecifier">The Order Specifier (orderId) [required]</param>
      /// <returns>a List of Order objects (or empty list, if no orders)</returns>
      public static async Task<IOrder> GetOrdersAsync(string accountID, long orderSpecifier)
      {
         string uri = ServerUri(Server.Account) + "accounts/" + accountID + "/orders/" + orderSpecifier;

         var response = await MakeRequestAsync<OrderResponse>(uri);

         return response.order;
      }
   }

   public class OrderResponse : Response
   {
      [JsonConverter(typeof(OrderConverter))]
      public IOrder order { get; set; }
   }
}