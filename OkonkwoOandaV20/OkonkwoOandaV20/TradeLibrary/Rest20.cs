using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Net;
using System.Runtime.Serialization.Json;
using System.Text;
using System.Threading.Tasks;

namespace OkonkwoOandaV20.TradeLibrary.REST
{
   /// <summary>
   /// http://developer.oanda.com/rest-live-v20/introduction/
   /// </summary>
   public partial class Rest20
   {
	  /// <summary>
	  /// The time of the last request made to an Oanda V20 service
	  /// </summary>
	  private static DateTime m_LastRequestTime = DateTime.UtcNow;

	  /// <summary>
	  /// The V20 access token for the user's or organization's Oanda Account.
	  /// The token also authenticates operations on all sub accounts.
	  /// </summary>
	  private static string AccessToken { get { return Credentials.GetDefaultCredentials().AccessToken; } }

	  /// <summary>
	  /// Oanda recommends that requests per Account are throttled to a maximium of 100 requests/second.
	  /// http://developer.oanda.com/rest-live-v20/best-practices/
	  /// </summary>
	  public static int RequestDelayMilliSeconds = 11;

	  #region request
	  /// <summary>
	  /// Gets the base uri of the target service
	  /// </summary>
	  /// <param name="server">The enurmeration for the target service</param>
	  /// <returns>Returns the base uri of the target server</returns>
	  private static string ServerUri(EServer server) { return Credentials.GetDefaultCredentials().GetServer(server); }

	  private static async Task<WebResponse> MakeStreamRequestAsync(Request request)
	  {
		 var webRequest = CreateHttpRequest(request.Uri, request.Method, request.Headers);

		 try
		 {
			WebResponse response = await webRequest.GetResponseAsync();
			return response;
		 }
		 catch (WebException ex)
		 {
			var response = (HttpWebResponse)ex.Response;
			var stream = new StreamReader(response.GetResponseStream());
			var result = stream.ReadToEnd();
			throw new Exception(result);
		 }
	  }

	  /// <summary>
	  /// Sends a web request to a remote endpoint (uri).
	  /// </summary>
	  /// <typeparam name="T">The response type</typeparam>
	  /// <param name="uri">The uri of the remote service</param>
	  /// <param name="method">method for the request (defaults to GET)</param>
	  /// <param name="request">optional parameters (if provided, it's assumed the uri doesn't contain any)</param>
	  /// <returns>A success response object of type T or a failure response object of type ErrorResponse</returns>
	  private static async Task<T> MakeRequestAsync<T>(Request request)
	  {
		 return await MakeRequestAsync<T, ErrorResponse>(request);
	  }

	  /// <summary>
	  /// Sends a web request to a remote service (uri).
	  /// </summary>
	  /// <typeparam name="T">The success response type</typeparam>
	  /// <typeparam name="E">The error  response type</typeparam>
	  /// <param name="uri">The uri of the remote service</param>
	  /// <param name="method">The request verb for the request. Default is GET</param>
	  /// <param name="request">parameters (if provided, it's assumed the uri doesn't contain any)</param>
	  /// <returns>A success response object of type T or a failure response object of type E</returns>
	  private static async Task<T> MakeRequestAsync<T, E>(Request request)
		 where E : IErrorResponse
	  {
		 if (request.Body.Count > 0)
		 {
			var requestBody = CreateJsonBody(request.Body);
			return await MakeRequestWithJsonBodyAsync<T, E>(request.Method, requestBody, request.Uri, request.Headers);
		 }

		 return await MakeRequestAsync<T, E>(request.Uri, request.Method, null, request.Headers);
	  }

	  /// <summary>
	  /// Sends a web request to a remote endpoint (uri).
	  /// </summary>
	  /// <typeparam name="T">The response type</typeparam>
	  /// <param name="uri">The uri of the remote service</param>
	  /// <param name="method">method for the request (defaults to GET)</param>
	  /// <param name="requestParams">optional parameters (if provided, it's assumed the uri doesn't contain any)</param>
	  /// <returns>A success response object of type T or a failure response object of type ErrorResponse</returns>
	  private static async Task<T> MakeRequestAsync<T>(string uri,
		 string method = "GET", IDictionary<string, string> requestParams = null, IDictionary<string, string> headers = null)
	  {
		 return await MakeRequestAsync<T, ErrorResponse>(uri, method, requestParams, headers);
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
	  private static async Task<T> MakeRequestAsync<T, E>(string uri,
		 string method = "GET", IDictionary<string, string> requestParams = null, IDictionary<string, string> headers = null)
		 where E : IErrorResponse
	  {
		 if (requestParams?.Count > 0)
		 {
			var queryString = CreateQueryString(requestParams);
			uri = uri + "?" + queryString;
		 }

		 return await GetWebResponse<T, E>(CreateHttpRequest(uri, method, headers));
	  }

	  /// <summary>
	  /// Sends a web request with a Json body to a remote service (uri). 
	  /// </summary>
	  /// <typeparam name="T">The success response type</typeparam>
	  /// <typeparam name="E">The error response type</typeparam>
	  /// <param name="method">The request verb for the request</param>
	  /// <param name="requestBody">The request body (must be a valid json string)</param>
	  /// <param name="uri">The uri of the remote service</param>
	  /// <returns>A success response object of type T or a failure response object of type E</returns>
	  private static async Task<T> MakeRequestWithJsonBodyAsync<T, E>(string method, string requestBody, string uri, IDictionary<string, string> headers = null)
		 where E : IErrorResponse
	  {
		 // Create the request
		 HttpWebRequest request = CreateHttpRequest(uri, method, headers);

		 // Write the body
		 using (var writer = new StreamWriter(await request.GetRequestStreamAsync()))
		 {
			await writer.WriteAsync(requestBody);
		 }

		 return await GetWebResponse<T, E>(request);
	  }

	  /// <summary>
	  /// Sends a web request with a Json body to a remote service (uri).
	  /// </summary>
	  /// <typeparam name="T">The success response type</typeparam>
	  /// <typeparam name="E">The error response type</typeparam>
	  /// <typeparam name="P">The requestParams object type</typeparam>
	  /// <param name="method">The request verb for the request</param>
	  /// <param name="requestParams">the parameters to pass in the request body</param>
	  /// <param name="uri">The uri of the remote service</param>
	  /// <returns>A success response object of type T or a failure response object of type E</returns>
	  private static async Task<T> MakeRequestWithJsonBodyAsync<T, E, P>(string method, P requestParams, string uri, IDictionary<string, string> headers = null)
		 where E : IErrorResponse
	  {
		 var requestBody = CreateJsonBody(requestParams);

		 return await MakeRequestWithJsonBodyAsync<T, E>(method, requestBody, uri, headers);
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
	  private static async Task<T> MakeRequestWithJSONBodyAsync<T, E>(Request request)
		 where E : IErrorResponse
	  {
		 var requestParams = ConvertToDictionary(request.Parameters);
		 var requestBody = CreateJsonBody(requestParams);

		 return await MakeRequestWithJsonBodyAsync<T, E>(request.Method, requestBody, request.Uri, request.Headers);
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
	  private static async Task<T> GetWebResponse<T, E>(HttpWebRequest request)
	  {
		 while (DateTime.UtcNow < m_LastRequestTime.AddMilliseconds(RequestDelayMilliSeconds))
		 {
		 }

		 try
		 {
			using (WebResponse response = await request.GetResponseAsync())
			{
			   var stream = GetResponseStream(response);
			   var reader = new StreamReader(stream);
			   var json = reader.ReadToEnd();
			   var result = JsonConvert.DeserializeObject<T>(json);
			   return result;
			}
		 }
		 catch (WebException ex)
		 {
			var stream = GetResponseStream(ex.Response);
			var reader = new StreamReader(stream);
			var result = reader.ReadToEnd();

			// add a 'type' property for the ErrorResponseFactory
			dynamic error = JsonConvert.DeserializeObject(result);
			error.type = typeof(E).Name;
			var errorResult = JsonConvert.SerializeObject(error);

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
	  private static Stream GetResponseStream(WebResponse response)
	  {
		 var stream = response.GetResponseStream();

		 // handle a gzipped response
		 if (response.Headers[HttpResponseHeader.ContentEncoding] == "gzip")
			stream = new GZipStream(stream, CompressionMode.Decompress);

		 // handle a deflated response
		 else if (response.Headers[HttpResponseHeader.ContentEncoding] == "deflate")
			stream = new DeflateStream(stream, CompressionMode.Decompress);

		 return stream;
	  }
	  #endregion

	  #region json
	  /// <summary>
	  /// Creates the request body as a Json string
	  /// </summary>
	  /// <typeparam name="P">The type of the parameterObject</typeparam>
	  /// <param name="parameterObject">The object containing the request body parameters</param>
	  /// <param name="simpleDictionary">Indicates if the passed object is a Dictionary</param>
	  /// <returns>A JSON string representing the request body</returns>
	  private static string CreateJsonBody<P>(P parameterObject)
	  {
		 // for parameters passed as dictionaries
		 if (typeof(P).GetInterfaces().Contains(typeof(IDictionary)))
		 {
			var settings = new DataContractJsonSerializerSettings
			{
			   UseSimpleDictionaryFormat = true
			};

			var jsonSerializer = new DataContractJsonSerializer(typeof(P), settings);
			using (var ms = new MemoryStream())
			{
			   jsonSerializer.WriteObject(ms, parameterObject);
			   var msBytes = ms.ToArray();
			   return Encoding.UTF8.GetString(msBytes, 0, msBytes.Length);
			}
		 }
		 // for parameters passed as objects
		 else
			return ConvertToJson(parameterObject);
	  }

	  /// <summary>
	  /// Serializes an object to a Json string
	  /// </summary>
	  /// <param name="obj">The object to serialize</param>
	  /// <param name="ignoreNulls">Indicates if null properties should be excluded from the JSON output</param>
	  /// <returns>A JSON string representing the input object</returns>
	  private static string ConvertToJson(object obj, bool ignoreNulls = true)
	  {
		 var nullHandling = ignoreNulls ? NullValueHandling.Ignore : NullValueHandling.Include;

		 // oco: look into the DateFormatting
		 // might be able to use DateTime instead of string in objects
		 var settings = new JsonSerializerSettings()
		 {
			TypeNameHandling = TypeNameHandling.None,
			NullValueHandling = nullHandling
		 };

		 string result = JsonConvert.SerializeObject(obj, settings);

		 return result;
	  }
	  #endregion

	  #region utilities
	  /// <summary>
	  /// Creates the request object string out of a dictionary of parameters
	  /// </summary>
	  /// <param name="uri">The uri of the remote service</param>
	  /// <param name="method">The Http verb for the request</param>
	  /// <returns>An HttpWebRequest object</returns>
	  private static HttpWebRequest CreateHttpRequest(string uri, string method, IDictionary<string, string> headers = null)
	  {
		 HttpWebRequest request = WebRequest.CreateHttp(uri);
		 request.Method = method;
		 request.ContentType = "application/json";

		 if (headers != null)
		 {
			foreach (var header in headers)
			{
			   request.Headers[header.Key] = header.Value;
			}
		 }
		 request.Headers[HttpRequestHeader.Authorization] = $"Bearer {AccessToken}";
		 request.Headers[HttpRequestHeader.AcceptEncoding] = "gzip, deflate";

		 return request;
	  }

	  /// <summary>
	  /// Converts a list of strings into a comma-separated values list (csv)
	  /// </summary>
	  /// <param name="items">The list of strings to convert to csv</param>
	  /// <returns>A csv string of the list items</returns>
	  private static string GetCommaSeparatedString<T>(List<T> items)
	  {
		 var stringBuilder = new StringBuilder();
		 foreach (var item in items)
		 {
			if (!stringBuilder.ToString().Contains(item.ToString()))
			   stringBuilder.Append(item + ",");
		 }
		 return stringBuilder.ToString().Trim(',');
	  }

	  /// <summary>
	  /// Creates a query string from a dictionary of parameters
	  /// </summary>
	  /// <param name="requestParams">The parameters dictionary to convert to a query string.</param>
	  /// <returns>A query string</returns>
	  private static string CreateQueryString(IDictionary<string, string> requestParams)
	  {
		 string queryString = "";
		 foreach (var pair in requestParams)
		 {
			queryString += WebUtility.UrlEncode(pair.Key) + "=" + WebUtility.UrlEncode(pair.Value) + "&";
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

		 string json = ConvertToJson(input, true);

		 return JsonConvert.DeserializeObject<Dictionary<string, string>>(json);
	  }
	  #endregion
   }
}
