using OkonkwoOandaV20.Framework;
using System.Collections.Generic;
using System.Linq;
using System.Net;

namespace OkonkwoOandaV20.TradeLibrary.REST
{
   internal class Request
   {
	  public string Uri
	  {
		 get => $"{Uri}{CreateQueryString(Query)}";
		 set => Uri = value;
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

	  public IDictionary<string, object> Body
	  {
		 get
		 {
			var bodyProperties = Parameters?.GetRequestParameters<BodyAttribute>();

			return bodyProperties;
		 }
	  }

	  /// <summary>
	  /// Creates a query string from a dictionary of parameters
	  /// </summary>
	  /// <param name="queryParams">The parameters dictionary to convert to a query string.</param>
	  /// <returns>A query string</returns>
	  public static string CreateQueryString(IDictionary<string, string> queryParams, bool includeQuestionMark = true)
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
			queryString += WebUtility.UrlEncode(pair.Key) + "=" + WebUtility.UrlEncode(pair.Value) + "&";
		 }
		 queryString = queryString.Trim('&');

		 var questionMark = includeQuestionMark ? "?" : null;

		 return $"{questionMark}{queryString}";
	  }
   }
}
