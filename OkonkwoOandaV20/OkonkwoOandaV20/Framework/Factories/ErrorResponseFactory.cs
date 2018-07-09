using Newtonsoft.Json;
using OkonkwoOandaV20.TradeLibrary.REST;
using static OkonkwoOandaV20.TradeLibrary.REST.Rest20;

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
            case "AccountConfigurationErrorResponse":
               return JsonConvert.DeserializeObject<AccountConfigurationErrorResponse>(message);
            case "OrderCancelErrorResponse":
               return JsonConvert.DeserializeObject<OrderCancelErrorResponse>(message);
            case "OrderReplaceErrorResponse":
               return JsonConvert.DeserializeObject<OrderReplaceErrorResponse>(message);
            case "OrderClientExtensionsErrorResponse":
               return JsonConvert.DeserializeObject<OrderClientExtensionsErrorResponse>(message);
            case "PostOrderErrorResponse":
               return JsonConvert.DeserializeObject<PostOrderErrorResponse>(message);
            case "PositionCloseErrorResponse":
               return JsonConvert.DeserializeObject<PositionCloseErrorResponse>(message);
            case "TradeClientExtensionsErrorResponse":
               return JsonConvert.DeserializeObject<TradeClientExtensionsErrorResponse>(message);
            case "TradeCloseErrorResponse":
               return JsonConvert.DeserializeObject<TradeCloseErrorResponse>(message);
            case "TradeOrdersErrorResponse":
               return JsonConvert.DeserializeObject<TradeOrdersErrorResponse>(message);
            default:
               return JsonConvert.DeserializeObject<ErrorResponse>(message);
         }
      }
   }
}
