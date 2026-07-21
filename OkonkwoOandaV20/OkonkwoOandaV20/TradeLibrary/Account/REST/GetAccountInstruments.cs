using Newtonsoft.Json.Linq;
using OkonkwoOandaV20.Framework;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text.Json.Serialization;
using System.Threading;
using System.Threading.Tasks;

namespace OkonkwoOandaV20.TradeLibrary.REST
{
   public partial class Rest20
   {
      /// <summary>
      /// Get the list of tradeable instruments for the given Account. The list of tradeable instruments is dependent 
      /// on the regulatory division that the Account is located in, thus should be the same for all Accounts owned by 
      /// a single user.
      /// http://developer.oanda.com/rest-live-v20/account-ep/#_collapse_endpoint_5
      /// </summary>
      /// <param name="parameters">the parameters for the request</param>
      /// <param name="cancellation">a cancellation token that can cancel the operation</param>
      /// <returns>a List of the tradeable instruments specified. If none are specified, all tradeable instruments for 
      /// the account are returned.</returns>
      public static async Task<AccountInstrumentsResponse> GetAccountInstrumentsAsync(AccountInstrumentsParameters parameters = null, CancellationToken cancellation = default)
      {
         var requestParams = new HttpParameters(parameters ?? new AccountInstrumentsParameters())
         {
            Method = HttpMethod.Get,
            Uri = new Uri(ServerUri(EServer.Account) + $"accounts/{parameters.accountID}/instruments"),
            Binding = HttpParametersBinding.QueryString,
         };

         var response = await MakeRequestAsync
            <AccountInstrumentsResponse, AccountInstrumentsErrorResponse>(requestParams, cancellation);

         return response;
      }

      public class AccountInstrumentsParameters : AccountParameters
      {
         /// <summary>
         /// List of instruments to query specifically.
         /// </summary>
         public List<string> instruments { get; set; }
      }
   }

   /// <summary>
   /// The GET success response received from the accounts/accountID/instruments
   /// </summary>
   public class AccountInstrumentsResponse : Response
   {
      /// <summary>
      /// The requested list of instruments.
      /// </summary>
      public List<Instrument.Instrument> instruments { get; set; }
   }

   /// <summary>
   /// The GET error response received from the accounts/accountID/instruments
   /// </summary>
   public class AccountInstrumentsErrorResponse : ErrorResponse
   {
   }
}
