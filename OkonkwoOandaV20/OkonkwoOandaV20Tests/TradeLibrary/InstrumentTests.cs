﻿using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Linq;

namespace OkonkwoOandaV20Tests.TradeLibrary
{
   [TestClass]
   public class InstrumentTests : Rest20TestsBase
   {
	  #region Instrument

	  [TestMethod]
	  public void success_retrieve_candlestick_list()
	  {
		 var candlesRetrieved = Results.Items.FirstOrDefault(x => x.Key == "12.0").Value;
		 var candlesCountCorrect = Results.Items.FirstOrDefault(x => x.Key == "12.1").Value;

		 Assert.IsTrue(candlesRetrieved.Success, candlesRetrieved.Success.ToString() + ": " + candlesRetrieved.Details);
		 Assert.IsTrue(candlesCountCorrect.Success, candlesCountCorrect.Success.ToString() + ": " + candlesCountCorrect.Details);
	  }

	  [TestMethod]
	  public void success_retrieve_candlestick_details()
	  {
		 var candlesInstrumentCorrect = Results.Items.FirstOrDefault(x => x.Key == "12.2").Value;
		 var candlesGranularityCorrect = Results.Items.FirstOrDefault(x => x.Key == "12.3").Value;
		 var candlesHaveMidPrice = Results.Items.FirstOrDefault(x => x.Key == "12.4").Value;
		 var candlesHaveBidPrice = Results.Items.FirstOrDefault(x => x.Key == "12.5").Value;
		 var candlesHaveAskPrice = Results.Items.FirstOrDefault(x => x.Key == "12.6").Value;

		 Assert.IsTrue(candlesInstrumentCorrect.Success, candlesInstrumentCorrect.Success.ToString() + ": " + candlesInstrumentCorrect.Details);
		 Assert.IsTrue(candlesGranularityCorrect.Success, candlesGranularityCorrect.Success.ToString() + ": " + candlesGranularityCorrect.Details);
		 Assert.IsTrue(candlesHaveMidPrice.Success, candlesHaveMidPrice.Success.ToString() + ": " + candlesHaveMidPrice.Details);
		 Assert.IsTrue(candlesHaveBidPrice.Success, candlesHaveBidPrice.Success.ToString() + ": " + candlesHaveBidPrice.Details);
		 Assert.IsTrue(candlesHaveAskPrice.Success, candlesHaveAskPrice.Success.ToString() + ": " + candlesHaveAskPrice.Details);
	  }

	  #endregion
   }
}
