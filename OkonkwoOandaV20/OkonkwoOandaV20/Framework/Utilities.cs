using Azure.Core;
using Azure.Identity;
using Microsoft.Identity.Client;
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
