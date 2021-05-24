using Newtonsoft.Json;
using OkonkwoOandaV20.TradeLibrary.REST;

namespace OkonkwoOandaV20.Framework.Factories
{
   public class ErrorResponseFactory
   {
	  public static IErrorResponse Create(string message)
	  {
		 var dynamicError = JsonConvert.DeserializeObject<dynamic>(message);

		 // the 'type' property is injected by the Rest20.GetWebResponse catch block

		 switch (dynamicError.type.ToString())
		 {
			case nameof(AccountChangesErrorResponse):
			   return JsonConvert.DeserializeObject<AccountChangesErrorResponse>(message);
			case nameof(AccountConfigurationErrorResponse):
			   return JsonConvert.DeserializeObject<AccountConfigurationErrorResponse>(message);
			case nameof(AccountErrorResponse):
			   return JsonConvert.DeserializeObject<AccountErrorResponse>(message);
			case nameof(AccountsErrorResponse):
			   return JsonConvert.DeserializeObject<AccountsErrorResponse>(message);
			case nameof(AccountInstrumentsErrorResponse):
			   return JsonConvert.DeserializeObject<AccountInstrumentsErrorResponse>(message);
			case nameof(AccountSummaryErrorResponse):
			   return JsonConvert.DeserializeObject<AccountSummaryErrorResponse>(message);
			case nameof(InstrumentCandlesErrorResponse):
			   return JsonConvert.DeserializeObject<InstrumentCandlesErrorResponse>(message);
			case nameof(InstrumentOrderBookErrorResponse):
			   return JsonConvert.DeserializeObject<InstrumentOrderBookErrorResponse>(message);
			case nameof(InstrumentPositionBookErrorResponse):
			   return JsonConvert.DeserializeObject<InstrumentPositionBookErrorResponse>(message);
			case nameof(OpenPositionsErrorResponse):
			   return JsonConvert.DeserializeObject<OpenPositionsErrorResponse>(message);
			case nameof(OpenTradesErrorResponse):
			   return JsonConvert.DeserializeObject<OpenTradesErrorResponse>(message);
			case nameof(OrderCancelErrorResponse):
			   return JsonConvert.DeserializeObject<OrderCancelErrorResponse>(message);
			case nameof(OrderReplaceErrorResponse):
			   return JsonConvert.DeserializeObject<OrderReplaceErrorResponse>(message);
			case nameof(OrderClientExtensionsErrorResponse):
			   return JsonConvert.DeserializeObject<OrderClientExtensionsErrorResponse>(message);
			case nameof(OrderErrorResponse):
			   return JsonConvert.DeserializeObject<OrderErrorResponse>(message);
			case nameof(OrdersErrorResponse):
			   return JsonConvert.DeserializeObject<OrdersErrorResponse>(message);
			case nameof(PendingOrdersErrorResponse):
			   return JsonConvert.DeserializeObject<PendingOrdersErrorResponse>(message);
			case nameof(PositionErrorResponse):
			   return JsonConvert.DeserializeObject<PositionErrorResponse>(message);
			case nameof(PositionsErrorResponse):
			   return JsonConvert.DeserializeObject<PositionsErrorResponse>(message);
			case nameof(PostOrderErrorResponse):
			   return JsonConvert.DeserializeObject<PostOrderErrorResponse>(message);
			case nameof(PositionCloseErrorResponse):
			   return JsonConvert.DeserializeObject<PositionCloseErrorResponse>(message);
			case nameof(PricingCandlesErrorResponse):
			   return JsonConvert.DeserializeObject<PricingCandlesErrorResponse>(message);
			case nameof(PricingErrorResponse):
			   return JsonConvert.DeserializeObject<PricingErrorResponse>(message);
			case nameof(PricingInstrumentCandlesErrorResponse):
			   return JsonConvert.DeserializeObject<PricingInstrumentCandlesErrorResponse>(message);
			case nameof(TradeErrorResponse):
			   return JsonConvert.DeserializeObject<TradeErrorResponse>(message);
			case nameof(TradesErrorResponse):
			   return JsonConvert.DeserializeObject<TradesErrorResponse>(message);
			case nameof(TradeClientExtensionsErrorResponse):
			   return JsonConvert.DeserializeObject<TradeClientExtensionsErrorResponse>(message);
			case nameof(TradeCloseErrorResponse):
			   return JsonConvert.DeserializeObject<TradeCloseErrorResponse>(message);
			case nameof(TradeOrdersErrorResponse):
			   return JsonConvert.DeserializeObject<TradeOrdersErrorResponse>(message);
			case nameof(TransactionErrorResponse):
			   return JsonConvert.DeserializeObject<TransactionErrorResponse>(message);
			case nameof(TransactionsByIdRangeErrorResponse):
			   return JsonConvert.DeserializeObject<TransactionsByIdRangeErrorResponse>(message);
			case nameof(TransactionPagesErrorResponse):
			   return JsonConvert.DeserializeObject<TransactionPagesErrorResponse>(message);
			case nameof(TransactionsSinceIdErrorResponse):
			   return JsonConvert.DeserializeObject<TransactionsSinceIdErrorResponse>(message);

			default:
			   return JsonConvert.DeserializeObject<ErrorResponse>(message);
		 }
	  }
   }
}
