using OkonkwoOandaV20.TradeLibrary.Transaction;
using System.Threading;
using System.Threading.Tasks;
using System.Net.Http;

namespace OkonkwoOandaV20.TradeLibrary.REST
{
   public partial class Rest20
   {
      public static async Task<HttpResponseMessage> GetTransactionsStream(string accountID)
      {
         return await GetTransactionsStream(accountID, CancellationToken.None);
      }

      public static async Task<HttpResponseMessage> GetPricingStream(string accountID, PricingStreamParameters parameters)
      {
         return await GetPricingStream(accountID, parameters, CancellationToken.None);
      }
   }
}
