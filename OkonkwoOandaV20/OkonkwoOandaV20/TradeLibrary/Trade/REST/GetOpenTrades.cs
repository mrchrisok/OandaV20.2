using OkonkwoOandaV20.TradeLibrary.DataTypes.Trade;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace OkonkwoOandaV20.TradeLibrary.REST
{
   public partial class Rest20
   {
      /// <summary>
      /// Get a list of open Trades for an Account
      /// </summary>
      /// <param name="accountID">Account identifier</param>
      /// <returns>A list of TradeData objects (or empty list, if no trades)</returns>
      public static async Task<List<Trade>> GetOpenTradesAsync(string accountID)
      {
         string uri = ServerUri(Server.Account) + "accounts/" + accountID + "/openTrades";

         var response = await MakeRequestAsync<TradesResponse>(uri);

         return response.trades ?? new List<Trade>();
      }
   }
}
