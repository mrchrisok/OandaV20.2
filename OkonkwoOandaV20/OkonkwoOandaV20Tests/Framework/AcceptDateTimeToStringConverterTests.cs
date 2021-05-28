using Microsoft.VisualStudio.TestTools.UnitTesting;
using OkonkwoOandaV20.Framework;
using OkonkwoOandaV20.Framework.TypeConverters;
using OkonkwoOandaV20.TradeLibrary.REST;
using System;
using System.Text.RegularExpressions;

namespace OkonkwoOandaV20Tests.Framework
{
   [TestClass]
   public class AcceptDateTimeToStringConverterTests
   {
	  [TestMethod]
	  public void method_CanConvert_type_DateTime_success()
	  {
		 // arrange
		 var acceptDateTimeFormat = AcceptDatetimeFormat.RFC3339;
		 var converter = new AcceptDateTimeToStringConverter(acceptDateTimeFormat);

		 // act
		 var canConvertDateTime = converter.CanConvert(typeof(DateTime));
		 var canConvertDateTimeNullable = converter.CanConvert(typeof(DateTime?));

		 // assert
		 Assert.IsTrue(canConvertDateTime);
		 Assert.IsTrue(canConvertDateTimeNullable);
	  }

	  [TestMethod]
	  public void method_CanConvert_type_not_DateTime_failure()
	  {
		 // arrange
		 var acceptDateTimeFormat = AcceptDatetimeFormat.RFC3339;
		 var converter = new AcceptDateTimeToStringConverter(acceptDateTimeFormat);

		 // act
		 var canConvertObject = converter.CanConvert(typeof(object));

		 // assert
		 Assert.IsFalse(canConvertObject);
	  }

	  [TestMethod]
	  public void method_ToOuput_RFC3339_returns_correct_value()
	  {
		 // arrange
		 var datePattern = "([0-9]+)-(0[1-9]|1[012])-(0[1-9]|[12][0-9]|3[01])";
		 var hourPattern = "([01][0-9]|2[0-3])";
		 var minutePattern = "([0-5][0-9])";
		 var secondsPattern = "([0-5][0-9]|60)(\\.[0-9]+)";
		 var offsetPattern = "([\\+|\\-]([01][0-9]|2[0-3]):[0-5][0-9])";

		 var regexRFC3339 = new Regex($"^{datePattern}[Tt]{hourPattern}:{minutePattern}:{secondsPattern}?(([Zz])|{offsetPattern})$");

		 var acceptDateTimeFormat = AcceptDatetimeFormat.RFC3339;
		 var converter = new AcceptDateTimeToStringConverter(acceptDateTimeFormat);
		 var testDateTime = DateTime.UtcNow;

		 // act
		 var convertedTestDateTime = converter.ToOutput(testDateTime);
		 var convertedToCorrectFormat = regexRFC3339.IsMatch(convertedTestDateTime);
		 var restoredTestDateTime = Utilities.ConvertAcceptDateFormatDateToDateTimeUtc(convertedTestDateTime, acceptDateTimeFormat);

		 // assert
		 Assert.IsTrue(convertedToCorrectFormat);
		 Assert.AreEqual(testDateTime, restoredTestDateTime);
	  }

	  [TestMethod]
	  public void method_ToOuput_Unix_returns_correct_value()
	  {
		 // arrange
		 var secondsPattern = "([0-9]+)";
		 var milliSecondsPattern = "([0-9]+)";

		 var regexUnix = new Regex($"^{secondsPattern}.{milliSecondsPattern}$");

		 var acceptDateTimeFormat = AcceptDatetimeFormat.Unix;
		 var converter = new AcceptDateTimeToStringConverter(acceptDateTimeFormat);
		 var testDateTime = DateTime.UtcNow;

		 // act
		 var convertedTestDateTime = converter.ToOutput(testDateTime);
		 var convertedToCorrectFormat = regexUnix.IsMatch(convertedTestDateTime);
		 var restoredTestDateTime = Utilities.ConvertAcceptDateFormatDateToDateTimeUtc(convertedTestDateTime, acceptDateTimeFormat);

		 // assert
		 Assert.IsTrue(convertedToCorrectFormat);
		 Assert.AreEqual(testDateTime.Date, restoredTestDateTime.Date);
		 Assert.AreEqual(testDateTime.Hour, restoredTestDateTime.Hour);
		 Assert.AreEqual(testDateTime.Minute, restoredTestDateTime.Minute);
		 Assert.AreEqual(testDateTime.Second, restoredTestDateTime.Second);
		 Assert.IsTrue(Math.Abs(testDateTime.Millisecond - restoredTestDateTime.Millisecond) <= 1);
	  }
   }
}
