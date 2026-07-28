using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using OkonkwoOandaV20.Framework;
using OkonkwoOandaV20.Framework.JsonConverters;
using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace OkonkwoOandaV20.TradeLibrary.REST
{
   /// <summary>
   /// http://developer.oanda.com/rest-live-v20/introduction/
   /// </summary>
   public partial class Rest20
   {
      /// <summary>
      /// Initialize the middleware that will make calls to Oanda V3 endpoints.
      /// This method is idempotent. Once initialize, any additional calls will have no effect.
      /// </summary>
      /// <param name="settings">The client TSV3 settings.</param>
      /// <param name="requestClient">An HttpClient configured to support standard connections.</param>
      /// <param name="streamsClient">An HttpClient configured to support streaming connections.</param>
      /// <param name="jsonSerializerSettings">Used to json serialization .. nuff said</param>
      /// <param name="valueTransformers">Used to transform request/response property values</param>
      /// <param name="credentials">Used to authenticate to Oanda api</param>
      /// <param name="logger">It's a logger.</param>
      /// <returns>True, if initialization was successful. False if not successful.</returns>
      public Rest20(HttpClient requestClient = null, HttpClient streamsClient = null
         , JsonSerializerSettings jsonSettingsRequest = null, JsonSerializerSettings jsonSettingsResponse = null
         , IList<JsonConverter> jsonConverters = null, IDictionary<string, Action<object>> valueTransformers = null
         , (EEnvironment environment, string accessToken, string accountId)? credentials = null, ILogger logger = null)
      {
         _requestClient = requestClient ?? new HttpClient();
         _streamsClient = streamsClient ?? new HttpClient() { Timeout = Timeout.InfiniteTimeSpan };
         _logger = logger;

         JsonConverters = SetJsonConverters(jsonConverters);
         JsonSettingsRequest = jsonSettingsRequest;
         JsonSettingsResponse = jsonSettingsResponse;
         ValueTransformers = valueTransformers ?? new Dictionary<string, Action<object>>();

         if (credentials.HasValue)
         {
            _credentials = new Credentials(
               credentials.Value.environment, credentials.Value.accessToken, credentials.Value.accountId);
         }
      }

      #region initialization

      /// <summary>
      /// Initialize the middleware that will make calls to Oanda V3 endpoints.
      /// This method is idempotent. Once initialize, any additional calls will have no effect.
      /// </summary>
      /// <param name="credentials">Used to authenticate to Oanda api</param>
      /// <returns>True, if initialization was successful. False if not successful.</returns>
      public virtual Task<bool> InitializeAsync((EEnvironment environment, string accessToken, string accountId)? credentials)
      {
         _creds = credentials ?? _creds;

         if (!_creds.HasValue)
            throw new ArgumentNullException("Credentials are null");

         _credentials = new Credentials(
            _creds.Value.environment, _creds.Value.accessToken, _creds.Value.accountId);

         return Task.FromResult(true);
      }

      protected virtual JsonSerializerSettings InitializeJsonSerializerSettings(string httpAction)
      {
         //jsonSettings = jsonSettings ?? new JsonSerializerSettings();
         //var jsonConverters = new List<JsonConverter>(JsonConverters);
         //jsonConverters.AddRange(jsonSettings.Converters);
         ////

         if (httpAction != HttpAction.Request)
         {
            JsonSettingsRequest = JsonSettingsRequest ?? new JsonSerializerSettings();
            var jsonConvertersRequest = new List<JsonConverter>(JsonConverters);
            jsonConvertersRequest.AddRange(JsonSettingsRequest.Converters);
            JsonSettingsRequest = new JsonSerializerSettings()
            {
               TypeNameHandling = TypeNameHandling.None,
               NullValueHandling = NullValueHandling.Ignore,
               DefaultValueHandling = DefaultValueHandling.Ignore,
               DateFormatHandling = DateFormatHandling.IsoDateFormat,
               Converters = JsonConverters
            };
         }

         if (httpAction != HttpAction.Response)
         {
            JsonSettingsRequest
         }

         return new JsonSerializerSettings()
         {
            TypeNameHandling = TypeNameHandling.None,
            NullValueHandling = NullValueHandling.Ignore,
            DefaultValueHandling = DefaultValueHandling.Ignore,
            DateFormatHandling = DateFormatHandling.IsoDateFormat,
            Converters = JsonConverters
         };
      }

      protected virtual IList<JsonConverter> SetJsonConverters(IList<JsonConverter> jsonConverters = null)
      {
         jsonConverters = jsonConverters ?? new List<JsonConverter>();

         return new List<JsonConverter>(jsonConverters) {

            new OrderConverter(), new PriceObjectConverter(), new TransactionConverter(),
            new PricingStreamResponseConverter(), new TransactionsStreamResponseConverter()
         };
      }

      #endregion

      #region properties

      protected (EEnvironment environment, string accessToken, string accountId)? _creds;
      protected Credentials _credentials;
      protected readonly HttpClient _requestClient;
      protected readonly HttpClient _streamsClient;
      protected readonly ILogger _logger;

      /// <summary>
      /// JsonSerializerSettings for request to Oanda
      /// </summary>
      public JsonSerializerSettings JsonSettingsRequest { get; protected set; }

      /// <summary>
      /// JsonSerializerSettings for response from Oanda
      /// </summary>
      public JsonSerializerSettings JsonSettingsResponse { get; protected set; }

      /// <summary>
      /// JsonConverters for request/response from Oanda
      /// </summary>
      protected IList<JsonConverter> JsonConverters { get; set; }

      /// <summary>
      /// Value transformers for request/response from Oanda
      /// </summary>
      protected IDictionary<string, Action<object>> ValueTransformers { get; set; }

      /// <summary>
      /// The time of the last request made to an Oanda V20 service
      /// </summary>
      protected static DateTime m_LastRequestTime = DateTime.UtcNow;

      /// <summary>
      /// The V20 access token for the user's or organization's Oanda Account.
      /// The token also authenticates operations on all sub accounts.
      /// </summary>
      protected string AccessToken { get { return _credentials.GetCredentials().AccessToken; } }

      /// <summary>
      /// Oanda recommends that requests per Account are throttled to a maximium of 100 requests/second.
      /// http://developer.oanda.com/rest-live-v20/best-practices/
      /// </summary>
      protected static int RequestDelayMilliSeconds = 11;

      #endregion

      #region request

      /// <summary>
      /// Gets the base uri of the target service
      /// </summary>
      /// <param name="server">The enurmeration for the target service</param>
      /// <returns>Returns the base uri of the target server</returns>
      protected string ServerUri(EServer server) { return _credentials.GetCredentials().GetServer(server); }

      /// <summary>
      /// New: Sends a web request using HttpParameters (object -> querystring/body) and returns a deserialized object.
      /// </summary>
      protected virtual async Task<T> MakeRequestAsync<T, E>(HttpParameters parameters, CancellationToken cancellation = default)
         where E : IErrorResponse
      {
         HttpRequestMessage request = await CreateHttpRequestAsync(parameters, cancellation);

         return await GetObjectResponseAsync<T, E>(request, parameters, cancellation);
      }

      /// <summary>
      /// New: Sends a streaming request using HttpParameters and returns the raw HttpResponseMessage for streaming consumption.
      /// </summary>
      protected virtual async Task<HttpResponseMessage> MakeStreamRequestAsync<E>(HttpParameters parameters, CancellationToken cancellation = default)
         where E : IErrorResponse
      {
         parameters.AcceptType = parameters.AcceptType ?? "application/json";

         HttpRequestMessage request = await CreateHttpRequestAsync(parameters, cancellation);

         return await GetStreamResponseAsync<E>(request, parameters, cancellation);
      }

      /// <summary>
      /// New: Create an HttpRequestMessage from HttpParameters
      /// </summary>
      protected virtual Task<HttpRequestMessage> CreateHttpRequestAsync(HttpParameters parameters, CancellationToken cancellation)
      {
         if (cancellation.IsCancellationRequested) return default;

         string queryString = string.Empty;
         HttpContent content = null;

         if (parameters.Binding == HttpParametersBinding.QueryString)
         {
            if (parameters.Data is JObject jObj && jObj.Count > 0)
            {
               if (parameters.Data?.Count() > 0)
                  queryString = $"?{Utilities.ConvertToQueryString(parameters.Data.Value<JObject>())}";
            }
         }

         else if (parameters.Binding == HttpParametersBinding.Body)
         {
            if (parameters.ContentType == "application/x-www-form-urlencoded")
            {
               var data = new Dictionary<string, string>();
               if (parameters.Data is JObject jObj2)
               {
                  foreach (var item in jObj2)
                  {
                     data.Add(item.Key, item.Value.ToString());
                  }
               }
               content = new FormUrlEncodedContent(data);
            }

            else if (parameters.ContentType == "application/json")
            {
               var json = parameters?.Data.ToString(Formatting.None);
               content = new StringContent(json, System.Text.Encoding.UTF8, parameters.ContentType);
            }
         }

         var request = new HttpRequestMessage()
         {
            Method = parameters.Method,
            RequestUri = new Uri(parameters.Uri.ToString() + queryString),
            Content = content
         };

         request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", AccessToken);
         request.Headers.Accept.Add(MediaTypeWithQualityHeaderValue.Parse(parameters.AcceptType));
         request.Headers.AcceptEncoding.Add(StringWithQualityHeaderValue.Parse("gzip"));
         request.Headers.AcceptEncoding.Add(StringWithQualityHeaderValue.Parse("deflate"));

         return Task.FromResult(request);
      }

      #endregion

      #region response

      /// <summary>
      /// Sends an Http request to a remote service and returns the de-serialized response
      /// </summary>
      /// <typeparam name="T">>Type of the response returned by the remote service</typeparam>
      /// <typeparam name="E">>Type of the error response returned by the remote service</typeparam>
      /// <param name="request">The request sent to the remote service</param>
      /// <param name="parameters">The parameters needed to configure the request</param>
      /// <param name="cancellation">The token for cancelling the request</param>
      /// <returns>The object returned in the response.</returns>
      protected virtual async Task<T> GetObjectResponseAsync<T, E>(HttpRequestMessage request, HttpParameters parameters, CancellationToken cancellation)
         where E : IErrorResponse
      {
         while (DateTime.UtcNow < m_LastRequestTime.AddMilliseconds(RequestDelayMilliSeconds))
         {
         }

         var completionOption = parameters?.CompletionOption ?? HttpCompletionOption.ResponseHeadersRead;

         try
         {
            using (var response = await _requestClient.SendAsync(request, completionOption, cancellation))
            {
               response.EnsureSuccessStatusCode();

               var stream = GetResponseStream(response);
               var reader = new StreamReader(stream);
               var json = reader.ReadToEnd();
               var result = JsonConvert.DeserializeObject<T>(json, JsonSettingsResponse);

               if (!parameters.ForInternalResponse)
                  TransformObjectValues(result, HttpAction.Response);

               return result;
            }
         }
         catch (HttpRequestException ex)
         {
            var errorObject = new JObject() {
               { "error", ex.Message }, { "type", typeof(E).Name }
            };
            var errorResult = JsonConvert.SerializeObject(errorObject);

            throw new Exception(errorResult);
         }
         finally
         {
            m_LastRequestTime = DateTime.UtcNow;
         }
      }

      /// <summary>
      /// Sends an Http request to a remote service and returns the HttpResponseMessage for streaming consumption
      /// </summary>
      protected virtual async Task<HttpResponseMessage> GetStreamResponseAsync<E>(HttpRequestMessage request, HttpParameters parameters, CancellationToken cancellation = default)
         where E : IErrorResponse
      {
         while (DateTime.UtcNow < m_LastRequestTime.AddMilliseconds(RequestDelayMilliSeconds))
         {
         }

         try
         {
            var completionOption = parameters?.CompletionOption ?? HttpCompletionOption.ResponseHeadersRead;
            var response = await _streamsClient.SendAsync(request, completionOption, cancellation);
            response.EnsureSuccessStatusCode();

            return response;
         }
         catch (HttpRequestException ex)
         {
            var errorObject = new JObject() {
               { "error", ex.Message }, { "type", typeof(E).Name }
            };
            var errorResult = JsonConvert.SerializeObject(errorObject);

            throw new Exception(errorResult);
         }
         finally
         {
            m_LastRequestTime = DateTime.UtcNow;
         }
      }

      /// <summary>
      /// Writes the response from the remote service to a text stream
      /// </summary>
      /// <param name="response">The response received from the remote service</param>
      /// <returns>A stream object. The stream may be a subclass (GZipStream or DeflateStream) if
      /// the response header indicates matched encoding.</returns>
      protected virtual Stream GetResponseStream(HttpResponseMessage response)
      {
         var stream = response.Content.ReadAsStreamAsync().Result;

         if (stream == null)
            return Stream.Null;

         var encodings = response.Content?.Headers?.ContentEncoding;

         if (encodings != null && encodings.Contains("gzip"))
            return new GZipStream(stream, CompressionMode.Decompress);
         if (encodings != null && encodings.Contains("deflate"))
            return new DeflateStream(stream, CompressionMode.Decompress);

         return stream;
      }

      #endregion

      #region json

      /// <summary>
      /// Converts a list of strings into a comma-separated values list (csv)
      /// </summary>
      /// <param name="items">The list of strings to convert to csv</param>
      /// <returns>A csv string of the list items</returns>
      protected virtual string GetCommaSeparatedString(List<string> items)
      {
         var stringBuilder = new StringBuilder();
         foreach (var item in items)
         {
            if (!stringBuilder.ToString().Contains(item))
               stringBuilder.Append(item + ",");
         }
         return stringBuilder.ToString().Trim(',');
      }

      /// <summary>
      /// Converts an object into a dictionary of key-value pairs
      /// </summary>
      /// <param name="input">The object to convet to a dictionary</param>
      /// <returns>A Dictionary{string,string} object.</returns>
      protected virtual Dictionary<string, string> ConvertToDictionary(object input)
      {
         if (input == null)
            return null;

         // using a different serializer so that values are not mutated by any
         //    settings in the request/response serializer
         _dictionarySettings = _dictionarySettings ?? new JsonSerializerSettings()
         {
            NullValueHandling = NullValueHandling.Ignore,
            TypeNameHandling = TypeNameHandling.None,
            DefaultValueHandling = DefaultValueHandling.Ignore
         };

         var json = JsonConvert.SerializeObject(input, _dictionarySettings);
         return JsonConvert.DeserializeObject<Dictionary<string, string>>(json);
      }
      protected static JsonSerializerSettings _dictionarySettings;

      #endregion

      public virtual void TransformObjectValues(object inputObject, string httpAction = HttpAction.Response)
      {
         if (inputObject == null)
            return;

         if (httpAction == HttpAction.Request || httpAction == HttpAction.Response)
         {
            if (ValueTransformers.TryGetValue(httpAction, out var valueTransformer))
               valueTransformer.Invoke(inputObject);

            if (httpAction == HttpAction.Request)
               SimpleObjectValidator.Validate(inputObject);

            return;
         }

         throw new ArgumentException($"Value transformer type {httpAction} is not supported.");
      }

      /// <summary>
      /// Determines if trading is halted for the provided instrument.
      /// </summary>
      /// <param name="instrument">Instrument to check if halted. Default is EUR_USD.</param>
      /// <returns>True if trading is halted, false if trading is not halted.</returns>
      public virtual async Task<bool> IsMarketHalted(string instrument = InstrumentName.Currency.EURUSD, bool throwIfHalted = false)
      {
         var accountId = _credentials.GetCredentials().AccountId;

         var parameters = new PricingParameters()
         {
            accountID = accountId,
            instruments = new List<string>() { instrument }
            ,
            ForInternalRequest = true
         };

         var response = await GetPricingAsync(parameters);

         bool isTradeable = false, hasBids = false, hasAsks = false;

         if (response.prices[0] != null)
         {
            isTradeable = response.prices[0].tradeable;
            hasBids = response.prices[0].bids.Count > 0;
            hasAsks = response.prices[0].asks.Count > 0;
         }

         if (isTradeable && hasBids && hasAsks)  // not halted
            return false;

         if (throwIfHalted)
            throw new MarketHaltedException($"Market is halted for this instrument: {instrument}");

         return true;
      }

   }
}
