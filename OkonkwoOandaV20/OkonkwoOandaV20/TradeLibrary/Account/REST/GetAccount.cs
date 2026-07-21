using Newtonsoft.Json;
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
      /// Get the full details for a single Account that a client has access to. Full pending Order, open Trade 
      /// and open Position representations are provided.
      /// http://developer.oanda.com/rest-live-v20/account-ep/#_collapse_endpoint_3
      /// </summary>
      /// <param name="parameters">the parameters for the request</param>
      /// <param name="cancellation">a cancellation token that can cancel the operation</param>
      /// <returns>an Account object containing the account details</returns>
      public static async Task<AccountResponse> GetAccountAsync(AccountParameters parameters, CancellationToken cancellation = default)
      {
         var requestParams = new HttpParameters(parameters)
         {
            Method = HttpMethod.Get,
            Uri = new Uri(ServerUri(EServer.Account) + $"accounts/{parameters.accountID}"),
         };

         var response = await MakeRequestAsync<AccountResponse, AccountErrorResponse>(requestParams, cancellation);

         return response;
      }
   }

   public class AccountParameters : ApiParameters
   {
      /// <summary>
      /// The account id
      /// </summary>
      [JsonIgnore]
      [Required]
      public string accountID { get; set; }
   }


   /// <summary>
   /// The GET success response received from the accounts/accountID
   /// </summary>
   public class AccountResponse : Response
   {
      /// <summary>
      /// The full details of the requested Account.
      /// </summary>
      public Account.Account account { get; set; }
   }

   /// <summary>
   /// The GET error response received from the accounts/accountID
   /// </summary>
   public class AccountErrorResponse : ErrorResponse
   {
   }
}
