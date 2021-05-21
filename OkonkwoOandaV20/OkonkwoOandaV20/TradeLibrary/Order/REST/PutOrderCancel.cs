using Newtonsoft.Json;
using OkonkwoOandaV20.TradeLibrary.Transaction;
using System.Collections.ObjectModel;
using System.Threading.Tasks;

namespace OkonkwoOandaV20.TradeLibrary.REST
{
   public partial class Rest20
   {
	  /// <summary>
	  /// Cancel a pending order in an Account
	  /// http://developer.oanda.com/rest-live-v20/order-ep/#_collapse_endpoint_7
	  /// </summary>
	  /// <param name="accountID">the identifier of the account to post on</param>
	  /// <param name="orderSpecifier">The Order Specifier (orderId) [required]</param>
	  /// <returns>a PutCancelOrderResponse with details of the cancelled order</returns>
	  public static async Task<OrderCancelResponse> PutOrderCancelAsync(string accountID, long orderSpecifier, OrderCancelParameters parameters = null)
	  {
		 parameters ??= new OrderCancelParameters();

		 var request = new OrderCancelRequest()
		 {
			Uri = $"{ServerUri(EServer.Account)}accounts/{accountID}/orders/{orderSpecifier}/cancel",
			Method = "PUT",
			Parameters = parameters
		 };

		 var response = await MakeRequestAsync<OrderCancelResponse, OrderCancelErrorResponse>(request);

		 return response;
	  }

	  public class OrderCancelParameters : Parameters
	  {
		 /// <summary>
		 /// Client specified RequestID to be sent with request.
		 /// </summary>
		 [JsonIgnore]
		 public string ClientRequestID { get; set; }

		 [JsonIgnore]
		 public override ReadOnlyDictionary<string, string> Headers
		 {
			get
			{
			   if (!string.IsNullOrWhiteSpace(ClientRequestID))
			   {
				  _headers["ClientRequestID"] = ClientRequestID;
			   }

			   return new ReadOnlyDictionary<string, string>(base.Headers);
			}
		 }
	  }
   }

   public class OrderCancelRequest : Request
   {
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