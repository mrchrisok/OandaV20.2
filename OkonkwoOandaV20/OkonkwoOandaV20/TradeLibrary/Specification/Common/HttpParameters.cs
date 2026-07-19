using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Net.Http;
using System.Threading;

namespace OkonkwoOandaV20.TradeLibrary.REST
{
   /// <summary>
   /// Container for request parameters and metadata used to build HTTP requests.
   /// Adapted to mirror the HttpParameters pattern used in OkonkwoTradeStationV3.
   /// </summary>
   public class HttpParameters
   {
      /// <summary>
      /// Constructor for creation of an un-initialized HttpParameters object.
      /// </summary>
      public HttpParameters()
      {
      }

      /// <summary>
      /// Constructor for creation of an HttpParameters object using passed in values.
      /// </summary>
      /// <param name="parameters">An arbitrary object that will be serialized into the request payload (body or query string)</param>
      /// <param name="jsonSettingsRequest">Optional serializer settings to use for turning the parameters into JSON</param>
      public HttpParameters(object parameters, JsonSerializerSettings jsonSettingsRequest = null)
      {
         jsonSettingsRequest ??= Rest20.JsonSettingsRequest;
         var jsonSerializer = JsonSerializer.CreateDefault(jsonSettingsRequest);

         Data = parameters == null ? null : JToken.FromObject(parameters, jsonSerializer);
      }

      /// <summary>
      /// The service endpoint address to which the request will be sent.
      /// </summary>
      public Uri Uri { get; set; }

      /// <summary>
      /// Used to indicate the REST verb to apply to the request.
      /// </summary>
      public HttpMethod Method { get; set; } = HttpMethod.Get;

      /// <summary>
      /// Used to indicate the content type expected in the response.
      /// </summary>
      public string ContentType { get; set; } = "application/json";

      /// <summary>
      /// Used to indicate the accept type expected in the response.
      /// </summary>
      public string AcceptType { get; set; } = "application/json";

      /// <summary>
      /// Container for key-value pairs to be conveyed with the request as the data payload.
      /// Stored as a JToken so it can represent both objects and primitive values.
      /// </summary>
      public JToken Data { get; set; }

      /// <summary>
      /// Used to indicate which property of the request will be used to convey the parameters.
      /// </summary>
      public HttpParametersBinding Binding { get; set; } = HttpParametersBinding.QueryString;

      /// <summary>
      /// Used to indicate when completion of the request+response cycle will be triggered to the server.
      /// </summary>
      public HttpCompletionOption CompletionOption { get; set; } = HttpCompletionOption.ResponseHeadersRead;

      /// <summary>
      /// Rate limit category placeholder to mirror TradeStation implementation.
      /// Not currently used by Oanda implementation but kept for parity.
      /// </summary>
      public RateLimitResourceCategory RateLimitResourceCategory { get; set; }

      /// <summary>
      /// Quota for the rate limit window. Not required for current implementation; default left as 500 for parity.
      /// </summary>
      public short RateLimitWindowQuota { get; set; } = 500;

      /// <summary>
      /// Window duration in minutes used by some services. Default is 1 (minute) for parity.
      /// </summary>
      public short RateLimitWindowMinutes { get; set; } = 1;

      /// <summary>
      /// When true, indicates that the resource category enforces concurrent connections (streaming).
      /// </summary>
      public bool RateLimitWindowConcurrent { get; set; } = false;

      /// <summary>
      /// Client systems can supply a cancellation token that will be used to cancel in-flight requests.
      /// </summary>
      public CancellationToken CancellationToken { get; set; } = CancellationToken.None;
   }

   /// <summary>
   /// Used to indicate which property of the request will be used to convey the parameters.
   /// </summary>
   public enum HttpParametersBinding
   {
      /// <summary>
      /// Indicates that request parameters should be added to the request header
      /// </summary>
      Header,

      /// <summary>
      /// Indicates that request parameters should be added to the query string
      /// </summary>
      QueryString,

      /// <summary>
      /// Indicates that request parameters should be added to the request body
      /// </summary>
      Body
   }

   /// <summary>
   /// Placeholder enum for rate limit resource categories. Kept to match TradeStationV3 pattern.
   /// </summary>
   public enum RateLimitResourceCategory
   {
      None,
      Pricing,
      OrderExecution,
      Account,
      Streaming
   }
}
