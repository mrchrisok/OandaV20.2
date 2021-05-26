using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Net;
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

	  /// <summary>
	  /// Sends a web request to a remote endpoint (uri).
	  /// </summary>
	  /// <typeparam name="T">The response type</typeparam>
	  /// <param name="request">The request definition.</param>
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
	  /// <param name="request">The request definition.</param>
	  /// <returns>A success response object of type T or a failure response object of type E</returns>
	  private static async Task<T> MakeRequestAsync<T, E>(Request request)
		 where E : IErrorResponse
	  {
		 HttpWebRequest webRequest = CreateHttpRequest(request.Uri, request.Method, request.Headers);

		 if (!string.IsNullOrWhiteSpace(request.Body))
		 {
			using (var writer = new StreamWriter(await webRequest.GetRequestStreamAsync()))
			{
			   await writer.WriteAsync(request.Body);
			}
		 }

		 return await GetWebResponse<T, E>(webRequest);
	  }

	  /// <summary>
	  /// Sends a web stream request to a remote service (uri).
	  /// </summary>
	  /// <param name="request">The request definition.</param>
	  /// <returns>A response object containing the stream handle.</returns>
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

	  #endregion
   }
}
