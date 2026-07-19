using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading;
using Newtonsoft.Json.Linq;
using OkonkwoOandaV20.TradeLibrary.REST;
using OkonkwoOandaV20.Framework;

namespace OkonkwoOandaV20.TradeLibrary.Common
{
   /// <summary>
   /// Represents the paramters used to make a request to the TradeStation api.
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
      /// <param name="parameters">A dictionary of values to use to create the HttpParameters.</param>
      /// <param name="jsonSettingsRequest"></param>
      public HttpParameters(object parameters, JsonSerializerSettings jsonSettingsRequest = null)
      {
         Rest20.TransformObjectValues(parameters, HttpAction.Request);
         //SimpleObjectValidator.Validate(parameters);
         //
         jsonSettingsRequest = jsonSettingsRequest ?? (parameters as ApiParameters)?.JsonSerializerSettingsRequest;
         jsonSettingsRequest = jsonSettingsRequest ?? Rest20.JsonSettingsRequest;

         var jsonSerializer = JsonSerializer.CreateDefault(jsonSettingsRequest);

         Data = JToken.FromObject(parameters, jsonSerializer);
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
      /// </summary>
      //public Dictionary<string, object> Data { get; set; }
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
      /// Different API endpoints are grouped into resource categories, each with its own quota and interval settings. <br/>
      /// This allows for more granular control over different types of API usage. <br/>
      /// Some resources, particularly streaming endpoints, have concurrent connection limits in addition to request rate limits.<br/>
      /// <see href="https://api.tradestation.com/docs/fundamentals/rate-limiting-overview" />
      /// </summary>
      public RateLimitResourceCategory RateLimitResourceCategory { get; set; }

      /// <summary>
      /// Each API key is allocated specific request quotas based on the resource category being accessed. These quotas are designed to provide sufficient capacity for typical application usage while preventing system abuse. Once a quota is exceeded, all subsequent requests will return 429 Too Many Requests until the window resets.<br/>
      /// <see href="https://api.tradestation.com/docs/fundamentals/rate-limiting-overview" /><br/>
      /// RateLimitWindowQuota default value is the least restrictive: 500 requests
      /// </summary>
      public short RateLimitWindowQuota { get; set; } = 500;

      /// <summary>
      /// Quotas have "Windows" that last for a limited time interval (generally 5-minutes). <br/>
      /// Once the user has exceeded the maximum request count, all future requests will fail with a 429 error until the interval expires. <br/>
      /// Rate Limit intervals do not slide based upon the number of requests, they are fixed at a point in time starting from the very first request for that category of resource. <br/>
      /// After the interval expires, the cycle will start over at zero and the user can make more requests.<br/>
      /// <see href="https://api.tradestation.com/docs/fundamentals/rate-limiting-overview"/><br/>
      /// RateLimitWindowMinutes default value is the least restrictive: 5 minute
      /// </summary>
      public short RateLimitWindowMinutes { get; set; } = 1;

      /// <summary>
      /// Window concurrence is only applicable to the streaming endpoints.<br/>
      /// When True, the client API key will only be allowed a maximum number of concurrent streams indicated by the RateLimitWindowQuota value.<br/>
      /// <see href="https://api.tradestation.com/docs/fundamentals/rate-limiting-overview" /><br/>
      /// RateLimitWindowConcurrent default value is: False
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
}
