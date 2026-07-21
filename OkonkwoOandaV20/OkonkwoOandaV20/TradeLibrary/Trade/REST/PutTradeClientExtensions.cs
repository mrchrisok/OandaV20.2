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
      /// Update the Client Extensions for a Trade. Do not add, update, or delete the Client Extensions if your account is associated with MT4.
      /// </summary>
      /// <param name="parameters">the parameters for the request</param>
      /// <param name="cancellation">a cancellation token that can cancel the operation</param>
      /// <returns>PostOrderResponse with details of the results (throws if if fails)</returns>
      public static async Task<TradeClientExtensionsResponse> PutTradeClientExtensionsAsync(TradeClientExtensionsParameters parameters, CancellationToken cancellation = default)
      {
         var requestParams = new HttpParameters(parameters)
         {
            Method = HttpMethod.Put,
            Uri = new Uri(ServerUri(EServer.Account) + $"accounts/{parameters.accountID}/trades/{parameters.tradeSpecifier}/clientExtensions"),
            Binding = HttpParametersBinding.Body,
         };

         var response = await MakeRequestAsync<TradeClientExtensionsResponse, TradeClientExtensionsErrorResponse>(requestParams, cancellation);

         return response;
      }

      public class TradeClientExtensionsParameters : ApiParameters
      {
         /// <summary>
         /// Account Identifier [required]
         /// </summary>
         [JsonIgnore]
         [Required]
         public string accountID { get; set; }

         /// <summary>
         /// Account Identifier [required]
         /// </summary>
         [JsonIgnore]
         [Required]
         public long tradeSpecifier { get; set; }

         /// <summary>
         /// The Client Extensions to update the Trade with. Do not add, update, or
         /// delete the Client Extensions if your account is associated with MT4.
         /// </summary>
         public ClientExtensions clientExtensions { get; set; }
      }
   }

   /// <summary>
   /// The PUT success response received from accounts/accountID/trades/tradeSpecifier/clientExtensions
   /// </summary>
   public class TradeClientExtensionsResponse : Response
   {
      /// <summary>
      /// The Transaction that updates the Trade’s Client Extensions.
      /// </summary>
      public TradeClientExtensionsModifyTransaction tradeClientExtensionsModifyTransaction { get; set; }
   }

   /// <summary>
   /// The PUT error response received from accounts/accountID/trades/tradeSpecifier/clientExtensions
   /// </summary>
   public class TradeClientExtensionsErrorResponse : ErrorResponse
   {
      /// <summary>
      /// The Transaction that rejects the modification of the Trade’s Client
      /// Extensions.
      /// </summary>
      public TradeClientExtensionsModifyRejectTransaction tradeClientExtensionsModifyRejectTransaction { get; set; }
   }
}
