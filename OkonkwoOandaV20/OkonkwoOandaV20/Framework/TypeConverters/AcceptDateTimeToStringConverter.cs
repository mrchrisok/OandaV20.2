using OkonkwoOandaV20.TradeLibrary.REST;
using System;

namespace OkonkwoOandaV20.Framework.TypeConverters
{
   public class AcceptDateTimeToStringConverter : ITypeConverter<string>
   {
	  public AcceptDateTimeToStringConverter(AcceptDatetimeFormat dateTimeFormat)
	  {
		 _acceptDateTimeFormat = dateTimeFormat;
	  }

	  private AcceptDatetimeFormat _acceptDateTimeFormat;

	  public bool CanConvert(Type objectType)
	  {
		 return objectType == typeof(DateTime) || objectType == typeof(DateTime?);
	  }

	  public string ToOutput(object input)
	  {
		 if (input == null)
		 {
			return null;
		 }

		 if (input is DateTime dateTime)
		 {
			var output = Utilities.ConvertDateTimeToAcceptDateFormat(dateTime, _acceptDateTimeFormat);

			return output;
		 }

		 throw new ArgumentException($"The input type '{input.GetType().Name}' is invalid.");
	  }
   }
}
