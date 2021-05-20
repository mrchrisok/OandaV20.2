﻿using System;
using System.Collections.Generic;
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
	  /// <param name="accountID">details will be retrieved for this account id</param>
	  /// <param name="parameters">the parameters for the request</param>
	  /// <returns>a List of the tradeable instruments specified. If none are specified, all tradeable instruments for 
	  /// the account are returned.</returns>
	  public static async Task<List<Instrument.Instrument>> GetAccountInstrumentsAsync(string accountID, AccountInstrumentsParameters parameters = null)
	  {
		 parameters.AcceptDatetimeFormat = null;

		 string uri = ServerUri(EServer.Account) + "accounts/" + accountID + "/instruments";

		 if (parameters?.instruments?.Count > 0)
		 {
			string commaSeparatedInstruments = GetCommaSeparatedString(parameters.instruments);
			uri += "?instruments=" + Uri.EscapeDataString(commaSeparatedInstruments);
		 }

		 var request = new AccountInstrumentsRequest()
		 {
			Uri = uri,
			Method = "GET",
			Parameters = parameters
		 };

		 var response = await MakeRequestAsync<AccountInstrumentsResponse, AccountInstrumentsErrorResponse>(request);
		 return response.instruments;
	  }

	  public class AccountInstrumentsParameters : Parameters
	  {
		 /// <summary>
		 /// List of instruments to query specifically.
		 /// </summary>
		 public List<string> instruments { get; set; }
	  }
   }

   public class AccountInstrumentsRequest : Request
   {
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