using Newtonsoft.Json;
using OkonkwoOandaV20.Framework;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Runtime.Serialization.Json;
using System.Text;

namespace OkonkwoOandaV20.TradeLibrary.REST
{
   internal class Request
   {
	  private string _uri;
	  public string Uri
	  {
		 get => $"{_uri}{CreateQueryString(Query)}";
		 set => _uri = value;
	  }

	  public string Method { get; set; }
	  public Parameters Parameters { get; set; }

	  public IDictionary<string, string> Headers
	  {
		 get
		 {
			var headers = Parameters?.GetRequestParameters<HeaderAttribute>();

			return headers?.ToDictionary(x => x.Key, x => x.Value.ToString());
		 }
	  }

	  public IDictionary<string, string> Query
	  {
		 get
		 {
			var queryProperties = Parameters?.GetRequestParameters<QueryAttribute>();

			return queryProperties?.ToDictionary(x => x.Key, x => x.Value.ToString());
		 }
	  }

	  public string Body
	  {
		 get
		 {
			var bodyProperties = Parameters?.GetRequestParameters<BodyAttribute>();

			if (bodyProperties?.Count > 0)
			{
			   var requestBody = ConvertToJson(bodyProperties);
			   return requestBody;
			}

			return null;
		 }
	  }

	  #region utilities

	  /// <summary>
	  /// Creates a query string from a dictionary of parameters
	  /// </summary>
	  /// <param name="queryParams">The parameters dictionary to convert to a query string.</param>
	  /// <returns>A query string</returns>
	  private static string CreateQueryString(IDictionary<string, string> queryParams, bool includeQuestionMark = true)
	  {
		 if (queryParams == null)
		 {
			return null;
		 }

		 if (queryParams.Count == 0)
		 {
			return null;
		 }

		 string queryString = "";
		 foreach (var pair in queryParams)
		 {
			queryString += $"{WebUtility.UrlEncode(pair.Key)}={WebUtility.UrlEncode(pair.Value)}&";
		 }
		 queryString = queryString.Trim('&');

		 var questionMark = includeQuestionMark ? "?" : null;

		 return $"{questionMark}{queryString}";
	  }

	  /// <summary>
	  /// Creates the request body as a Json string
	  /// </summary>
	  /// <typeparam name="P">The type of the parameterObject</typeparam>
	  /// <param name="parameterObject">The object containing the request body parameters</param>
	  /// <param name="simpleDictionary">Indicates if the passed object is a Dictionary</param>
	  /// <returns>A JSON string representing the request body</returns>
	  private static string ConvertToJson<P>(P parameterObject)
	  {
		 // for parameters passed as dictionaries
		 if (typeof(P).GetInterfaces().Contains(typeof(System.Collections.IDictionary)))
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
			return ConvertObjectToJson(parameterObject);
	  }

	  /// <summary>
	  /// Serializes an object to a Json string
	  /// </summary>
	  /// <param name="obj">The object to serialize</param>
	  /// <param name="ignoreNulls">Indicates if null properties should be excluded from the JSON output</param>
	  /// <returns>A JSON string representing the input object</returns>
	  private static string ConvertObjectToJson(object obj, bool ignoreNulls = true)
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
   }
}
