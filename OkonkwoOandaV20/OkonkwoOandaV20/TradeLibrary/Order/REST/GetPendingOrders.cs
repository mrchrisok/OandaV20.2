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
	  public static async Task<List<IOrder>> GetPendingOrdersAsync(string accountID, PendingOrdersParameters parameters = null)
	  {
		 parameters ??= new PendingOrdersParameters();

		 var request = new PendingOrdersRequest()
		 {
			Uri = $"{ServerUri(EServer.Account)}accounts/{accountID}/pendingOrders",
			Method = "GET",
			Parameters = parameters
		 };

		 var response = await MakeRequestAsync<PendingOrdersResponse, PendingOrdersErrorResponse>(request);

		 return new List<IOrder>(response.orders);
	  }

	  public class PendingOrdersParameters : Parameters
	  {
	  }
   }

   public class PendingOrdersRequest : Request
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