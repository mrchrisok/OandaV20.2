using Newtonsoft.Json;
using OkonkwoOandaV20.Framework;
using OkonkwoOandaV20.Framework.JsonConverters;
using System.Collections.Generic;
using System.Linq;
using System.Net;

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
			   IList<JsonConverter> getConverters()
			   {
				  if (Parameters.AcceptDatetimeFormat.HasValue)
				  {
					 var acceptDateTimeConverter = new AcceptDateTimeConverter(Parameters.AcceptDatetimeFormat.Value);
					 return new List<JsonConverter>() { acceptDateTimeConverter };
				  }
				  return null;
			   }

			   var requestBody = ConvertToJson(bodyProperties, ignoreNulls: true, getConverters());

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
	  /// Serializes an object to a Json string
	  /// </summary>
	  /// <param name="obj">The object to serialize</param>
	  /// <param name="ignoreNulls">Indicates if null properties should be excluded from the JSON output</param>
	  /// <returns>A JSON string representing the input object</returns>
	  private static string ConvertToJson(object obj, bool ignoreNulls = true, IList<JsonConverter> converters = null)
	  {
		 var nullHandling = ignoreNulls ? NullValueHandling.Ignore : NullValueHandling.Include;

		 // oco: look into the DateFormatting
		 // might be able to use DateTime instead of string in objects
		 var settings = new JsonSerializerSettings()
		 {
			TypeNameHandling = TypeNameHandling.None,
			NullValueHandling = nullHandling,
			Converters = converters
		 };

		 string result = JsonConvert.SerializeObject(obj, settings);

		 return result;
	  }

	  #endregion
   }
}
