using OkonkwoOandaV20.TradeLibrary.REST;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using static OkonkwoOandaV20.TradeLibrary.REST.Rest20;

namespace OkonkwoOandaV20.Framework
{
   public class Utilities
   {
	  /// <summary>
	  /// Determines if trading is halted for the provided instrument.
	  /// </summary>
	  /// <param name="instrument">Instrument to check if halted. Default is EUR_USD.</param>
	  /// <returns>True if trading is halted, false if trading is not halted.</returns>
	  public static async Task<bool> IsMarketHaltedAsync(string instrument = InstrumentName.Currency.EURUSD)
	  {
		 var accountID = Credentials.GetDefaultCredentials().DefaultAccountId;
		 var parameters = new PricingParameters() { instruments = new List<string>() { instrument } };

		 var prices = await Rest20.GetPricingAsync(accountID, parameters);

		 bool isTradeable = false, hasBids = false, hasAsks = false;

		 if (prices[0] != null)
		 {
			isTradeable = prices[0].tradeable;
			hasBids = prices[0].bids.Count > 0;
			hasAsks = prices[0].asks.Count > 0;
		 }

		 return !(isTradeable && hasBids && hasAsks);
	  }

	  /// <summary>
	  /// Convert DateTime object to a string of the indicated format
	  /// </summary>
	  /// <param name="time">A DateTime object</param>
	  /// <param name="format">Format type (RFC3339 or UNIX only)</param>
	  /// <returns>A date-time string</returns>
	  public static string ConvertDateTimeToAcceptDateFormat(DateTime time, AcceptDatetimeFormat format = AcceptDatetimeFormat.RFC3339)
	  {
		 if (format == AcceptDatetimeFormat.RFC3339)
			return XmlConvert.ToString(time, "yyyy-MM-ddTHH:mm:ssZ");
		 else if (format == AcceptDatetimeFormat.Unix)
			return ((int)time.Subtract(new DateTime(1970, 1, 1)).TotalSeconds).ToString();
		 else
			throw new ArgumentException($"The format parameter '{format}' is not supported.");
	  }

	  /// <summary>
	  /// Convert formatted time string to DateTime
	  /// </summary>
	  /// <param name="time">A formatted time string</param>
	  /// <param name="format">Format type (RFC3339 or UNIX only)</param>
	  /// <returns>A DateTime object. Utc only.</returns>
	  public static DateTime ConvertAcceptDateFormatDateToDateTime(object time, AcceptDatetimeFormat format)
	  {
		 if (format == AcceptDatetimeFormat.RFC3339)
		 {
			var dateTime = DateTime.Parse((string)time).ToUniversalTime();
			return dateTime;
		 }
		 else if (format == AcceptDatetimeFormat.Unix)
		 {
			long timeSeconds = Convert.ToInt64(time);
			var dateTime = DateTimeOffset.FromUnixTimeSeconds(timeSeconds).UtcDateTime;
			return dateTime;
		 }
		 else
			throw new ArgumentException($"The format parameter '{format}' is not supported.");
	  }

	  /// <summary>
	  /// Converts a list of strings into a comma-separated values list (csv)
	  /// </summary>
	  /// <param name="items">The list of strings to convert to csv</param>
	  /// <returns>A csv string of the list items</returns>
	  public static string ConvertListToDelimitedValues(System.Collections.IList items, string delimiter = ",")
	  {
		 var stringBuilder = new StringBuilder();
		 foreach (var item in items)
		 {
			if (!stringBuilder.ToString().Contains(item.ToString()))
			   stringBuilder.Append(item + delimiter);
		 }
		 return stringBuilder.ToString().Trim(delimiter.ToCharArray());
	  }
   }
}
