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
      /// The time of the last request made to an Account endpoint
      /// </summary>
      private static DateTime m_LastRequestTime = DateTime.UtcNow;
       
      /// <summary>
      /// The active access token for the Account
      /// </summary>
      private static string AccessToken { get { return Credentials.GetDefaultCredentials().AccessToken; } }

      /// <summary>
      /// Oanda recommends that requests per Account are throttled to a maximium of 100 requests/second
      /// http://developer.oanda.com/rest-live-v20/best-practices/
      /// </summary>
      public static int RequestDelayMilliSeconds = 11;

      #region request
      /// <summary>
      /// Returns the base uri of the target server
      /// </summary>
      /// <param name="server">The target server</param>
      /// <returns></returns>
      private static string ServerUri(EServer server) { return Credentials.GetDefaultCredentials().GetServer(server); }

      /// <summary>
      /// Primary (internal) request handler
      /// </summary>
      /// <typeparam name="T">The response type</typeparam>
      /// <param name="uri">the uri of the request</param>
      /// <param name="method">method for the request (defaults to GET)</param>
      /// <param name="requestParams">optional parameters (if provided, it's assumed the uri doesn't contain any)</param>
      /// <returns>response via type T</returns>
      private static async Task<T> MakeRequestAsync<T>(string uri, string method = "GET", Dictionary<string, string> requestParams = null)
      {
         return await MakeRequestAsync<T, ErrorResponse>(uri, method, requestParams);
      }

      /// <summary>
      /// Primary (internal) request handler
      /// </summary>
      /// <typeparam name="T">The success response type</typeparam>
      /// <typeparam name="E">The error  response type</typeparam>
      /// <param name="uri">the uri of the request</param>
      /// <param name="method">method for the request (defaults to GET)</param>
      /// <param name="requestParams">optional parameters (if provided, it's assumed the uri doesn't contain any)</param>
      /// <returns>response via type T</returns>
      private static async Task<T> MakeRequestAsync<T, E>(string uri, string method = "GET", Dictionary<string, string> requestParams = null) 
         where E : IErrorResponse
      {
         if (requestParams != null && requestParams.Count > 0)
         {
            var queryString = CreateQueryString(requestParams);
            uri = uri + "?" + queryString;
         }

         return await GetWebResponse<T, E>(CreateHttpRequest(uri, method));
      }

      /// <summary>
      /// Secondary (internal) request handler. differs from primary in that parameters are placed in the body instead of the request string
      /// </summary>
      /// <typeparam name="T">The success response type</typeparam>
      /// <typeparam name="E">The error response type</typeparam>
      /// <param name="method">method to use (usually POST or PATCH)</param>
      /// <param name="requestBody">request body (must be a valid json string)</param>
      /// <param name="uri">the uri of the request</param>
      /// <returns>response, via type T</returns>
      private static async Task<T> MakeRequestWithJSONBody<T, E>(string method, string requestBody, string uri) 
         where E : IErrorResponse
      {
         // Create the request
         HttpWebRequest request = CreateHttpRequest(uri, method);

         // Write the body
         using (var writer = new StreamWriter(await request.GetRequestStreamAsync()))
            await writer.WriteAsync(requestBody);

         return await GetWebResponse<T, E>(request);
      }

      /// <summary>
      /// Secondary (internal) request handler.
      /// Differs from primary in that parameters are placed in the body instead of the request string.
      /// </summary>
      /// <typeparam name="T">The success response type</typeparam>
      /// <typeparam name="E">The error response type</typeparam>
      /// <typeparam name="P">The requestParams object type</typeparam>
      /// <param name="method">method to use (usually POST or PATCH)</param>
      /// <param name="requestParams">the parameters to pass in the request body</param>
      /// <param name="uri">the uri of the request</param>
      /// <returns>response, via type T</returns>
      private static async Task<T> MakeRequestWithJSONBody<T, E, P>(string method, P requestParams, string uri) 
         where E : IErrorResponse
      {
         var requestBody = (typeof(P).GetInterfaces().Contains(typeof(IDictionary)) == false)
            ? CreateJSONBody(ConvertToDictionary(requestParams))
            : CreateJSONBody(requestParams);

         return await MakeRequestWithJSONBody<T, E>(method, requestBody, uri);
      }
      #endregion

      #region response
      /// <summary>
      /// Sends an Http request to a remote server and returns the de-serialized response
      /// </summary>
      /// <typeparam name="T">>Type of the response returned by the remote server</typeparam>
      /// <typeparam name="E">>Type of the error response returned by the remote server</typeparam>
      /// <param name="request">Request sent to the remote server</param>
      /// <returns>The object of type T returned by the remote server</returns>
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
      /// Writes the response from the remote server to a text stream
      /// </summary>
      /// <param name="response">The response received from the remote server</param>
      /// <returns>A text stream object</returns>
      private static Stream GetResponseStream(WebResponse response)
      {
         var stream = response.GetResponseStream();
         if (response.Headers["Content-Encoding"] == "gzip")
         {  // if we received a gzipped response, handle that
            stream = new GZipStream(stream, CompressionMode.Decompress);
         }
         else if (response.Headers["Content-Encoding"] == "deflate")
         {  // if we received a deflated response, handle that
            stream = new DeflateStream(stream, CompressionMode.Decompress);
         }
         return stream;
      }
      #endregion

      #region json
      /// <summary>
      /// Helper function to create the request body as a JSON string
      /// </summary>
      /// <typeparam name="P">The type of the parameterObject</typeparam>
      /// <param name="obj">The object containing the request body parameters</param>
      /// <param name="simpleDictionary">Indicates if the passed object is a Dictionary</param>
      /// <returns>A JSON string representing the request body</returns>
      protected static string CreateJSONBody<P>(P parameterObject, bool simpleDictionary = false)
      {
         // trap this in case of forgetting
         if (typeof(P).GetInterfaces().Contains(typeof(IDictionary)))
            simpleDictionary = true;

         var settings = new DataContractJsonSerializerSettings();
         settings.UseSimpleDictionaryFormat = simpleDictionary;

         var jsonSerializer = new DataContractJsonSerializer(typeof(P), settings);
         using (var ms = new MemoryStream())
         {
            jsonSerializer.WriteObject(ms, parameterObject);
            var msBytes = ms.ToArray();
            return Encoding.UTF8.GetString(msBytes, 0, msBytes.Length);
         }
      }

      /// <summary>
      /// Serializes an object to a JSON string
      /// </summary>
      /// <param name="obj">the object to serialize</param>
      /// <returns>A JSON string representing the input object</returns>
      protected static string ConvertToJSON(object obj, bool ignoreNulls = true)
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
      /// Helper function to create the parameter string out of a dictionary of parameters
      /// </summary>
      /// <param name="requestParams">the parameters to convert</param>
      /// <returns>string containing all the parameters for use in requests</returns>
      protected static HttpWebRequest CreateHttpRequest(string uri, string method)
      {
         // Create the request
         HttpWebRequest request = WebRequest.CreateHttp(uri);
         request.Headers[HttpRequestHeader.Authorization] = "Bearer " + AccessToken;
         request.Headers[HttpRequestHeader.AcceptEncoding] = "gzip, deflate";
         request.Method = method;
         request.ContentType = "application/json";

         return request;
      }

      /// <summary>
      /// Converts a list of strings into a comma-separated values list (csv)
      /// </summary>
      /// <param name="items">The list of strings to convert to csv</param>
      /// <returns>A csv string of the list items</returns>
      protected static string GetCommaSeparatedString(List<string> items)
      {
         StringBuilder result = new StringBuilder();
         foreach (var item in items)
         {
            if (!result.ToString().Contains(item))
               result.Append(item + ",");
         }
         return result.ToString().Trim(',');
      }

      /// <summary>
      /// Helper function to create the query string out of a dictionary of parameters
      /// </summary>
      /// <param name="requestParams">the parameters to convert</param>
      /// <returns>string containing all the parameters for use in requests</returns>
      protected static string CreateQueryString(Dictionary<string, string> requestParams)
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
      /// <param name="input"></param>
      /// <returns>a Dictionary{string,string] object.</returns>
      public static Dictionary<string, string> ConvertToDictionary(object input)
      {
         if (input == null)
            return null;

         string json = ConvertToJSON(input, true);

         return JsonConvert.DeserializeObject<Dictionary<string, string>>(json);
      }
      #endregion
   }
}
