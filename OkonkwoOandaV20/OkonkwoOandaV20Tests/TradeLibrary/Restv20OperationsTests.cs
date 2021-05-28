﻿using Microsoft.VisualStudio.TestTools.UnitTesting;
using OkonkwoOandaV20.TradeLibrary.REST;
using System.Linq;
using System.Threading.Tasks;

namespace OkonkwoOandaV20Tests.TradeLibrary
{
   [TestClass]
   public partial class Restv20OperationsTests : Restv20TestsBase
   {
	  [TestInitialize]
	  public void CheckIfAllApiOperationsHaveCompleted()
	  {
		 while (!m_ApiOperationsComplete) Task.Delay(250).Wait();
	  }

	  #region Credentials

	  [TestMethod]
	  public void test_Credentials_get_credentials()
	  {
		 Assert.IsTrue(Rest20.Credentials.GetDefaultCredentials().Environment == m_TestEnvironment, "Credentials Environment is incorrect.");
		 Assert.IsTrue(Rest20.Credentials.GetDefaultCredentials().AccessToken == m_TestToken, "Credentials Token is incorrect.");
		 Assert.IsTrue(Rest20.Credentials.GetDefaultCredentials().DefaultAccountId == m_TestAccount, "Credentials AccountId is incorrect.");
	  }

	  #endregion

	  #region Account
	  [TestMethod]
	  public void test_Account_retrieve_accounts_list()
	  {
		 var accountsRetrieved = m_Results.Items.FirstOrDefault(x => x.Key == "01.0").Value;

		 Assert.IsTrue(accountsRetrieved.Success, accountsRetrieved.Success.ToString() + ": " + accountsRetrieved.Details);
	  }

	  [TestMethod]
	  public void test_Account_retrieve_correct_number_of_accounts()
	  {
		 string key = "01.1";
		 var result = m_Results.Items.FirstOrDefault(x => x.Key == key).Value;

		 Assert.IsTrue(result.Success, result.Success.ToString() + ": " + result.Details);
	  }

	  [TestMethod]
	  public void test_Account_account_numbers_have_correct_format()
	  {
		 string key = "01.";
		 var results = m_Results.Items.Where(x => x.Key.StartsWith(key) && x.Key != "01.0" && x.Key != "01.1");
		 var failure = results.FirstOrDefault(x => x.Value.Success == false);

		 string message = failure.Key != null ? failure.Value.Success.ToString() + ": " + failure.Value.Details : "";

		 Assert.IsTrue(failure.Key == null, failure.Key + ": " + message);
	  }

	  [TestMethod]
	  public void test_Account_retrieve_account_detail_info()
	  {
		 // 08
		 var accountRetrieved = m_Results.Items.FirstOrDefault(x => x.Key == "08.0").Value;
		 var idIsCorrect = m_Results.Items.FirstOrDefault(x => x.Key == "08.1").Value;
		 var correctCurrency = m_Results.Items.FirstOrDefault(x => x.Key == "08.2").Value;
		 var correctOpenTradeCount = m_Results.Items.FirstOrDefault(x => x.Key == "08.3").Value;
		 var tradesMatchTradeCount = m_Results.Items.FirstOrDefault(x => x.Key == "08.4").Value;

		 Assert.IsTrue(accountRetrieved.Success, $"08.0,{accountRetrieved.Success}: {accountRetrieved.Details}");
		 Assert.IsTrue(idIsCorrect.Success, $"08.1,{idIsCorrect.Success}: {idIsCorrect.Details}");
		 Assert.IsTrue(correctCurrency.Success, $"08.2,{correctCurrency.Success}: {correctCurrency.Details}");
		 Assert.IsTrue(correctOpenTradeCount.Success, $"08.3,{correctOpenTradeCount.Success}: {correctOpenTradeCount.Details}");
		 Assert.IsTrue(tradesMatchTradeCount.Success, $"08.4,{tradesMatchTradeCount.Success}: {tradesMatchTradeCount.Details}");
	  }

	  [TestMethod]
	  public void test_Account_retrieve_instruments_list()
	  {
		 string key = "02.0";
		 var results = m_Results.Items.Where(x => x.Key == key);
		 var failure = results.FirstOrDefault(x => x.Value.Success == false);

		 Assert.IsTrue(failure.Key == null, failure.Key + ": " + failure.Value);
	  }

	  [TestMethod]
	  public void test_Account_retrieve_single_instrument_info()
	  {
		 // 03
		 var instrumentReceived = m_Results.Items.FirstOrDefault(x => x.Key == "03.0").Value;
		 var instrumentTypeCorrect = m_Results.Items.FirstOrDefault(x => x.Key == "03.1").Value;
		 var instrumentNameCorrect = m_Results.Items.FirstOrDefault(x => x.Key == "03.2").Value;

		 Assert.IsTrue(instrumentReceived.Success, instrumentReceived.Success.ToString() + ": " + instrumentReceived.Details);
		 Assert.IsTrue(instrumentTypeCorrect.Success, instrumentTypeCorrect.Success.ToString() + ": " + instrumentTypeCorrect.Details);
		 Assert.IsTrue(instrumentNameCorrect.Success, instrumentNameCorrect.Success.ToString() + ": " + instrumentNameCorrect.Details);
	  }

	  [TestMethod]
	  public void test_Account_retrieve_account_summary_info()
	  {
		 // 04
		 var summaryRetrieved = m_Results.Items.FirstOrDefault(x => x.Key == "04.0").Value;
		 var accountIdIsCorrect = m_Results.Items.FirstOrDefault(x => x.Key == "04.1").Value;
		 var currencyIsCorrect = m_Results.Items.FirstOrDefault(x => x.Key == "04.2").Value;

		 Assert.IsTrue(summaryRetrieved.Success, summaryRetrieved.Success.ToString() + ": " + summaryRetrieved.Details);
		 Assert.IsTrue(accountIdIsCorrect.Success, accountIdIsCorrect.Success.ToString() + ": " + accountIdIsCorrect.Details);
		 Assert.IsTrue(currencyIsCorrect.Success, currencyIsCorrect.Success.ToString() + ": " + currencyIsCorrect.Details);
	  }

	  [TestMethod]
	  public void test_Account_patch_account_configuration()
	  {
		 // 05
		 var configRetrieved = m_Results.Items.FirstOrDefault(x => x.Key == "05.0").Value;
		 var aliasChanged = m_Results.Items.FirstOrDefault(x => x.Key == "05.1").Value;
		 var marginRateChanged = m_Results.Items.FirstOrDefault(x => x.Key == "05.2").Value;
		 var aliasReverted = m_Results.Items.FirstOrDefault(x => x.Key == "05.3").Value;
		 var marginRateReverted = m_Results.Items.FirstOrDefault(x => x.Key == "05.4").Value;

		 Assert.IsTrue(configRetrieved.Success, configRetrieved.Success.ToString() + ": " + configRetrieved.Details);
		 Assert.IsTrue(aliasChanged.Success, aliasChanged.Success.ToString() + ": " + aliasChanged.Details);
		 Assert.IsTrue(marginRateChanged.Success, marginRateChanged.Success.ToString() + ": " + marginRateChanged.Details);
		 Assert.IsTrue(aliasReverted.Success, aliasReverted.Success.ToString() + ": " + aliasReverted.Details);
		 Assert.IsTrue(marginRateReverted.Success, marginRateReverted.Success.ToString() + ": " + marginRateReverted.Details);
	  }

	  [TestMethod]
	  public void test_Account_retrieve_account_changes()
	  {
		 var changesRetrieved = m_Results.Items.FirstOrDefault(x => x.Key == "17.0").Value;
		 var ordersFilledReceived = m_Results.Items.FirstOrDefault(x => x.Key == "17.1").Value;
		 var ordersCancelledReceived = m_Results.Items.FirstOrDefault(x => x.Key == "17.2").Value;
		 var tradesClosedReceived = m_Results.Items.FirstOrDefault(x => x.Key == "17.3").Value;
		 var positionsReceived = m_Results.Items.FirstOrDefault(x => x.Key == "17.4").Value;
		 var accountHasMargin = m_Results.Items.FirstOrDefault(x => x.Key == "17.5").Value;

		 Assert.IsTrue(changesRetrieved.Success, changesRetrieved.Success.ToString() + ": " + changesRetrieved.Details);
		 Assert.IsTrue(ordersFilledReceived.Success, ordersFilledReceived.Success.ToString() + ": " + ordersFilledReceived.Details);
		 Assert.IsTrue(ordersCancelledReceived.Success, ordersCancelledReceived.Success.ToString() + ": " + ordersCancelledReceived.Details);
		 Assert.IsTrue(tradesClosedReceived.Success, tradesClosedReceived.Success.ToString() + ": " + tradesClosedReceived.Details);
		 Assert.IsTrue(positionsReceived.Success, positionsReceived.Success.ToString() + ": " + positionsReceived.Details);
		 Assert.IsTrue(accountHasMargin.Success, accountHasMargin.Success.ToString() + ": " + accountHasMargin.Details);
	  }
	  #endregion

	  #region Instrument
	  [TestMethod]
	  public void test_Instrument_retrieve_candlestick_list()
	  {
		 var candlesRetrieved = m_Results.Items.FirstOrDefault(x => x.Key == "12.0").Value;
		 var candlesCountCorrect = m_Results.Items.FirstOrDefault(x => x.Key == "12.1").Value;

		 Assert.IsTrue(candlesRetrieved.Success, candlesRetrieved.Success.ToString() + ": " + candlesRetrieved.Details);
		 Assert.IsTrue(candlesCountCorrect.Success, candlesCountCorrect.Success.ToString() + ": " + candlesCountCorrect.Details);
	  }

	  [TestMethod]
	  public void test_Instrument_retrieve_candlestick_details()
	  {
		 var candlesInstrumentCorrect = m_Results.Items.FirstOrDefault(x => x.Key == "12.2").Value;
		 var candlesGranularityCorrect = m_Results.Items.FirstOrDefault(x => x.Key == "12.3").Value;
		 var candlesHaveMidPrice = m_Results.Items.FirstOrDefault(x => x.Key == "12.4").Value;
		 var candlesHaveBidPrice = m_Results.Items.FirstOrDefault(x => x.Key == "12.5").Value;
		 var candlesHaveAskPrice = m_Results.Items.FirstOrDefault(x => x.Key == "12.6").Value;

		 Assert.IsTrue(candlesInstrumentCorrect.Success, candlesInstrumentCorrect.Success.ToString() + ": " + candlesInstrumentCorrect.Details);
		 Assert.IsTrue(candlesGranularityCorrect.Success, candlesGranularityCorrect.Success.ToString() + ": " + candlesGranularityCorrect.Details);
		 Assert.IsTrue(candlesHaveMidPrice.Success, candlesHaveMidPrice.Success.ToString() + ": " + candlesHaveMidPrice.Details);
		 Assert.IsTrue(candlesHaveBidPrice.Success, candlesHaveBidPrice.Success.ToString() + ": " + candlesHaveBidPrice.Details);
		 Assert.IsTrue(candlesHaveAskPrice.Success, candlesHaveAskPrice.Success.ToString() + ": " + candlesHaveAskPrice.Details);
	  }
	  #endregion

	  #region Order
	  [TestMethod]
	  public void test_Order_create_order()
	  {
		 var orderCreated = m_Results.Items.FirstOrDefault(x => x.Key == "11.0").Value;
		 var orderTypeIsCorrect = m_Results.Items.FirstOrDefault(x => x.Key == "11.1").Value;

		 Assert.IsTrue(orderCreated.Success, orderCreated.Success.ToString() + ": " + orderCreated.Details);
		 Assert.IsTrue(orderTypeIsCorrect.Success, orderTypeIsCorrect.Success.ToString() + ": " + orderTypeIsCorrect.Details);
	  }

	  [TestMethod]
	  public void test_Order_get_all_orders()
	  {
		 var allOrdersRetrieved = m_Results.Items.FirstOrDefault(x => x.Key == "11.2").Value;
		 var testOrderTypeReturned = m_Results.Items.FirstOrDefault(x => x.Key == "11.3").Value;
		 var allOrdersByIDRetrieved = m_Results.Items.FirstOrDefault(x => x.Key == "11.29").Value;

		 Assert.IsTrue(allOrdersRetrieved.Success, allOrdersRetrieved.Success.ToString() + ": " + allOrdersRetrieved.Details);
		 Assert.IsTrue(testOrderTypeReturned.Success, testOrderTypeReturned.Success.ToString() + ": " + testOrderTypeReturned.Details);
		 Assert.IsTrue(allOrdersByIDRetrieved.Success, $"11.29,{allOrdersByIDRetrieved.Success}: { allOrdersByIDRetrieved.Details}");
	  }

	  [TestMethod]
	  public void test_Order_get_pending_orders()
	  {
		 var pendingOrdersRetrieved = m_Results.Items.FirstOrDefault(x => x.Key == "11.4").Value;
		 var onlyPendingOrderRetrieved = m_Results.Items.FirstOrDefault(x => x.Key == "11.5").Value;
		 var testPendingOrderRetrieved = m_Results.Items.FirstOrDefault(x => x.Key == "11.6").Value;

		 Assert.IsTrue(pendingOrdersRetrieved.Success, pendingOrdersRetrieved.Success.ToString() + ": " + pendingOrdersRetrieved.Details);
		 Assert.IsTrue(onlyPendingOrderRetrieved.Success, onlyPendingOrderRetrieved.Success.ToString() + ": " + onlyPendingOrderRetrieved.Details);
		 Assert.IsTrue(testPendingOrderRetrieved.Success, testPendingOrderRetrieved.Success.ToString() + ": " + testPendingOrderRetrieved.Details);
	  }

	  [TestMethod]
	  public void test_Order_get_order_details()
	  {
		 var orderDetailsRetrieved = m_Results.Items.FirstOrDefault(x => x.Key == "11.7").Value;
		 var clientExtensionsRetrieved = m_Results.Items.FirstOrDefault(x => x.Key == "11.8").Value;
		 var tradeExtensionsRetrieved = m_Results.Items.FirstOrDefault(x => x.Key == "11.9").Value;

		 Assert.IsTrue(orderDetailsRetrieved.Success, orderDetailsRetrieved.Success.ToString() + ": " + orderDetailsRetrieved.Details);
		 Assert.IsTrue(clientExtensionsRetrieved.Success, clientExtensionsRetrieved.Success.ToString() + ": " + clientExtensionsRetrieved.Details);
		 Assert.IsTrue(tradeExtensionsRetrieved.Success, tradeExtensionsRetrieved.Success.ToString() + ": " + tradeExtensionsRetrieved.Details);
	  }

	  [TestMethod]
	  public void test_Order_update_order_extensions()
	  {
		 var extensionsUpdated = m_Results.Items.FirstOrDefault(x => x.Key == "11.10").Value;
		 var extensionsOrderCorrect = m_Results.Items.FirstOrDefault(x => x.Key == "11.11").Value;
		 var clientExtensionCommentCorrect = m_Results.Items.FirstOrDefault(x => x.Key == "11.12").Value;
		 var tradeExtensionCommentCorrect = m_Results.Items.FirstOrDefault(x => x.Key == "11.13").Value;

		 Assert.IsTrue(extensionsUpdated.Success, extensionsUpdated.Success.ToString() + ": " + extensionsUpdated.Details);
		 Assert.IsTrue(extensionsOrderCorrect.Success, extensionsOrderCorrect.Success.ToString() + ": " + extensionsOrderCorrect.Details);
		 Assert.IsTrue(clientExtensionCommentCorrect.Success, clientExtensionCommentCorrect.Success.ToString() + ": " + clientExtensionCommentCorrect.Details);
		 Assert.IsTrue(tradeExtensionCommentCorrect.Success, tradeExtensionCommentCorrect.Success.ToString() + ": " + tradeExtensionCommentCorrect.Details);
	  }

	  [TestMethod]
	  public void test_Order_cancel_replace_order()
	  {
		 var orderWasCancelled = m_Results.Items.FirstOrDefault(x => x.Key == "11.14").Value;
		 var orderWasReplaced = m_Results.Items.FirstOrDefault(x => x.Key == "11.15").Value;
		 var newOrderDetailsCorrect = m_Results.Items.FirstOrDefault(x => x.Key == "11.16").Value;

		 Assert.IsTrue(orderWasCancelled.Success, orderWasCancelled.Success.ToString() + ": " + orderWasCancelled.Details);
		 Assert.IsTrue(orderWasReplaced.Success, orderWasReplaced.Success.ToString() + ": " + orderWasReplaced.Details);
		 Assert.IsTrue(newOrderDetailsCorrect.Success, newOrderDetailsCorrect.Success.ToString() + ": " + newOrderDetailsCorrect.Details);
	  }

	  [TestMethod]
	  public void test_Order_cancel_order()
	  {
		 var cancelledOrderReturned = m_Results.Items.FirstOrDefault(x => x.Key == "11.17").Value;
		 var orderWasCancelled = m_Results.Items.FirstOrDefault(x => x.Key == "11.18").Value;

		 Assert.IsTrue(cancelledOrderReturned.Success, cancelledOrderReturned.Success.ToString() + ": " + cancelledOrderReturned.Details);
		 Assert.IsTrue(orderWasCancelled.Success, orderWasCancelled.Success.ToString() + ": " + orderWasCancelled.Details);
	  }

	  [TestMethod]
	  public void test_Order_cancel_replace_order_with_exit_orders()
	  {
		 var orderWasCreated = m_Results.Items.FirstOrDefault(x => x.Key == "11.19").Value;
		 var orderTypeIsCorrect = m_Results.Items.FirstOrDefault(x => x.Key == "11.20").Value;
		 var orderHasStopLoss = m_Results.Items.FirstOrDefault(x => x.Key == "11.21").Value;
		 var orderHasTakeProfit = m_Results.Items.FirstOrDefault(x => x.Key == "11.22").Value;
		 var orderWasCancelled = m_Results.Items.FirstOrDefault(x => x.Key == "11.23").Value;
		 var cancelledOrderHasCorrectReason = m_Results.Items.FirstOrDefault(x => x.Key == "11.24").Value;
		 var newOrderWasCreated = m_Results.Items.FirstOrDefault(x => x.Key == "11.25").Value;
		 var newOrderReasonIsCorrect = m_Results.Items.FirstOrDefault(x => x.Key == "11.26").Value;
		 var newOrderStopLossIsCorrect = m_Results.Items.FirstOrDefault(x => x.Key == "11.27").Value;
		 var newOrderTakeProfitIsCorrect = m_Results.Items.FirstOrDefault(x => x.Key == "11.28").Value;

		 Assert.IsTrue(orderWasCreated.Success, orderWasCreated.Success.ToString() + ": " + orderWasCreated.Details);
		 Assert.IsTrue(orderTypeIsCorrect.Success, orderTypeIsCorrect.Success.ToString() + ": " + orderTypeIsCorrect.Details);
		 Assert.IsTrue(orderHasStopLoss.Success, $"11.21,{orderHasStopLoss.Success}: {orderHasStopLoss.Details}");
		 Assert.IsTrue(orderHasTakeProfit.Success, orderHasTakeProfit.Success.ToString() + ": " + orderHasTakeProfit.Details);
		 Assert.IsTrue(orderWasCancelled.Success, orderWasCancelled.Success.ToString() + ": " + orderWasCancelled.Details);
		 Assert.IsTrue(cancelledOrderHasCorrectReason.Success, cancelledOrderHasCorrectReason.Success.ToString() + ": " + cancelledOrderHasCorrectReason.Details);
		 Assert.IsTrue(newOrderWasCreated.Success, newOrderWasCreated.Success.ToString() + ": " + newOrderWasCreated.Details);
		 Assert.IsTrue(newOrderReasonIsCorrect.Success, newOrderReasonIsCorrect.Success.ToString() + ": " + newOrderReasonIsCorrect.Details);
		 Assert.IsTrue(newOrderStopLossIsCorrect.Success, newOrderStopLossIsCorrect.Success.ToString() + ": " + newOrderStopLossIsCorrect.Details);
		 Assert.IsTrue(newOrderTakeProfitIsCorrect.Success, newOrderTakeProfitIsCorrect.Success.ToString() + ": " + newOrderTakeProfitIsCorrect.Details);
	  }
	  #endregion

	  #region Trade
	  [TestMethod]
	  public void test_Trade_place_market_order()
	  {
		 var marketOrderCreated = m_Results.Items.FirstOrDefault(x => x.Key == "13.0").Value;
		 var marketOrderFilled = m_Results.Items.FirstOrDefault(x => x.Key == "13.1").Value;

		 Assert.IsTrue(marketOrderCreated.Success, marketOrderCreated.Success.ToString() + ": " + marketOrderCreated.Details);
		 Assert.IsTrue(marketOrderFilled.Success, marketOrderFilled.Success.ToString() + ": " + marketOrderFilled.Details);
	  }

	  [TestMethod]
	  public void test_Trade_get_trades_list()
	  {
		 var allTradesRetrieved = m_Results.Items.FirstOrDefault(x => x.Key == "13.2").Value;
		 var openTradesRetrieved = m_Results.Items.FirstOrDefault(x => x.Key == "13.3").Value;

		 Assert.IsTrue(allTradesRetrieved.Success, allTradesRetrieved.Success.ToString() + ": " + allTradesRetrieved.Details);
		 Assert.IsTrue(openTradesRetrieved.Success, openTradesRetrieved.Success.ToString() + ": " + openTradesRetrieved.Details);
	  }

	  [TestMethod]
	  public void test_Trade_get_trade_details()
	  {
		 var detailsRetrieved = m_Results.Items.FirstOrDefault(x => x.Key == "13.4").Value;

		 Assert.IsTrue(detailsRetrieved.Success, detailsRetrieved.Success.ToString() + ": " + detailsRetrieved.Details);
	  }

	  [TestMethod]
	  public void test_Trade_update_trade_extensions()
	  {
		 var extensionsUpdated = m_Results.Items.FirstOrDefault(x => x.Key == "13.5").Value;
		 var extensionsTradeCorrect = m_Results.Items.FirstOrDefault(x => x.Key == "13.6").Value;
		 var tradeExtensionCommentCorrect = m_Results.Items.FirstOrDefault(x => x.Key == "13.7").Value;

		 Assert.IsTrue(extensionsUpdated.Success, extensionsUpdated.Success.ToString() + ": " + extensionsUpdated.Details);
		 Assert.IsTrue(extensionsTradeCorrect.Success, extensionsTradeCorrect.Success.ToString() + ": " + extensionsTradeCorrect.Details);
		 Assert.IsTrue(tradeExtensionCommentCorrect.Success, tradeExtensionCommentCorrect.Success.ToString() + ": " + tradeExtensionCommentCorrect.Details);
	  }

	  [TestMethod]
	  public void test_Trade_patch_exit_orders()
	  {
		 var takeProfitPatched = m_Results.Items.FirstOrDefault(x => x.Key == "13.8").Value;
		 var takeProfitPriceCorrect = m_Results.Items.FirstOrDefault(x => x.Key == "13.9").Value;
		 var stopLossPatched = m_Results.Items.FirstOrDefault(x => x.Key == "13.10").Value;
		 var stopLossPriceCorrect = m_Results.Items.FirstOrDefault(x => x.Key == "13.11").Value;
		 var trailingStopLossPatched = m_Results.Items.FirstOrDefault(x => x.Key == "13.12").Value;
		 var trailingStopLossDistanceCorrect = m_Results.Items.FirstOrDefault(x => x.Key == "13.13").Value;

		 var takeProfitCancelled = m_Results.Items.FirstOrDefault(x => x.Key == "13.14").Value;
		 var stopLossCancelled = m_Results.Items.FirstOrDefault(x => x.Key == "13.15").Value;
		 var trailingStopLossCancelled = m_Results.Items.FirstOrDefault(x => x.Key == "13.16").Value;

		 Assert.IsTrue(takeProfitPatched.Success, takeProfitPatched.Success.ToString() + ": " + takeProfitPatched.Details);
		 Assert.IsTrue(takeProfitPriceCorrect.Success, $"13.9,{takeProfitPriceCorrect.Success}: {takeProfitPriceCorrect.Details}");
		 Assert.IsTrue(stopLossPatched.Success, stopLossPatched.Success.ToString() + ": " + stopLossPatched.Details);
		 Assert.IsTrue(stopLossPriceCorrect.Success, stopLossPriceCorrect.Success.ToString() + ": " + stopLossPriceCorrect.Details);
		 Assert.IsTrue(trailingStopLossPatched.Success, trailingStopLossPatched.Success.ToString() + ": " + trailingStopLossPatched.Details);
		 Assert.IsTrue(trailingStopLossDistanceCorrect.Success, trailingStopLossDistanceCorrect.Success.ToString() + ": " + trailingStopLossDistanceCorrect.Details);
		 Assert.IsTrue(takeProfitCancelled.Success, takeProfitCancelled.Success.ToString() + ": " + takeProfitCancelled.Details);
		 Assert.IsTrue(stopLossCancelled.Success, stopLossCancelled.Success.ToString() + ": " + stopLossCancelled.Details);
		 Assert.IsTrue(trailingStopLossCancelled.Success, trailingStopLossCancelled.Success.ToString() + ": " + trailingStopLossCancelled.Details);
	  }

	  [TestMethod]
	  public void test_Trade_close_open_trade()
	  {
		 var tradeClosed = m_Results.Items.FirstOrDefault(x => x.Key == "13.17").Value;
		 var tradeCloseHasTime = m_Results.Items.FirstOrDefault(x => x.Key == "13.18").Value;
		 var tradeCloseHasInstrument = m_Results.Items.FirstOrDefault(x => x.Key == "13.19").Value;
		 var tradeCloseUnitsCorrect = m_Results.Items.FirstOrDefault(x => x.Key == "13.20").Value;
		 var tradeCloseHasPrice = m_Results.Items.FirstOrDefault(x => x.Key == "13.21").Value;

		 Assert.IsTrue(tradeClosed.Success, tradeClosed.Success.ToString() + ": " + tradeClosed.Details);
		 Assert.IsTrue(tradeCloseHasTime.Success, tradeCloseHasTime.Success.ToString() + ": " + tradeCloseHasTime.Details);
		 Assert.IsTrue(tradeCloseHasInstrument.Success, tradeCloseHasInstrument.Success.ToString() + ": " + tradeCloseHasInstrument.Details);
		 Assert.IsTrue(tradeCloseUnitsCorrect.Success, tradeCloseUnitsCorrect.Success.ToString() + ": " + tradeCloseUnitsCorrect.Details);
		 Assert.IsTrue(tradeCloseHasPrice.Success, tradeCloseHasPrice.Success.ToString() + ": " + tradeCloseHasPrice.Details);
	  }
	  #endregion

	  #region Position
	  [TestMethod]
	  public void test_Position_get_positions()
	  {
		 // 14.0 & 14.1 do not need to be tested.

		 var allPositionsRetrieved = m_Results.Items.FirstOrDefault(x => x.Key == "14.2").Value;
		 var openPositionsRetrieved = m_Results.Items.FirstOrDefault(x => x.Key == "14.3").Value;

		 Assert.IsTrue(allPositionsRetrieved.Success, allPositionsRetrieved.Success.ToString() + ": " + allPositionsRetrieved.Details);
		 Assert.IsTrue(openPositionsRetrieved.Success, openPositionsRetrieved.Success.ToString() + ": " + openPositionsRetrieved.Details);
	  }

	  [TestMethod]
	  public void test_Position_get_open_positions()
	  {
		 var postionHasDirection = m_Results.Items.FirstOrDefault(x => x.Key == "14.4").Value;
		 var positionHasUnits = m_Results.Items.FirstOrDefault(x => x.Key == "14.5").Value;
		 var positionHasAvgPrice = m_Results.Items.FirstOrDefault(x => x.Key == "14.6").Value;
		 var positionHasInstrument = m_Results.Items.FirstOrDefault(x => x.Key == "14.7").Value;

		 Assert.IsTrue(postionHasDirection.Success, postionHasDirection.Success.ToString() + ": " + postionHasDirection.Details);
		 Assert.IsTrue(positionHasUnits.Success, positionHasUnits.Success.ToString() + ": " + positionHasUnits.Details);
		 Assert.IsTrue(positionHasAvgPrice.Success, positionHasAvgPrice.Success.ToString() + ": " + positionHasAvgPrice.Details);
		 Assert.IsTrue(positionHasInstrument.Success, positionHasInstrument.Success.ToString() + ": " + positionHasInstrument.Details);
	  }

	  [TestMethod]
	  public void test_Position_get_position_details()
	  {
		 var postionHasDirection = m_Results.Items.FirstOrDefault(x => x.Key == "14.8").Value;
		 var positionHasUnits = m_Results.Items.FirstOrDefault(x => x.Key == "14.9").Value;
		 var positionHasAvgPrice = m_Results.Items.FirstOrDefault(x => x.Key == "14.10").Value;
		 var positionHasInstrument = m_Results.Items.FirstOrDefault(x => x.Key == "14.11").Value;

		 Assert.IsTrue(postionHasDirection.Success, postionHasDirection.Success.ToString() + ": " + postionHasDirection.Details);
		 Assert.IsTrue(positionHasUnits.Success, $"14.9,{positionHasUnits.Success}: {positionHasUnits.Details}");
		 Assert.IsTrue(positionHasAvgPrice.Success, positionHasAvgPrice.Success.ToString() + ": " + positionHasAvgPrice.Details);
		 Assert.IsTrue(positionHasInstrument.Success, positionHasInstrument.Success.ToString() + ": " + positionHasInstrument.Details);
	  }

	  [TestMethod]
	  public void test_Position_close_position()
	  {
		 var closeOrderCreated = m_Results.Items.FirstOrDefault(x => x.Key == "14.12").Value;
		 var closeOrderFilled = m_Results.Items.FirstOrDefault(x => x.Key == "14.13").Value;
		 var closeUnitsCorrect = m_Results.Items.FirstOrDefault(x => x.Key == "14.14").Value;

		 Assert.IsTrue(closeOrderCreated.Success, closeOrderCreated.Success.ToString() + ": " + closeOrderCreated.Details);
		 Assert.IsTrue(closeOrderFilled.Success, closeOrderFilled.Success.ToString() + ": " + closeOrderFilled.Details);
		 Assert.IsTrue(closeUnitsCorrect.Success, closeUnitsCorrect.Success.ToString() + ": " + closeUnitsCorrect.Details);
	  }
	  #endregion

	  #region Transaction
	  [TestMethod]
	  public void test_Transaction_get_transactions_by_date_range()
	  {
		 var transactionsReceived = m_Results.Items.FirstOrDefault(x => x.Key == "09.0").Value;
		 var clientConfigureReceived = m_Results.Items.FirstOrDefault(x => x.Key == "09.1").Value;
		 var notClientConfigureReceived = m_Results.Items.FirstOrDefault(x => x.Key == "09.2").Value;

		 Assert.IsTrue(transactionsReceived.Success, transactionsReceived.Success.ToString() + ": " + transactionsReceived.Details);
		 Assert.IsTrue(clientConfigureReceived.Success, clientConfigureReceived.Success.ToString() + ": " + clientConfigureReceived.Details);
		 Assert.IsTrue(transactionsReceived.Success, transactionsReceived.Success.ToString() + ": " + transactionsReceived.Details);
		 Assert.IsFalse(notClientConfigureReceived.Success, notClientConfigureReceived.Success.ToString() + ": " + notClientConfigureReceived.Details);
	  }

	  [TestMethod]
	  public void test_Transaction_get_transactions_since_id()
	  {
		 var transactionsReceived = m_Results.Items.FirstOrDefault(x => x.Key == "10.0").Value;
		 var firstIdIsNextId = m_Results.Items.FirstOrDefault(x => x.Key == "10.1").Value;
		 var allIdsGreaterThanLastId = m_Results.Items.FirstOrDefault(x => x.Key == "10.2").Value;
		 var clientConfigureReceived = m_Results.Items.FirstOrDefault(x => x.Key == "10.3").Value;

		 Assert.IsTrue(transactionsReceived.Success, transactionsReceived.Success.ToString() + ": " + transactionsReceived.Details);
		 Assert.IsTrue(firstIdIsNextId.Success, firstIdIsNextId.Success.ToString() + ": " + firstIdIsNextId.Details);
		 Assert.IsTrue(allIdsGreaterThanLastId.Success, allIdsGreaterThanLastId.Success.ToString() + ": " + allIdsGreaterThanLastId.Details);
		 Assert.IsTrue(clientConfigureReceived.Success, clientConfigureReceived.Success.ToString() + ": " + clientConfigureReceived.Details);
	  }

	  [TestMethod]
	  public void test_Transaction_get_transaction_detail()
	  {
		 var transactionReceived = m_Results.Items.FirstOrDefault(x => x.Key == "15.0").Value;
		 var transactionHasId = m_Results.Items.FirstOrDefault(x => x.Key == "15.1").Value;
		 var transactionHasType = m_Results.Items.FirstOrDefault(x => x.Key == "15.2").Value;
		 var transactionHasTime = m_Results.Items.FirstOrDefault(x => x.Key == "15.3").Value;

		 Assert.IsTrue(transactionReceived.Success, transactionReceived.Success.ToString() + ": " + transactionReceived.Details);
		 Assert.IsTrue(transactionHasId.Success, transactionHasId.Success.ToString() + ": " + transactionHasId.Details);
		 Assert.IsTrue(transactionHasType.Success, transactionHasType.Success.ToString() + ": " + transactionHasType.Details);
		 Assert.IsTrue(transactionHasTime.Success, transactionHasTime.Success.ToString() + ": " + transactionHasTime.Details);
	  }

	  [TestMethod]
	  public void test_Transaction_get_transactions_by_id_range()
	  {
		 var transactionsReceived = m_Results.Items.FirstOrDefault(x => x.Key == "16.0").Value;
		 var firstIdIsCorrect = m_Results.Items.FirstOrDefault(x => x.Key == "16.1").Value;
		 var allIdsGreaterThanFirst = m_Results.Items.FirstOrDefault(x => x.Key == "16.2").Value;
		 var clientConfiguredReturned = m_Results.Items.FirstOrDefault(x => x.Key == "16.3").Value;
		 var marketOrdersReturned = m_Results.Items.FirstOrDefault(x => x.Key == "16.4").Value;

		 Assert.IsTrue(transactionsReceived.Success, transactionsReceived.Success.ToString() + ": " + transactionsReceived.Details);
		 Assert.IsTrue(firstIdIsCorrect.Success, firstIdIsCorrect.Success.ToString() + ": " + firstIdIsCorrect.Details);
		 Assert.IsTrue(allIdsGreaterThanFirst.Success, allIdsGreaterThanFirst.Success.ToString() + ": " + allIdsGreaterThanFirst.Details);
		 Assert.IsTrue(clientConfiguredReturned.Success, clientConfiguredReturned.Success.ToString() + ": " + clientConfiguredReturned.Details);
		 Assert.IsTrue(marketOrdersReturned.Success, marketOrdersReturned.Success.ToString() + ": " + marketOrdersReturned.Details);
	  }
	  #endregion

	  #region Pricing
	  [TestMethod]
	  public void test_Pricing_get_prices_list()
	  {
		 var pricesReceived = m_Results.Items.FirstOrDefault(x => x.Key == "06.0").Value;
		 var priceCountMatches = m_Results.Items.FirstOrDefault(x => x.Key == "06.1").Value;

		 Assert.IsTrue(pricesReceived.Success, pricesReceived.Success.ToString() + ": " + pricesReceived.Details);
		 Assert.IsTrue(priceCountMatches.Success, priceCountMatches.Success.ToString() + ": " + priceCountMatches.Details);
	  }
	  #endregion

	  #region Stream
	  [TestMethod]
	  public void test_Stream_transaction_stream_functional()
	  {
		 // 7
		 var streamFunctional = m_Results.Items.FirstOrDefault(x => x.Key == "07.0").Value;
		 var dataReceived = m_Results.Items.FirstOrDefault(x => x.Key == "07.1").Value;
		 var dataHasId = m_Results.Items.FirstOrDefault(x => x.Key == "07.2").Value;
		 var dataHasAccountId = m_Results.Items.FirstOrDefault(x => x.Key == "07.3").Value;

		 Assert.IsTrue(streamFunctional.Success, streamFunctional.Success.ToString() + ": " + streamFunctional.Details);
		 Assert.IsTrue(dataReceived.Success, dataReceived.Success.ToString() + ": " + dataReceived.Details);
		 Assert.IsTrue(dataHasId.Success, dataHasId.Success.ToString() + ": " + dataHasId.Details);
		 Assert.IsTrue(dataHasAccountId.Success, dataHasAccountId.Success.ToString() + ": " + dataHasAccountId.Details);
	  }

	  [TestMethod]
	  public void test_Stream_pricing_stream_functional()
	  {
		 // 18
		 var streamFunctional = m_Results.Items.FirstOrDefault(x => x.Key == "18.0").Value;
		 var dataReceived = m_Results.Items.FirstOrDefault(x => x.Key == "18.1").Value;
		 var dataHasInstrument = m_Results.Items.FirstOrDefault(x => x.Key == "18.2").Value;

		 Assert.IsTrue(streamFunctional.Success, streamFunctional.Success.ToString() + ": " + streamFunctional.Details);
		 Assert.IsTrue(dataReceived.Success, dataReceived.Success.ToString() + ": " + dataReceived.Details);
		 Assert.IsTrue(dataHasInstrument.Success, dataHasInstrument.Success.ToString() + ": " + dataHasInstrument.Details);

		 // trap these
		 // will throw if the instrument is not tradeable
		 // the keys will not be present in _results
		 try
		 {
			var dataHasBids = m_Results.Items.FirstOrDefault(x => x.Key == "18.3").Value;
			var dataHasAsks = m_Results.Items.FirstOrDefault(x => x.Key == "18.4").Value;

			Assert.IsTrue(dataHasBids.Success, dataHasBids.Success.ToString() + ": " + dataHasBids.Details);
			Assert.IsTrue(dataHasAsks.Success, dataHasAsks.Success.ToString() + ": " + dataHasAsks.Details);
		 }
		 catch
		 {

		 }
	  }
	  #endregion

	  #region Errors
	  [TestMethod]
	  public void test_Error_response_has_correct_type()
	  {
		 var caughtAccountConfigurationError = m_Results.Items.FirstOrDefault(x => x.Key == "05.E0").Value;
		 var caughtOrderPostError = m_Results.Items.FirstOrDefault(x => x.Key == "11.E0").Value;
		 var caughtOrderClientExtensionsModifyError = m_Results.Items.FirstOrDefault(x => x.Key == "11.E1").Value;
		 var caughtOrderCancelReplaceError = m_Results.Items.FirstOrDefault(x => x.Key == "11.E2").Value;
		 var caughtOrderCancelError = m_Results.Items.FirstOrDefault(x => x.Key == "11.E3").Value;
		 var caughtTradeClientExtensionsModifyError = m_Results.Items.FirstOrDefault(x => x.Key == "13.E0").Value;
		 var caughtTradePatchExitOrdersError = m_Results.Items.FirstOrDefault(x => x.Key == "13.E1").Value;
		 var caughtTradeCloseError = m_Results.Items.FirstOrDefault(x => x.Key == "13.E2").Value;
		 var caughtPositionCloseError = m_Results.Items.FirstOrDefault(x => x.Key == "14.E0").Value;

		 Assert.IsTrue(caughtAccountConfigurationError.Success, caughtAccountConfigurationError.Success.ToString() + ": " + caughtAccountConfigurationError.Details);
		 Assert.IsTrue(caughtOrderPostError.Success, caughtOrderPostError.Success.ToString() + ": " + caughtOrderPostError.Details);
		 Assert.IsTrue(caughtOrderClientExtensionsModifyError.Success, caughtOrderClientExtensionsModifyError.Success.ToString() + ": " + caughtOrderClientExtensionsModifyError.Details);
		 Assert.IsTrue(caughtOrderCancelReplaceError.Success, caughtOrderCancelReplaceError.Success.ToString() + ": " + caughtOrderCancelReplaceError.Details);
		 Assert.IsTrue(caughtOrderCancelError.Success, caughtOrderCancelError.Success.ToString() + ": " + caughtOrderCancelError.Details);
		 Assert.IsTrue(caughtTradeClientExtensionsModifyError.Success, caughtTradeClientExtensionsModifyError.Success.ToString() + ": " + caughtTradeClientExtensionsModifyError.Details);
		 Assert.IsTrue(caughtTradePatchExitOrdersError.Success, caughtTradePatchExitOrdersError.Success.ToString() + ": " + caughtTradePatchExitOrdersError.Details);
		 Assert.IsTrue(caughtTradeCloseError.Success, caughtTradeCloseError.Success.ToString() + ": " + caughtTradeCloseError.Details);
		 Assert.IsTrue(caughtPositionCloseError.Success, caughtPositionCloseError.Success.ToString() + ": " + caughtPositionCloseError.Details);
	  }
	  #endregion
   }
}
