using OkonkwoOandaV20.Framework;
using OkonkwoOandaV20.TradeLibrary.Account;
using System;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace OkonkwoOandaV20.TradeLibrary.REST
{
   public partial class Rest20
   {
      /// <summary>
      /// Get a summary for a single Account that a client has access to.
      /// http://developer.oanda.com/rest-live-v20/account-ep/#_collapse_endpoint_4
      /// </summary>
      /// <param name="accountID">summary will be retrieved for this account id</param>
      /// <returns>an AccountSummary object containing the account details</returns>
      public static async Task<AccountSummaryResponse> GetAccountSummaryAsync(AccountSummaryParameters parameters, CancellationToken cancellation = default)
      {
         var requestParams = new HttpParameters(parameters)
         {
            Method = HttpMethod.Get,
            Uri = new Uri(ServerUri(EServer.Account) + $"accounts/{parameters.accountID}/summary"),
         };

         var response = await MakeRequestAsync<AccountSummaryResponse, AccountSummaryErrorResponse>(requestParams, cancellation);

         return response;
      }
   }

   public class AccountSummaryParameters: AccountParameters
   {

   }

   /// <summary>
   /// The GET success response received from the accounts/accountID/summary
   /// </summary>
   public class AccountSummaryResponse : Response
   {
      /// <summary>
      /// The summary of the requested Account.
      /// </summary>
      public AccountSummary account { get; set; }
   }

   /// <summary>
   /// The GET error response received from the accounts/accountID/summary
   /// </summary>
   public class AccountSummaryErrorResponse : ErrorResponse
   {
   }
}
