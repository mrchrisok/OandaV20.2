using Newtonsoft.Json;
using OkonkwoOandaV20.TradeLibrary.REST;
using System;

namespace OkonkwoOandaV20.Framework.JsonConverters
{
   public class AcceptDateTimeConverter : AbstractJsonConverter
   {

	  public AcceptDateTimeConverter(AcceptDatetimeFormat dateTimeFormat)
	  {
		 _acceptDateTimeFormat = dateTimeFormat;
	  }

	  private AcceptDatetimeFormat _acceptDateTimeFormat;

	  public override bool CanRead => false;

	  public override bool CanConvert(Type objectType)
	  {
		 return objectType == typeof(DateTime) || objectType == typeof(DateTime?);
	  }

	  public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
	  {
		 var dateTimeValue = reader.Value;
		 var dateTime = Utilities.ConvertAcceptDateFormatDateToDateTime(dateTimeValue, _acceptDateTimeFormat);

		 return dateTime;
	  }

	  public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
	  {
		 var convertedTime = Utilities.ConvertDateTimeToAcceptDateFormat((DateTime)value, _acceptDateTimeFormat);

		 writer.WriteValue(convertedTime);
	  }
   }
}
