using Azure.Core;
using Azure.Identity;
using Newtonsoft.Json.Linq;
using OkonkwoOandaV20.TradeLibrary.REST;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using static OkonkwoOandaV20.TradeLibrary.REST.Rest20;

namespace OkonkwoOandaV20.Framework
{
   public static class Utilities
   {
      /// <summary>
      /// Creates a query string from a JObject
      /// </summary>
      /// <param name="requestParamsObject">The parameters JObject to convert to a query string.</param>
      /// <returns>A query string</returns>

      internal static string ConvertToQueryString(JObject requestParamsObject)
      {
         var queryString = new StringBuilder();

         foreach (var kvPair in requestParamsObject)
         {
            string key = WebUtility.UrlEncode(kvPair.Key);

            string value = kvPair.Value is JArray array
               ? array.Select(x => x.ToString()).ToDistinctCSV()
               : kvPair.Value.ToString();

            queryString.Append($"{key}={WebUtility.UrlEncode(value)}&");
         }

         return queryString.ToString().TrimEnd('&');
      }

      /// <summary>
      /// Determines if trading is halted for the provided instrument.
      /// </summary>
      /// <param name="instrument">Instrument to check if halted. Default is EUR_USD.</param>
      /// <returns>True if trading is halted, false if trading is not halted.</returns>
      public static async Task<bool> IsMarketHalted(string instrument = InstrumentName.Currency.EURUSD, bool throwIfHalted = false)
      {
         var parameters = new PricingParameters() { instruments = new List<string>() { instrument }
            , ForInternalRequest = true };

         var accountId = Credentials.GetCredentials().AccountId;
         var prices = await Rest20.GetPricingAsync(accountId, parameters);

         bool isTradeable = false, hasBids = false, hasAsks = false;

         if (prices[0] != null)
         {
            isTradeable = prices[0].tradeable;
            hasBids = prices[0].bids.Count > 0;
            hasAsks = prices[0].asks.Count > 0;
         }

         if (isTradeable && hasBids && hasAsks)  // not halted
            return false;

         if (throwIfHalted)
            throw new MarketHaltedException($"Market is halted for this instrument: {instrument}");

         return true;
      }

      public static TokenCredential GetAzureCredential(bool isLocalEnvironment = true)
      {
         var credentialOptions = new DefaultAzureCredentialOptions
         {
            ExcludeManagedIdentityCredential = isLocalEnvironment
         };
         return new DefaultAzureCredential(credentialOptions);
      }

      internal static string ToDistinctCSV(this IEnumerable<string> list)
      {
         return string.Join(",", list.Distinct());
      }

      internal static string ToDistinctCSV(this IEnumerable<long> list)
      {
         return string.Join(",", list.Distinct());
      }
   }
}
