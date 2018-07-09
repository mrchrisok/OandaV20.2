﻿using System.Threading.Tasks;

namespace OkonkwoOandaV20.TradeLibrary.REST
{
   public partial class Rest20
   {
      /// <summary>
      /// Gets the details for a specific Trade in an Account
      /// </summary>
      /// <param name="accountID">Account identifier</param>
      /// <param name="tradeSpecifier">Specifier for the Trade</param>
      /// <returns>TradeData object containing the details of the trade</returns>
      public static async Task<Trade.Trade> GetTradeAsync(string accountID, long tradeSpecifier)
      {
         string uri = ServerUri(Server.Account) + "accounts/" + accountID + "/trades/" + tradeSpecifier;

         var response = await MakeRequestAsync<TradeResponse>(uri);

         return response.trade;
      }

      public class TradeResponse : Response
      {
         /// <summary>
         /// The details for the Trade requested
         /// </summary>
         public Trade.Trade trade { get; set; }
      }
   }
}
