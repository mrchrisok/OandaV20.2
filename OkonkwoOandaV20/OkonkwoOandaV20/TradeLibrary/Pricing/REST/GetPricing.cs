using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

using OkonkwoOandaV20.TradeLibrary.Pricing;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace OkonkwoOandaV20.TradeLibrary.REST
{
   public partial class Rest20
   {
      /// <summary>
      /// Get pricing information for a specified list of instrumentes within an Account
      /// http://developer.oanda.com/rest-live-v20/pricing-ep/#collapse_endpoint_2
      /// </summary>
      /// <param name="parameters">the parameters for the request</param>
      /// <param name="cancellation">a cancellation token that can cancel the operation</param>
      /// <returns></returns>
      public static async Task<PricingResponse> GetPricingAsync(PricingParameters parameters, CancellationToken cancellation = default)
      {
         //var requestDict = new Dictionary<string, string>(ConvertToDictionary(parameters))
         //{
         //   { "instruments", GetCommaSeparatedString(parameters.instruments ?? new List<string>()) }
         //};

         var requestParams = new HttpParameters(parameters)
         {
            Method = HttpMethod.Get,
            Uri = new Uri(ServerUri(EServer.Account) + $"accounts/{parameters.accountID}/pricing"),
            Binding = HttpParametersBinding.QueryString,
         };

         var response = await MakeRequestAsync<PricingResponse, PricingErrorResponse>(requestParams, cancellation);

         return response;
      }

      public class PricingParameters : ApiParameters
      {
         /// <summary>
         /// The account ID
         /// </summary>
         public string accountID { get; set; }

         /// <summary>
         /// List of Instruments to get pricing for. [required]
         /// </summary>
         [Required]
         public List<string> instruments { get; set; }

         /// <summary>
         /// Date/Time filter to apply to the response. Only prices and home conversions (if requested) with a 
         /// time later than this filter (i.e. the price has changed after the since time) will be provided, 
         /// and are filtered independently.
         /// </summary>
         public string since { get; set; }

         /// <summary>
         /// Flag that enables the inclusion of the unitsAvailable field in the returned Price objects. [default=True]
         /// </summary>
         [Obsolete("Deprecated: Wil be removed in a future API update.")]
         public bool? includeUnitsAvailable { get; set; }

         /// <summary>
         /// Flag that enables the inclusion of the homeConversions field in the returned response. 
         /// An entry will be returned for each currency in the set of all base and quote currencies present 
         /// in the requested instruments list. [default=False]
         /// </summary>
         public bool? includeHomeConversions { get; set; }
      }
   }

   /// <summary>
   /// The GET success response received from accounts/accountID/pricing
   /// </summary>
   public class PricingResponse : Response
   {
      /// <summary>
      /// The list of Price objects requested.
      /// </summary>
      public List<Price> prices;

      /// <summary>
      /// The list of home currency conversion factors requested. This field will
      /// only be present if includeHomeConversions was set to true in the request.
      /// </summary>
      public List<HomeConversions> homeConversions;

      /// <summary>
      /// The DateTime value to use for the “since” parameter in the next poll request.
      /// </summary>
      public string time;
   }

   /// <summary>
   /// The GET error response received from accounts/accountID/pricing
   /// </summary>
   public class PricingErrorResponse : ErrorResponse
   {
   }
}
