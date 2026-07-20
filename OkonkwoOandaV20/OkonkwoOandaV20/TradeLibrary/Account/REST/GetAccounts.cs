using OkonkwoOandaV20.Framework;
using OkonkwoOandaV20.TradeLibrary.Account;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace OkonkwoOandaV20.TradeLibrary.REST
{
   public partial class Rest20
   {
      /// <summary>
      /// Get a list of all Accounts authorized for the provided token.
      /// http://developer.oanda.com/rest-live-v20/account-ep/#_collapse_endpoint_2
      /// </summary>
      /// <returns>a List of AccountProperties that includes basic information about the accounts</returns>
      public static async Task<List<AccountProperties>> GetAccountsAsync(CancellationToken cancellation = default)
      {
         var requestParams = new HttpParameters()
         {
            Method = HttpMethod.Get,
            Uri = new Uri(ServerUri(EServer.Account) + "accounts"),
            ForInternalRequest = true
         };

         var response = await MakeRequestAsync<AccountsResponse, AccountsErrorResponse>(requestParams, cancellation);
         Rest20.TransformObjectValues(response.accounts);
         return response.accounts;
      }
   }

   /// <summary>
   /// The GET success response received from the accounts
   /// </summary>
   public class AccountsResponse : Response
   {
      /// <summary>
      /// The list of Accounts the client is authorized to access and their 
      /// associated properties.
      /// </summary>
      public List<AccountProperties> accounts;
   }

   /// <summary>
   /// The GET error response received from the accounts
   /// </summary>
   public class AccountsErrorResponse : ErrorResponse
   {
   }
}
