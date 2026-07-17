using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using OkonkwoOandaV20.Framework.JsonConverters;
using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
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
      #region initialization

      /// <summary>
      /// Initialize the middleware that will make calls to TradeStation V3 endpoints.
      /// This method is idempotent. Once initialize, any additional calls will have no effect.
      /// </summary>
      /// <param name="settings">The client TSV3 settings.</param>
      /// <param name="requestClient">An HttpClient configured to support standard connections.</param>
      /// <param name="streamsClient">An HttpClient configured to support streaming connections.</param>
      /// <param name="jsonSerializerSettings"></param>
      /// <param name="oauthSvc"></param>
      /// <param name="cache"></param>
      /// <param name="logger"></param>
      /// <param name="authorizeApplication"></param>
      /// <returns>True, if initialization was successful. False if not successful.</returns>
      public static Task<bool> InitializeAsync(HttpClient requestClient = null, HttpClient streamsClient = null
         , JsonSerializerSettings jsonSerializerSettings = null, (EEnvironment environment
         , string accessToken, string accountId)? credentials = null)
      {
         if (!_initialized)
         {
            _requestClient = requestClient ?? new HttpClient();
            _streamsClient = streamsClient ?? new HttpClient() { Timeout = Timeout.InfiniteTimeSpan };

            JsonSerializerSettings = jsonSerializerSettings ?? GetJsonSerializerSettings();

            if (credentials.HasValue)
            {
               Credentials.SetCredentials(credentials.Value.environment, credentials.Value.accessToken, credentials.Value.accountId);
            }

            _initialized = true;
         }

         return Task.FromResult(true);
      }

      private static JsonSerializerSettings GetJsonSerializerSettings()
      {
         return new JsonSerializerSettings()
         {
            TypeNameHandling = TypeNameHandling.None,
            NullValueHandling = NullValueHandling.Ignore,
            DefaultValueHandling = DefaultValueHandling.Ignore,
            DateFormatHandling = DateFormatHandling.IsoDateFormat,

            Converters = new List<JsonConverter> {
               new TransactionConverter(),
               new OrderConverter(), new PriceObjectConverter(), new StringDecimalConverter(),
               new PricingStreamResponseConverter(), new TransactionsStreamResponseConverter()
            }
         };
      }

      #endregion

      #region properties

      private static bool _initialized = false;
      private static HttpClient _requestClient;
      private static HttpClient _streamsClient;
      public static JsonSerializerSettings JsonSerializerSettings { get; private set; }

      /// <summary>
      /// The time of the last request made to an Oanda V20 service
      /// </summary>
      private static DateTime m_LastRequestTime = DateTime.UtcNow;

      /// <summary>
      /// The V20 access token for the user's or organization's Oanda Account.
      /// The token also authenticates operations on all sub accounts.
      /// </summary>
      private static string AccessToken { get { return Credentials.GetCredentials().AccessToken; } }

      /// <summary>
      /// Oanda recommends that requests per Account are throttled to a maximium of 100 requests/second.
      /// http://developer.oanda.com/rest-live-v20/best-practices/
      /// </summary>
      public static int RequestDelayMilliSeconds = 11;

      #endregion

      #region request
      /// <summary>
      /// Gets the base uri of the target service
      /// </summary>
      /// <param name="server">The enurmeration for the target service</param>
      /// <returns>Returns the base uri of the target server</returns>
      private static string ServerUri(EServer server) { return Credentials.GetCredentials().GetServer(server); }

      /// <summary>
      /// Sends a web request to a remote endpoint (uri).
      /// </summary>
      /// <typeparam name="T">The response type</typeparam>
      /// <param name="uri">The uri of the remote service</param>
      /// <param name="method">method for the request (defaults to GET)</param>
      /// <param name="requestParams">optional parameters (if provided, it's assumed the uri doesn't contain any)</param>
      /// <returns>A success response object of type T or a failure response object of type ErrorResponse</returns>
      private static async Task<T> MakeRequestAsync<T>(string uri, string method = "GET", Dictionary<string, string> requestParams = null, HttpCompletionOption? completionOption = null)
      {
         return await MakeRequestAsync<T, ErrorResponse>(uri, method, requestParams, completionOption);
      }

      /// <summary>
      /// Sends a web request to a remote service (uri).
      /// </summary>
      /// <typeparam name="T">The success response type</typeparam>
      /// <typeparam name="E">The error  response type</typeparam>
      /// <param name="uri">The uri of the remote service</param>
      /// <param name="method">The request verb for the request. Default is GET</param>
      /// <param name="requestParams">optional parameters (if provided, it's assumed the uri doesn't contain any)</param>
      /// <returns>A success response object of type T or a failure response object of type E</returns>
      private static async Task<T> MakeRequestAsync<T, E>(string uri, string method = "GET", Dictionary<string, string> requestParams = null, HttpCompletionOption? completionOption = null)
         where E : IErrorResponse
      {
         if (requestParams?.Count > 0)
         {
            var queryString = CreateQueryString(requestParams);
            uri = uri + "?" + queryString;
         }

         return await GetWebResponse<T, E>(CreateHttpRequestMessage(uri, method), completionOption);
      }

      /// <summary>
      /// Sends a web request with a JSON body to a remote service (uri). 
      /// </summary>
      /// <typeparam name="T">The success response type</typeparam>
      /// <typeparam name="E">The error response type</typeparam>
      /// <param name="method">The request verb for the request</param>
      /// <param name="requestBody">The request body (must be a valid json string)</param>
      /// <param name="uri">The uri of the remote service</param>
      /// <returns>A success response object of type T or a failure response object of type E</returns>
      private static async Task<T> MakeRequestWithJSONBody<T, E>(string method, string requestBody, string uri
         , HttpCompletionOption? completionOption = null)
         where E : IErrorResponse
      {
         // Create the request
         HttpRequestMessage request = CreateHttpRequestMessage(uri, method, requestBody);

         return await GetWebResponse<T, E>(request, completionOption);
      }

      /// <summary>
      /// Sends a web request with a JSON body to a remote service (uri).
      /// </summary>
      /// <typeparam name="T">The success response type</typeparam>
      /// <typeparam name="E">The error response type</typeparam>
      /// <typeparam name="P">The requestParams object type</typeparam>
      /// <param name="method">The request verb for the request</param>
      /// <param name="requestParams">the parameters to pass in the request body</param>
      /// <param name="uri">The uri of the remote service</param>
      /// <returns>A success response object of type T or a failure response object of type E</returns>
      private static async Task<T> MakeRequestWithJSONBody<T, E, P>(string method, P requestParams, string uri
         , HttpCompletionOption? completionOption = null)
         where E : IErrorResponse
      {
         var requestBody = ConvertObjectToJson(requestParams);

         return await MakeRequestWithJSONBody<T, E>(method, requestBody, uri, completionOption);
      }
      #endregion

      #region response

      /// <summary>
      /// Sends an Http request to a remote service and returns the de-serialized response
      /// </summary>
      /// <typeparam name="T">>Type of the response returned by the remote service</typeparam>
      /// <typeparam name="E">>Type of the error response returned by the remote service</typeparam>
      /// <param name="request">The request sent to the remote service</param>
      /// <returns>A success response object of type T or a failure response object of type E</returns>
      private static async Task<T> GetWebResponse<T, E>(HttpRequestMessage request
         , HttpCompletionOption? completionOption = null)
      {
         while (DateTime.UtcNow < m_LastRequestTime.AddMilliseconds(RequestDelayMilliSeconds))
         {
         }

         completionOption = completionOption ?? HttpCompletionOption.ResponseHeadersRead;

         try
         {
            using (HttpResponseMessage response = await _requestClient.SendAsync(request, completionOption.Value))
            {
               response.EnsureSuccessStatusCode();

               var stream = GetResponseStream(response);
               var reader = new StreamReader(stream);
               var json = reader.ReadToEnd();
               var result = JsonConvert.DeserializeObject<T>(json, JsonSerializerSettings);
               return result;
            }
         }
         catch (HttpRequestException ex)
         {
            // add a 'type' property for the ErrorResponseFactory
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
      private static Stream GetResponseStream(HttpResponseMessage response)
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
      /// Serializes an object to a JSON string
      /// </summary>
      /// <param name="obj">The object to serialize</param>
      /// <param name="ignoreNulls">Indicates if null properties should be excluded from the JSON output</param>
      /// <returns>A JSON string representing the input object</returns>
      private static string ConvertObjectToJson(object obj, bool ignoreNulls = true)
      {
         var nullHandling = ignoreNulls ? NullValueHandling.Ignore : NullValueHandling.Include;

         var settings = new JsonSerializerSettings(JsonSerializerSettings)
         {
            NullValueHandling = nullHandling,
         };

         return JsonConvert.SerializeObject(obj, settings);
      }

      private static HttpRequestMessage CreateHttpRequestMessage(string uri, string method, string requestBody = null)
      {
         var request = new HttpRequestMessage(new HttpMethod(method), uri)
         {
            Content = !string.IsNullOrEmpty(requestBody) 
                      ? new StringContent(requestBody, Encoding.UTF8, "application/json") 
                      : null
         };

         request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", AccessToken);
         request.Headers.Accept.Add(MediaTypeWithQualityHeaderValue.Parse("application/json"));
         request.Headers.AcceptEncoding.Add(StringWithQualityHeaderValue.Parse("gzip"));
         request.Headers.AcceptEncoding.Add(StringWithQualityHeaderValue.Parse("deflate"));

         return request;
      }

      /// <summary>
      /// Converts a list of strings into a comma-separated values list (csv)
      /// </summary>
      /// <param name="items">The list of strings to convert to csv</param>
      /// <returns>A csv string of the list items</returns>
      private static string GetCommaSeparatedString(List<string> items)
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
      /// Creates a query string from a dictionary of parameters
      /// </summary>
      /// <param name="requestParams">The parameters dictionary to convert to a query string.</param>
      /// <returns>A query string</returns>
      private static string CreateQueryString(Dictionary<string, string> requestParams)
      {
         string queryString = "";
         foreach (var pair in requestParams)
         {
            queryString += $"{WebUtility.UrlEncode(pair.Key)}={WebUtility.UrlEncode(pair.Value)}&";
         }
         queryString = queryString.Trim('&');
         return queryString;
      }

      /// <summary>
      /// Converts an object into a dictionary of key-value pairs
      /// </summary>
      /// <param name="input">The object to convet to a dictionary</param>
      /// <returns>A Dictionary{string,string} object.</returns>
      public static Dictionary<string, string> ConvertToDictionary(object input)
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

      private static JsonSerializerSettings _dictionarySettings;

      #endregion
   }
}
