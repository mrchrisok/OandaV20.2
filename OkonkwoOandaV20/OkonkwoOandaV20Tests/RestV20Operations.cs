﻿using Microsoft.VisualStudio.TestTools.UnitTesting;
using OkonkwoOandaV20.Framework;
using OkonkwoOandaV20.Framework.Factories;
using OkonkwoOandaV20.TradeLibrary.Account;
using OkonkwoOandaV20.TradeLibrary.Instrument;
using OkonkwoOandaV20.TradeLibrary.Order;
using OkonkwoOandaV20.TradeLibrary.Position;
using OkonkwoOandaV20.TradeLibrary.Pricing;
using OkonkwoOandaV20.TradeLibrary.REST;
using OkonkwoOandaV20.TradeLibrary.REST.OrderRequests;
using OkonkwoOandaV20.TradeLibrary.Transaction;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Xml;
using static OkonkwoOandaV20.TradeLibrary.REST.Rest20;

namespace OkonkwoOandaV20Tests
{
   /// <summary>
   /// http://developer.oanda.com/rest-live-v20/introduction/
   /// </summary>
   public partial class Restv20Test
   {
	  #region Default Configuration

	  // warning: do not rely on these
	  // For best results, please create and use your own or your organization's Oanda Practice account

	  static EEnvironment m_TestEnvironment = EEnvironment.Practice;
	  static string m_TestToken = "d845d3f81613358410e3e5298aebdce1-111fcc1d52a6c625c42c05366a22a286";
	  static short m_TokenAccounts = 2;
	  static string m_TestAccount = "101-001-1913854-002";

	  #endregion

	  #region Declarations

	  static bool m_ApiOperationsComplete = false;
	  static string m_Currency = "USD";
	  static string m_TestInstrument = InstrumentName.Currency.USDCHF;
	  static List<Instrument> m_OandaInstruments;
	  static List<Price> m_OandaPrices;
	  static long m_FirstTransactionID;
	  static long m_LastTransactionID;
	  static string m_LastTransactionTime;
	  static decimal m_TestNumber;

	  protected List<Price> _prices;

	  #endregion

	  static string AccountID { get { return Credentials.GetDefaultCredentials().DefaultAccountId; } }

	  [ClassInitialize]
	  public static async void RunApiOperations(TestContext context)
	  {
		 try
		 {
			await SetApiCredentials();

			if (Credentials.GetDefaultCredentials() == null)
			   throw new Exception("Exception: RestV20Test - Credentials must be defined to run this test.");

			if (Credentials.GetDefaultCredentials().HasServer(EServer.Account))
			{
			   // first, get accounts
			   // this operation adds the test AccountId to Credentials (if it is null)
			   await Account_GetAccountsList(m_TokenAccounts);

			   // second, check market status
			   await Initialize_GetMarketStatus();

			   // third, proceed with all other operations
			   await Account_GetAccountDetails();
			   await Account_GetAccountSummary();
			   await Account_GetAccountsInstruments();
			   await Account_GetSingleAccountInstrument();
			   await Account_PatchAccountConfiguration();

			   await Transaction_GetTransaction();
			   await Transaction_GetTransactionsSinceId();

			   // important: do this before Orders and Trades tests
			   await Pricing_GetPricing();

			   await Instrument_GetInstrumentCandles();
			   await Instrument_GetInstrumentOrderBook();
			   await Instrument_GetInstrumentPositionBook();

			   // test the pricing stream
			   if (Credentials.GetDefaultCredentials().HasServer(EServer.PricingStream))
			   {
				  Stream_GetStreamingPrices();
			   }

			   // start transactions stream
			   Task transactionsStreamCheck = null;
			   if (Credentials.GetDefaultCredentials().HasServer(EServer.TransactionsStream))
			   {
				  transactionsStreamCheck = Stream_GetStreamingTransactions();
			   }

			   // create stream traffic
			   await Order_RunOrderOperations();
			   await Trade_RunTradeOperations();
			   await Position_RunPositionOperations();

			   // stop transactions stream 
			   if (transactionsStreamCheck != null)
			   {
				  await transactionsStreamCheck;
			   }

			   // review the traffic
			   await Transaction_GetTransactionsByDateRange();
			   await Transaction_GetTransactionsByIdRange();
			   await Account_GetAccountChanges();
			}
		 }
		 catch (MarketHaltedException ex)
		 {
			throw ex;
		 }
		 catch (Exception ex)
		 {
			throw new Exception("An unhandled error occured during execution of REST V20 operations.", ex);
		 }
		 finally
		 {
			m_ApiOperationsComplete = true;
		 }
	  }

	  private static async Task Initialize_GetMarketStatus()
	  {
		 bool marketIsHalted = await Utilities.IsMarketHalted();
		 m_Results.Verify("00.0", marketIsHalted, "Market is halted.");
		 if (marketIsHalted) throw new MarketHaltedException("Unable to continue tests. OANDA Fx market is halted!");
	  }

	  #region Account
	  /// <summary>
	  /// Retrieve the list of accounts associated with the account token
	  /// </summary>
	  /// <param name="listCount"></param>
	  private static async Task Account_GetAccountsList(short? listCount = null)
	  {
		 short count = 0;

		 List<AccountProperties> result = await Rest20.GetAccountsAsync();
		 m_Results.Verify("01.0", result.Count > 0, "Account list received.");

		 string message = "Correct number of accounts received.";
		 bool correctCount = true;
		 if (listCount.HasValue && listCount.Value > 0)
		 {
			count++;
			correctCount = result.Count == listCount;
			message = string.Format("Correct number of accounts ({0}) received.", result.Count);
		 }
		 m_Results.Verify("01." + count.ToString(), correctCount, message);

		 foreach (var account in result)
		 {
			count++;
			string description = string.Format("Account id {0} has correct format.", account.id);
			m_Results.Verify("01." + count.ToString(), account.id.Split('-').Length == 4, description);
		 }

		 // ensure the first account has sufficient funds
		 m_TestAccount = m_TestAccount ?? result.OrderBy(r => r.id).First().id;
		 Credentials.SetCredentials(m_TestEnvironment, m_TestToken, m_TestAccount);
	  }

	  // <summary>
	  // Retrieve the full details for the test accountId
	  // </summary>
	  private static async Task Account_GetAccountDetails()
	  {
		 // first, kill all open trades
		 var closeList = await Rest20.GetOpenTradesAsync(AccountID);

		 foreach (var trade in closeList)
			Rest20.PutTradeCloseAsync(AccountID, trade.id).Wait();

		 Account result = await Rest20.GetAccountAsync(AccountID);

		 m_Results.Verify("08.0", result != null, string.Format("Account {0} info received.", AccountID));
		 m_Results.Verify("08.1", result.id == AccountID, string.Format("Account id ({0}) is correct.", result.id));
		 m_Results.Verify("08.2", result.currency == m_Currency, string.Format("Account currency ({0}) is correct.", result.currency));
		 m_Results.Verify("08.3", result.openTradeCount == 0, string.Format("Account openTradeCount ({0}) is correct.", 0));
		 m_Results.Verify("08.4", result.trades.Count == result.openTradeCount, string.Format("Account trades count ({0}) is correct.", 0));
	  }

	  /// <summary>
	  /// Retrieve the list of instruments associated with the given accountId
	  /// </summary>
	  private static async Task Account_GetAccountsInstruments()
	  {
		 // Get an instrument list (basic)
		 List<Instrument> result = await Rest20.GetAccountInstrumentsAsync(AccountID);
		 m_Results.Verify("02.0", result.Count > 0, "Instrument list received");

		 // Store the instruments for other tests
		 m_OandaInstruments = result;
	  }

	  /// <summary>
	  /// Retrieve the list of instruments associated with the given accountId
	  /// </summary>
	  private static async Task Account_GetSingleAccountInstrument()
	  {
		 // Get an instrument list (basic)
		 var instruments = new List<string>() { m_TestInstrument };
		 string type = InstrumentType.Currency;
		 var parameters = new AccountInstrumentsParameters() { instruments = instruments };

		 List<Instrument> result = await Rest20.GetAccountInstrumentsAsync(AccountID, parameters);

		 m_Results.Verify("03.0", result.Count == 1, string.Format("{0} info received.", instruments[0]));
		 m_Results.Verify("03.1", result[0].type == type, string.Format("{0} type ({1}) is correct.", instruments[0], result[0].type));
		 m_Results.Verify("03.2", result[0].name == instruments[0], string.Format("{0} name ({1}) is correct.", instruments[0], result[0].name));
	  }

	  /// <summary>
	  /// Retrieve summary information for the given accountId
	  /// </summary>
	  private static async Task Account_GetAccountSummary()
	  {
		 // 04
		 AccountSummary result = await Rest20.GetAccountSummaryAsync(AccountID);
		 m_Results.Verify("04.0", result != null, string.Format("Account {0} info received.", AccountID));
		 m_Results.Verify("04.1", result.id == AccountID, string.Format("AccounSummary.id ({0}) is correct.", result.id));
		 m_Results.Verify("04.2", result.currency == m_Currency, string.Format("AccountSummary.currency ({0}) is correct.", result.currency));
	  }

	  /// <summary>
	  /// Retrieve summary information for the given accountId
	  /// </summary>
	  private static async Task Account_PatchAccountConfiguration()
	  {
		 // 05
		 AccountSummary summary = await Rest20.GetAccountSummaryAsync(AccountID);

		 m_FirstTransactionID = summary.lastTransactionID;
		 m_LastTransactionID = summary.lastTransactionID;

		 string alias = summary.alias;
		 decimal? marginRate = summary.marginRate;

		 string testAlias = $"testAlias_{DateTime.UtcNow.ToString("yyyyMMddTHHmmssfffffffK")}";
		 decimal? testMarginRate = (decimal)(marginRate == null ? 0.5 : (((double)marginRate.Value == 0.5) ? 0.4 : 0.5));
		 var parameters = new AccountConfigurationParameters()
		 {
			alias = testAlias,
			marginRate = testMarginRate
		 };

		 m_LastTransactionTime = ConvertDateTimeToAcceptDateFormat(DateTime.UtcNow, AcceptDatetimeFormat.RFC3339);

		 AccountConfigurationResponse response = null;

		 // error test
		 try { response = await Rest20.PatchAccountConfigurationAsync("fakeId", parameters); }
		 catch (Exception ex)
		 {
			var errorResponse = ErrorResponseFactory.Create(ex.Message) as AccountConfigurationErrorResponse;
			m_Results.Verify("05.E0", errorResponse != null, "Error response has correct type: AccountConfigurationErrorResponse");
		 }

		 // response test
		 response = await Rest20.PatchAccountConfigurationAsync(AccountID, parameters);
		 ClientConfigureTransaction newConfig = response.clientConfigureTransaction;

		 m_Results.Verify("05.0", newConfig != null, "Account configuration retrieved successfully.");
		 m_Results.Verify("05.1", newConfig.alias != alias, string.Format("Account alias {0} updated successfully.", newConfig.alias));
		 m_Results.Verify("05.2", newConfig.marginRate != marginRate, string.Format("Account marginRate {0} updated succesfully.", newConfig.marginRate));

		 parameters.alias = alias;
		 parameters.marginRate = marginRate ?? (decimal)1.0;
		 AccountConfigurationResponse response2 = await Rest20.PatchAccountConfigurationAsync(AccountID, parameters);
		 ClientConfigureTransaction newConfig2 = response2.clientConfigureTransaction;

		 m_Results.Verify("05.3", newConfig2.alias == alias, string.Format("Account alias {0} reverted successfully.", newConfig2.alias));
		 m_Results.Verify("05.4", newConfig2.marginRate == marginRate, string.Format("Account marginRate {0} reverted succesfully.", newConfig2.marginRate));
	  }

	  private static async Task Account_GetAccountChanges()
	  {
		 //17
		 var parameters = new AccountChangesParameters() { sinceTransactionID = m_FirstTransactionID };
		 AccountChangesResponse result = await Rest20.GetAccountChangesAsync(AccountID, parameters);

		 var changes = result.changes;

		 m_Results.Verify("17.0", result != null, "Account changes received.");
		 m_Results.Verify("17.1", changes.ordersFilled.Count > 0, "Account has filled orders.");
		 m_Results.Verify("17.2", changes.ordersCancelled.Count > 0, "Account has cancelled orders.");
		 m_Results.Verify("17.3", changes.tradesClosed.Count > 0, "Account has closed trades.");
		 m_Results.Verify("17.4", changes.positions.Count > 0, "Account has position.");
		 m_Results.Verify("17.5", result.state.marginAvailable > 0, "Account has transactions.");
	  }
	  #endregion

	  #region Instrument
	  // <summary>
	  // Retrieve the full details for the test accountId
	  // </summary>
	  private static async Task Instrument_GetInstrumentCandles()
	  {
		 short count = 500;

		 // get all price types .. MBA
		 string price = string.Concat(CandleStickPriceType.Midpoint, CandleStickPriceType.Bid, CandleStickPriceType.Ask);

		 string instrument = m_TestInstrument;
		 string granularity = CandleStickGranularity.Hours01;

		 var parameters = new InstrumentCandlesParameters()
		 {
			price = price,
			granularity = granularity,
			count = count
		 };

		 List<CandlestickPlus> result = await Rest20.GetInstrumentCandlesAsync(instrument, parameters);
		 CandlestickPlus candle = result.FirstOrDefault();

		 m_Results.Verify("12.0", result != null, "Candles list received.");
		 m_Results.Verify("12.1", result.Count() == count, "Candles count is correct.");
		 m_Results.Verify("12.2", candle.instrument == instrument, "Candles instrument is correct.");
		 m_Results.Verify("12.3", candle.granularity == granularity, "Candles granularity is correct.");
		 m_Results.Verify("12.4", candle.mid != null && candle.mid.o > 0, "Candle has mid prices");
		 m_Results.Verify("12.5", candle.bid != null && candle.bid.o > 0, "Candle has bid prices");
		 m_Results.Verify("12.6", candle.ask != null && candle.ask.o > 0, "Candle has ask prices");
	  }

	  private static async Task Instrument_GetInstrumentOrderBook()
	  {
		 // currently, Oanda only provides snapshots at 0th, 20th and 40th minute of each hour
		 // this behavior appears to be undocumented, but was deduced by examining the 'Link' header of the response

		 OrderBook result = null;

		 // error test - future snapshot time
		 var futureDateTime = DateTime.UtcNow.AddHours(1);
		 string unavailableSnapshotTime = $"{ConvertDateTimeToAcceptDateFormat(futureDateTime).Split(':')[0]}:00:00Z";
		 var parameters = new InstrumentOrderBookParameters()
		 {
			time = unavailableSnapshotTime,
			getLastTimeOnFailure = false
		 };
		 try { result = await Rest20.GetInstrumentOrderBookAsync(m_TestInstrument, parameters); }
		 catch (Exception ex)
		 {
			var errorResponse = ErrorResponseFactory.Create(ex.Message);
			m_Results.Verify("19.E0", errorResponse != null, "Future snapshot time returns an error response.");
		 }

		 // get the 0th hour (or previous) snapshot
		 string availableSnapshotTime = $"{ConvertDateTimeToAcceptDateFormat(DateTime.UtcNow).Split(':')[0]}:00:00Z";
		 parameters.time = availableSnapshotTime;
		 parameters.getLastTimeOnFailure = true;
		 result = await Rest20.GetInstrumentOrderBookAsync(m_TestInstrument, parameters);

		 m_Results.Verify("19.0", result != null, "Order book snapshot was received.");
		 m_Results.Verify("19.1", result.time == availableSnapshotTime, "Order book snapshot time is correct.");
		 m_Results.Verify("19.2", result.buckets.Count > 0, "Order book snapshot has buckets.");
	  }

	  private static async Task Instrument_GetInstrumentPositionBook()
	  {
		 // currently, Oanda only provides snapshots at 0th, 20th and 40th minute of each hour
		 // this behavior appears to be undocumented, but was deduced by examining the 'Link' header of the response

		 PositionBook result = null;

		 // error test - future snapshot time
		 var futureDateTime = DateTime.UtcNow.AddHours(1);
		 string unavailableSnapshotTime = $"{ConvertDateTimeToAcceptDateFormat(futureDateTime).Split(':')[0]}:00:00Z";
		 var parameters = new InstrumentPositionBookParameters()
		 {
			time = unavailableSnapshotTime,
			getLastTimeOnFailure = false
		 };
		 try { result = await Rest20.GetInstrumentPositionBookAsync(m_TestInstrument, parameters); }
		 catch (Exception ex)
		 {
			var errorResponse = ErrorResponseFactory.Create(ex.Message);
			m_Results.Verify("20.E0", errorResponse != null, "Future snapshot time returns an error response.");
		 }

		 // get the 0th hour (or previous) snapshot
		 string availableSnapshotTime = $"{ConvertDateTimeToAcceptDateFormat(DateTime.UtcNow).Split(':')[0]}:00:00Z";
		 parameters.time = availableSnapshotTime;
		 parameters.getLastTimeOnFailure = true;
		 result = await Rest20.GetInstrumentPositionBookAsync(m_TestInstrument, parameters);

		 m_Results.Verify("20.0", result != null, "Position book snapshot was received.");
		 m_Results.Verify("20.1", result.time == availableSnapshotTime, "Position book snapshot time is correct.");
		 m_Results.Verify("20.2", result.buckets.Count > 0, "Position book snapshot has buckets.");
	  }
	  #endregion

	  #region Order
	  /// <summary>
	  /// Runs operations available at OANDA's Order endpoint
	  /// </summary>
	  /// <returns></returns>
	  private static async Task Order_RunOrderOperations()
	  {
		 if (await Utilities.IsMarketHalted())
			throw new MarketHaltedException("OANDA Fx market is halted!");

		 string expiry = ConvertDateTimeToAcceptDateFormat(DateTime.Now.AddMonths(1));
		 decimal price = GetOandaPrice(m_TestInstrument) * (decimal)0.9;
		 #region create new pending order
		 var request1 = new MarketIfTouchedOrderRequest(GetOandaInstrument())
		 {
			units = 1, // buy
			timeInForce = TimeInForce.GoodUntilDate,
			gtdTime = expiry,
			priceBound = price * (decimal)1.01,
			clientExtensions = new ClientExtensions()
			{
			   id = "test_order_1",
			   comment = "test order 1 comment",
			   tag = "test_order_1"
			},
			tradeClientExtensions = new ClientExtensions()
			{
			   id = "test_trade_1",
			   comment = "test trade 1 comment",
			   tag = "test_trade_1"
			}
		 };

		 // first, kill any open orders
		 var openOrders = await Rest20.GetPendingOrdersAsync(AccountID);
		 openOrders.ForEach(async x => await Rest20.PutOrderCancelAsync(AccountID, x.id));

		 // error test - create order
		 PostOrderResponse response1 = null;
		 request1.price = -1;
		 try { response1 = await Rest20.PostOrderAsync(AccountID, request1); }
		 catch (Exception ex)
		 {
			var errorResponse = ErrorResponseFactory.Create(ex.Message) as PostOrderErrorResponse;
			m_Results.Verify("11.E0", errorResponse != null, "Error response has correct type: PostOrderErrorResponse");
		 }

		 // create order1
		 request1.price = price;
		 response1 = await Rest20.PostOrderAsync(AccountID, request1);
		 var orderTransaction1 = response1.orderCreateTransaction;

		 m_Results.Verify("11.0", orderTransaction1 != null && orderTransaction1.id > 0, "Order successfully opened");
		 m_Results.Verify("11.1", orderTransaction1.type == TransactionType.MarketIfTouchedOrder, "Order type is correct.");

		 // create order2
		 var request2 = new MarketIfTouchedOrderRequest(GetOandaInstrument())
		 {
			units = -1, // sell
			timeInForce = TimeInForce.GoodUntilDate,
			gtdTime = expiry,
			priceBound = price * (decimal)0.99,
			clientExtensions = new ClientExtensions()
			{
			   id = "test_order_2",
			   comment = "test order 2 comment",
			   tag = "test_order_2"
			},
			tradeClientExtensions = new ClientExtensions()
			{
			   id = "test_trade_2",
			   comment = "test trade 2 comment",
			   tag = "test_trade_2"
			}
		 };
		 request2.price = price;
		 PostOrderResponse response2 = await Rest20.PostOrderAsync(AccountID, request2);
		 var orderTransaction2 = response2.orderCreateTransaction;

		 // Get all orders
		 var allOrders = await Rest20.GetOrdersAsync(AccountID);
		 m_Results.Verify("11.2", allOrders != null && allOrders.Count > 0, "All orders list successfully retrieved");
		 m_Results.Verify("11.3", allOrders.Where(x => x.id == orderTransaction1.id || x.id == orderTransaction2.id).Count() == 2, "Test orders in all orders return.");

		 // Get pending orders
		 var pendingOrders = await Rest20.GetPendingOrdersAsync(AccountID);
		 m_Results.Verify("11.4", pendingOrders != null && pendingOrders.Count > 0, "Pending orders list successfully retrieved");
		 m_Results.Verify("11.5", pendingOrders.Where(x => x.state != OrderState.Pending).Count() == 0, "Only pending orders returned.");
		 m_Results.Verify("11.6", pendingOrders.Where(x => x.id == orderTransaction1.id || x.id == orderTransaction2.id).Count() == 2, "Test orders in pending orders return.");

		 // Get orders by ID list
		 var pendingOrderParameters = new OrdersParameters() { ids = new List<string>() };
		 pendingOrders.ForEach(pendingOrder => pendingOrderParameters.ids.Add(pendingOrder.id.ToString()));
		 var allPendingOrders = await Rest20.GetOrdersAsync(AccountID, pendingOrderParameters);
		 m_Results.Verify("11.29", allPendingOrders.Where(x => x.id == orderTransaction1.id || x.id == orderTransaction2.id).Count() == 2, "Test orders in orders by ID list return.");
		 // kill orderTransaction2 because it is no longer needed
		 await Rest20.PutOrderCancelAsync(AccountID, orderTransaction2.id);

		 // Get order details
		 var order1 = await Rest20.GetOrderAsync(AccountID, orderTransaction1.id) as MarketIfTouchedOrder;
		 m_Results.Verify("11.7", order1 != null && order1.id == orderTransaction1.id, "Order details successfully retrieved.");
		 m_Results.Verify("11.8", (order1.clientExtensions == null) == (request1.clientExtensions == null), "order client extensions successfully retrieved.");
		 m_Results.Verify("11.9", (order1.tradeClientExtensions == null) == (request1.tradeClientExtensions == null), "order trade client extensions successfully retrieved.");

		 // create extensions
		 var newOrderExtensions = request1.clientExtensions;
		 newOrderExtensions.comment = "updated order 1 comment";
		 var newTradeExtensions = request1.tradeClientExtensions;
		 newTradeExtensions.comment = "updated trade 1 comment";

		 // error test - update extensions
		 OrderClientExtensionsResponse extensionsModifyResponse = null;
		 var extensionsParameters = new OrderClientExtensionsParameters()
		 {
			clientExtensions = newOrderExtensions,
			tradeClientExtensions = newTradeExtensions
		 };
		 try { extensionsModifyResponse = await Rest20.PutOrderClientExtensionsAsync(AccountID, -1, extensionsParameters); }
		 catch (Exception ex)
		 {
			var errorResponse = ErrorResponseFactory.Create(ex.Message) as OrderClientExtensionsErrorResponse;
			m_Results.Verify("11.E1", errorResponse != null, "Error response has correct type: OrderClientExtensionsErrorResponse");
		 }

		 // Udpate extensions
		 extensionsModifyResponse = await Rest20.PutOrderClientExtensionsAsync(AccountID, order1.id, extensionsParameters);

		 m_Results.Verify("11.10", extensionsModifyResponse != null, "Order extensions update received successfully.");
		 m_Results.Verify("11.11", extensionsModifyResponse.orderClientExtensionsModifyTransaction.orderID == order1.id, "Correct order extensions updated.");
		 m_Results.Verify("11.12", extensionsModifyResponse.orderClientExtensionsModifyTransaction.clientExtensionsModify.comment == "updated order 1 comment", "Order extensions comment updated successfully.");
		 m_Results.Verify("11.13", extensionsModifyResponse.orderClientExtensionsModifyTransaction.tradeClientExtensionsModify.comment == "updated trade 1 comment", "Order trade extensions comment updated successfully.");

		 // error test - cancel+replace an existing order
		 OrderReplaceResponse cancelReplaceResponse = null;
		 request1.price = -1;
		 try { cancelReplaceResponse = await Rest20.PutOrderReplaceAsync(AccountID, order1.id, request1); }
		 catch (Exception ex)
		 {
			var errorResponse = ErrorResponseFactory.Create(ex.Message) as OrderReplaceErrorResponse;
			m_Results.Verify("11.E2", errorResponse != null, "Error response has correct type: OrderReplaceErrorResponse");
		 }

		 // Cancel & Replace an existing order
		 request1.price = price;
		 request1.units += 10;
		 cancelReplaceResponse = await Rest20.PutOrderReplaceAsync(AccountID, order1.id, request1);
		 var cancelTransaction = cancelReplaceResponse.orderCancelTransaction;
		 var newOrderTransaction = cancelReplaceResponse.orderCreateTransaction;

		 m_Results.Verify("11.14", cancelTransaction != null && cancelTransaction.orderID == order1.id, "Order ancel+replace cancelled successfully.");
		 m_Results.Verify("11.15", newOrderTransaction != null && newOrderTransaction.id > 0 && newOrderTransaction.id != order1.id, "Order cancel+replace replaced successfully.");

		 // Get new order details
		 var newOrder = await Rest20.GetOrderAsync(AccountID, newOrderTransaction.id) as MarketIfTouchedOrder;
		 m_Results.Verify("11.16", newOrder != null && newOrder.units == request1.units, "New order details are correct.");

		 // error test - cancel an order
		 OrderCancelResponse cancelOrderResponse = null;
		 try { cancelOrderResponse = await Rest20.PutOrderCancelAsync(AccountID, -1); }
		 catch (Exception ex)
		 {
			var errorResponse = ErrorResponseFactory.Create(ex.Message) as OrderCancelErrorResponse;
			m_Results.Verify("11.E3", errorResponse != null, "Error response has correct type: OrderCancelErrorResponse");
		 }

		 // Cancel an order
		 cancelOrderResponse = await Rest20.PutOrderCancelAsync(AccountID, newOrder.id);
		 m_Results.Verify("11.17", cancelOrderResponse != null, "Cancelled order retrieved successfully.");
		 var cancelTransaction2 = cancelOrderResponse.orderCancelTransaction;
		 m_Results.Verify("11.18", cancelTransaction2.orderID == newOrder.id, "Order cancelled successfully.");
		 #endregion

		 #region Create new pending order with exit orders

		 // need this for rounding, etc.
		 var owxInstrument = GetOandaInstrument(m_TestInstrument);

		 decimal owxOrderPrice = Math.Round(GetOandaPrice(m_TestInstrument), owxInstrument.displayPrecision);
		 decimal owxStopLossPrice = Math.Round(owxOrderPrice - ((decimal)0.10 * owxOrderPrice), owxInstrument.displayPrecision);
		 decimal owxTakeProfitPrice = Math.Round(owxOrderPrice + ((decimal)0.10 * owxOrderPrice), owxInstrument.displayPrecision);

		 var owxRequest = new LimitOrderRequest(owxInstrument)
		 {
			units = 1, // buy
			timeInForce = TimeInForce.GoodUntilDate,
			gtdTime = expiry,
			price = owxOrderPrice,
			priceBound = owxOrderPrice + ((decimal)0.01 * owxOrderPrice),
			stopLossOnFill = new StopLossDetails(owxInstrument)
			{
			   timeInForce = TimeInForce.GoodUntilCancelled,
			   price = owxStopLossPrice
			},
			takeProfitOnFill = new TakeProfitDetails(owxInstrument)
			{
			   timeInForce = TimeInForce.GoodUntilCancelled,
			   price = owxTakeProfitPrice
			}
		 };

		 // create order with exit orders
		 var owxResponse = await Rest20.PostOrderAsync(AccountID, owxRequest);
		 var owxOrderTransaction = owxResponse.orderCreateTransaction;
		 m_Results.Verify("11.19", owxOrderTransaction != null && owxOrderTransaction.id > 0, "Order with exit orders successfully opened");
		 m_Results.Verify("11.20", owxOrderTransaction.type == TransactionType.LimitOrder, "Order with exit orders type is correct.");

		 // get order with exit orders details
		 var pendingOrders2 = await Rest20.GetPendingOrdersAsync(AccountID);
		 var order2 = await Rest20.GetOrderAsync(AccountID, pendingOrders2[0].id) as LimitOrder;
		 m_Results.Verify("11.21", order2 != null && order2.stopLossOnFill.price == owxStopLossPrice, "Order with exit orders stoploss details are correct.");
		 m_Results.Verify("11.22", order2 != null && order2.takeProfitOnFill.price == owxTakeProfitPrice, "Order with exit orders takeprofit details are correct.");

		 // cancel & replace order with exit orders - update stoploss and takeprofit
		 owxStopLossPrice = Math.Round(owxOrderPrice - ((decimal)0.15 * owxOrderPrice), owxInstrument.displayPrecision);
		 owxRequest.stopLossOnFill = new StopLossDetails(owxInstrument) { price = owxStopLossPrice };

		 owxTakeProfitPrice = Math.Round(owxOrderPrice + ((decimal)0.15 * owxOrderPrice), owxInstrument.displayPrecision);
		 owxRequest.takeProfitOnFill = new TakeProfitDetails(owxInstrument) { price = owxTakeProfitPrice };

		 var owxReplaceResponse = await Rest20.PutOrderReplaceAsync(AccountID, order2.id, owxRequest);
		 var owxCancelTransaction = owxReplaceResponse.orderCancelTransaction;
		 var owxNewOrderTransaction = owxReplaceResponse.orderCreateTransaction;

		 m_Results.Verify("11.23", owxCancelTransaction != null && owxCancelTransaction.orderID == order2.id, "Order with exit orders cancel+replace cancelled successfully.");
		 m_Results.Verify("11.24", owxCancelTransaction != null && owxCancelTransaction.reason == OrderCancelReason.ClientRequestReplaced, "Order with exit orders cancel+replace has correct reason.");
		 m_Results.Verify("11.25", owxNewOrderTransaction != null && owxNewOrderTransaction.id > 0 && owxNewOrderTransaction.id == owxCancelTransaction.replacedByOrderID, "Order with exit orders cancel+replace replaced successfully.");

		 // Get order with exit orders replacement order details
		 var owxOrderTransaction2 = owxNewOrderTransaction as LimitOrderTransaction;
		 m_Results.Verify("11.26", owxOrderTransaction2 != null && owxOrderTransaction2.reason == LimitOrderReason.Replacement, "Order with exit orders replacement order reason is correct.");
		 m_Results.Verify("11.27", owxOrderTransaction2 != null && owxOrderTransaction2.stopLossOnFill.price == owxStopLossPrice, "Order with exit orders replacement order stoploss is correct.");
		 m_Results.Verify("11.28", owxOrderTransaction2 != null && owxOrderTransaction2.takeProfitOnFill.price == owxTakeProfitPrice, "Order with exit orders replacement order takeprofit is correct.");
		 #endregion
	  }

	  /// <summary>
	  /// Places a market order.
	  /// </summary>
	  /// <param name="key">The key root used to store the order success and order fill results.</param>
	  /// <returns></returns>
	  private static async Task PlaceMarketOrder(string key, decimal units = 0, bool closeAllTrades = true)
	  {
		 // I'm fine with a throw here
		 // To each his/her own on doing something different.
		 if (await Utilities.IsMarketHalted())
			throw new MarketHaltedException("OANDA Fx market is halted!");

		 if (closeAllTrades)
		 {
			var closeList = await Rest20.GetOpenTradesAsync(AccountID);
			closeList.ForEach(async x => await Rest20.PutTradeCloseAsync(AccountID, x.id));
		 }

		 // this should have a value by now
		 var instrument = GetOandaInstrument(m_TestInstrument);
		 if (units == 0) units = instrument.minimumTradeSize;

		 var request = new MarketOrderRequest(GetOandaInstrument())
		 {
			units = units, // buy
			clientExtensions = new ClientExtensions()
			{
			   id = "test_market_order_1",
			   comment = "test market order comment",
			   tag = "test_market_order"
			},
			tradeClientExtensions = new ClientExtensions()
			{
			   id = "test_market_trade_1",
			   comment = "test market trade comment",
			   tag = "test_market_trade"
			}
		 };

		 var response = await Rest20.PostOrderAsync(AccountID, request);
		 var createTransaction = response.orderCreateTransaction;
		 var fillTransaction = response.orderFillTransaction;

		 m_Results.Verify(key + ".0", createTransaction != null && createTransaction.id > 0, "Market order successfully placed.");
		 m_Results.Verify(key + ".1", fillTransaction != null && fillTransaction.id > 0, "Market order successfully filled.");
	  }
	  #endregion

	  #region Trade
	  /// <summary>
	  /// Runs operations available at OANDA's Trade endpoint
	  /// </summary>
	  /// <returns></returns>
	  private static async Task Trade_RunTradeOperations()
	  {
		 await PlaceMarketOrder("13");

		 // get list of trades
		 var trades = await Rest20.GetTradesAsync(AccountID);
		 m_Results.Verify("13.2", trades.Count > 0 && trades[0].id > 0, "Trades list retrieved.");

		 // get list of open trades
		 var openTrades = await Rest20.GetTradesAsync(AccountID);
		 m_Results.Verify("13.3", openTrades.Count > 0 && openTrades[0].id > 0, "Open trades list retrieved.");

		 if (openTrades.Count == 0)
			throw new InvalidOperationException("Trade test operations cannot continue without an open trade.");

		 // get details for a trade
		 var trade = await Rest20.GetTradeAsync(AccountID, openTrades[0].id);
		 m_Results.Verify("13.4", trade.id > 0 && trade.price > 0 && trade.initialUnits != 0, "Trade details retrieved");

		 // create extensions
		 var updatedExtensions = trade.clientExtensions;
		 updatedExtensions.comment = "updated test market trade comment";

		 // error test - update extensions
		 TradeClientExtensionsResponse extensionsModifyResponse = null;
		 var clientExtensionsParameters = new TradeClientExtensionsParameters() { clientExtensions = updatedExtensions };
		 try { extensionsModifyResponse = await Rest20.PutTradeClientExtensionsAsync(AccountID, -1, clientExtensionsParameters); }
		 catch (Exception ex)
		 {
			var errorResponse = ErrorResponseFactory.Create(ex.Message) as TradeClientExtensionsErrorResponse;
			m_Results.Verify("13.E0", errorResponse != null, "Error response has correct type: TradeClientExtensionsModifyErrorResponse");
		 }

		 // update extensions
		 extensionsModifyResponse = await Rest20.PutTradeClientExtensionsAsync(AccountID, trade.id, clientExtensionsParameters);

		 m_Results.Verify("13.5", extensionsModifyResponse != null, "Order extensions update received successfully.");
		 m_Results.Verify("13.6", extensionsModifyResponse.tradeClientExtensionsModifyTransaction.tradeID == trade.id, "Correct trade extensions updated.");
		 m_Results.Verify("13.7", extensionsModifyResponse.tradeClientExtensionsModifyTransaction.tradeClientExtensionsModify.comment == "updated test market trade comment", "Trade extensions comment updated successfully.");

		 // need this for rounding, etc.
		 var instrument = GetOandaInstrument(trade.instrument);

		 // create takeProfit 
		 decimal takeProfitPrice = Math.Round((decimal)1.10 * trade.price, instrument.displayPrecision);
		 var takeProfit = new TakeProfitDetails(instrument)
		 {
			price = takeProfitPrice,
			timeInForce = TimeInForce.GoodUntilCancelled,
			clientExtensions = new ClientExtensions()
			{
			   id = "take_profit_1",
			   comment = "take profit comment",
			   tag = "take_profit"
			}
		 };
		 //var patch1 = new PatchExitOrdersRequest() { takeProfit = takeProfit };
		 var patch1 = new TradeOrdersParameters();
		 patch1.SetTakeProfit(TradeOrdersAction.Create, takeProfit);

		 // error test - patch open trade
		 TradeOrdersResponse response = null;
		 patch1.takeProfit.price = -1;
		 try { response = await Rest20.PutTradeOrdersAsync(AccountID, trade.id, patch1); }
		 catch (Exception ex)
		 {
			var errorResponse = ErrorResponseFactory.Create(ex.Message) as TradeOrdersErrorResponse;
			m_Results.Verify("13.E1", errorResponse != null, "Error response has correct type: TradePatchExitOrdersErrorResponse");
		 }

		 // add a takeProft to an open trade
		 patch1.takeProfit.price = takeProfitPrice;
		 var takeProfitPatch = (await Rest20.PutTradeOrdersAsync(AccountID, trade.id, patch1)).takeProfitOrderTransaction;
		 m_Results.Verify("13.8", takeProfitPatch != null && takeProfitPatch.id > 0, "Take profit patch received.");
		 m_Results.Verify("13.9", takeProfitPatch.price == takeProfitPrice, "Trade patched with take profit.");

		 // Add a stopLoss to an open trade
		 decimal stopLossPrice = Math.Round(trade.price - ((decimal)0.10 * trade.price), instrument.displayPrecision);
		 var stopLoss = new StopLossDetails(instrument)
		 {
			price = stopLossPrice,
			timeInForce = TimeInForce.GoodUntilCancelled,
			clientExtensions = new ClientExtensions()
			{
			   id = "stop_loss_1",
			   comment = "stop loss comment",
			   tag = "stop_loss"
			}
		 };
		 var patch2 = new TradeOrdersParameters();
		 patch2.SetStopLoss(TradeOrdersAction.Create, stopLoss);
		 var stopLossPatch = (await Rest20.PutTradeOrdersAsync(AccountID, trade.id, patch2)).stopLossOrderTransaction;
		 m_Results.Verify("13.10", takeProfitPatch != null && takeProfitPatch.id > 0, "Stop loss patch received.");
		 m_Results.Verify("13.11", stopLossPatch.price == stopLossPrice, "Trade patched with stop loss.");

		 // Add a trailingStopLoss to an open trade
		 decimal distance = 2 * (trade.price - stopLoss.price.Value);
		 var trailingStopLoss = new TrailingStopLossDetails(instrument)
		 {
			distance = distance,
			timeInForce = TimeInForce.GoodUntilCancelled,
			clientExtensions = new ClientExtensions()
			{
			   id = "trailing_stop_loss_1",
			   comment = "trailing stop loss comment",
			   tag = "trailing_stop_loss"
			}
		 };

		 var patch3 = new TradeOrdersParameters();

		 // stopLoss and trailingStopLoss cannot exist simultaneously
		 // thus the stopLoss must be cancelled whilst creating the trailingStopLoss
		 patch3.SetStopLoss(TradeOrdersAction.Cancel, null);
		 patch3.SetTrailingStopLoss(TradeOrdersAction.Create, trailingStopLoss);

		 var trailingStopLossPatch = (await Rest20.PutTradeOrdersAsync(AccountID, trade.id, patch3));
		 var trailingStopLossOrderTransaction = trailingStopLossPatch.trailingStopLossOrderTransaction;
		 m_Results.Verify("13.12", trailingStopLossPatch.stopLossOrderCancelTransaction != null, "Stop loss cancelled.");
		 m_Results.Verify("13.13", trailingStopLossOrderTransaction != null && trailingStopLossOrderTransaction.id > 0, "Trailing stop loss patch created.");
		 m_Results.Verify("13.14", trailingStopLossOrderTransaction.distance == distance, "Trade patched with correct trailing stop loss.");

		 // remove dependent orders
		 var tradeOrdersParameters = new TradeOrdersParameters();
		 tradeOrdersParameters.SetTakeProfit(TradeOrdersAction.Cancel, null);
		 tradeOrdersParameters.SetTrailingStopLoss(TradeOrdersAction.Cancel, null);

		 var result = await Rest20.PutTradeOrdersAsync(AccountID, trade.id, tradeOrdersParameters);
		 m_Results.Verify("13.15", result.takeProfitOrderCancelTransaction != null, "Take profit cancelled.");
		 //m_Results.Verify("13.15", result.stopLossOrderCancelTransaction != null, "Stop loss cancelled.");
		 m_Results.Verify("13.16", result.trailingStopLossOrderCancelTransaction != null, "Trailing stop loss cancelled.");

		 if (await Utilities.IsMarketHalted())
			throw new MarketHaltedException("OANDA Fx market is halted!");

		 // error test - close open trade
		 TradeCloseResponse tradeCloseResponse = null;
		 var tradeCloseParameters = new TradeCloseParameters();
		 try { tradeCloseResponse = await Rest20.PutTradeCloseAsync(AccountID, -1, tradeCloseParameters); }
		 catch (Exception ex)
		 {
			var errorResponse = ErrorResponseFactory.Create(ex.Message) as TradeCloseErrorResponse;
			m_Results.Verify("13.E2", errorResponse != null, "Error response has correct type: TradeCloseResponse");
		 }

		 // close an open trade
		 var closedDetails = (await Rest20.PutTradeCloseAsync(AccountID, trade.id, tradeCloseParameters)).orderFillTransaction;
		 m_Results.Verify("13.17", closedDetails.id > 0, "Trade closed");
		 m_Results.Verify("13.18", !string.IsNullOrEmpty(closedDetails.time), "Trade close details has time.");
		 m_Results.Verify("13.19", !string.IsNullOrEmpty(closedDetails.instrument), "Trade close details instrument correct.");
		 m_Results.Verify("13.20", closedDetails.units == -1 * trade.initialUnits, "Trade close details units correct.");
		 m_Results.Verify("13.21", closedDetails.tradesClosed[0].price > 0, "Trade close details has price.");
	  }
	  #endregion

	  #region Position
	  /// <summary>
	  /// Runs operations available at OANDA's Position endpoint
	  /// </summary>
	  /// <returns></returns>
	  private static async Task Position_RunPositionOperations()
	  {
		 if (await Utilities.IsMarketHalted())
			throw new MarketHaltedException("OANDA Fx market is halted!");

		 short units = 1;

		 // make sure there's a position to test
		 await PlaceMarketOrder("14", units);

		 // verification function
		 Func<Position, short, short> verifyPosition = (position, i) =>
		 {
			string key = "14.";

			m_Results.Verify(key + i.ToString(), position.@long != null, "Position has direction");
			m_Results.Verify(key + (i + 1).ToString(), position.@long.units > 0, "Position has units");
			m_Results.Verify(key + (i + 2).ToString(), position.@long.averagePrice > 0, "Position has avgPrice");
			m_Results.Verify(key + (i + 3).ToString(), !string.IsNullOrEmpty(position.instrument), "Position has instrument");

			return i += 4;
		 };

		 // get list of historical positions
		 // only open positions will have units > 0
		 var positions = await Rest20.GetPositionsAsync(AccountID);
		 m_Results.Verify("14.2", positions.Count > 0, "All historical positions retrieved");

		 // get list of open positions
		 var openPositions = await Rest20.GetOpenPositionsAsync(AccountID);
		 m_Results.Verify("14.3", openPositions.Count > 0, "Open positions retrieved");

		 short increment = 4;
		 foreach (var position in openPositions)
		 {
			increment = verifyPosition(position, increment);
		 }

		 // get position for a given instrument
		 var onePosition = await Rest20.GetPositionAsync(AccountID, m_TestInstrument);
		 increment = verifyPosition(onePosition, increment);

		 // error test - close open position
		 var parameters = new PositionCloseParameters() { longUnits = "FAKE" };
		 PositionCloseResponse response = null;
		 try { response = await Rest20.PutPositionCloseAsync(AccountID, m_TestInstrument, parameters); }
		 catch (Exception ex)
		 {
			var errorResponse = ErrorResponseFactory.Create(ex.Message) as PositionCloseErrorResponse;
			m_Results.Verify("14.E0", errorResponse != null, "Error response has correct type: PositionCloseErrorResponse");
		 }

		 // closeout open position 
		 parameters.longUnits = "ALL";
		 response = await Rest20.PutPositionCloseAsync(AccountID, m_TestInstrument, parameters);
		 m_LastTransactionID = response.lastTransactionID;
		 m_Results.Verify("14." + increment.ToString(), response.longOrderCreateTransaction != null && response.longOrderCreateTransaction.id > 0, "Position close order created.");
		 m_Results.Verify("14." + (increment + 1).ToString(), response.longOrderFillTransaction != null && response.longOrderFillTransaction.id > 0, "Position close fill order created.");
		 m_Results.Verify("14." + (increment + 2).ToString(), response.longOrderFillTransaction.units == -1 * units, "Position close units correct.");

	  }
	  #endregion

	  #region Transaction
	  private static async Task Transaction_GetTransactionsByDateRange()
	  {
		 // 09
		 var parameters = new TransactionsParameters()
		 {
			from = m_LastTransactionTime,
			type = new List<string>(new[] { TransactionType.ClientConfigure })
		 };

		 List<ITransaction> results = await Rest20.GetTransactionsAsync(AccountID, parameters);

		 m_Results.Verify("09.0", results != null, string.Format("Transactions info received.", AccountID));
		 m_Results.Verify("09.1", results.Where(x => x.type == TransactionType.ClientConfigure).Count() > 0, "Client configure transactions returned.");
		 m_Results.Verify("09.2", results.Where(x => x.type != TransactionType.ClientConfigure).Count() > 0, "Non-client configure transactions returned.");
	  }

	  private static async Task Transaction_GetTransaction()
	  {
		 //15
		 ITransaction result = await Rest20.GetTransactionAsync(AccountID, m_LastTransactionID);

		 m_Results.Verify("15.0", result != null, string.Format("Transaction info received."));
		 m_Results.Verify("15.1", result.id > 0, "Transaction has id.");
		 m_Results.Verify("15.2", result.type != null, "Transaction has type.");
		 m_Results.Verify("15.3", result.time != null, "Transaction has time.");
	  }

	  private static async Task Transaction_GetTransactionsByIdRange()
	  {
		 // 16
		 var parameters = new TransactionsByIdRangeParameters()
		 {
			from = m_FirstTransactionID,
			to = m_LastTransactionID
		 };

		 List<ITransaction> results = await Rest20.GetTransactionsByIdRangeAsync(AccountID, parameters);
		 results.OrderBy(x => x.id);

		 m_Results.Verify("16.0", results != null, string.Format("Transactions info received.", AccountID));
		 m_Results.Verify("16.1", results.First().id == m_FirstTransactionID, string.Format("Id of first transaction is correct.", results.First().id));
		 m_Results.Verify("16.2", results.Where(x => x.id < m_FirstTransactionID).Count() == 0, "All Id's are greater than first transactionId.");
		 m_Results.Verify("16.3", results.Where(x => x.type == TransactionType.ClientConfigure).Count() > 0, "Client configure transactions returned.");
		 m_Results.Verify("16.4", results.Where(x => x.type == TransactionType.MarketOrder).Count() > 0, "Market order transactions returned.");
	  }

	  private static async Task Transaction_GetTransactionsSinceId()
	  {
		 // 10
		 List<ITransaction> results = await Rest20.GetTransactionsSinceIdAsync(AccountID, m_LastTransactionID);
		 results.OrderBy(x => x.id);

		 m_Results.Verify("10.0", results != null, string.Format("Transactions info received.", AccountID));
		 m_Results.Verify("10.1", results.First().id == m_LastTransactionID + 1, string.Format("Id of first transaction is correct.", results.First().id));
		 m_Results.Verify("10.2", results.Where(x => x.id <= m_LastTransactionID).Count() == 0, "All Id's are greater than last transactionId.");
		 m_Results.Verify("10.3", results.Where(x => x.type == TransactionType.ClientConfigure).Count() > 0, "Client configure transactions returned.");
	  }
	  #endregion

	  #region Pricing
	  private static async Task Pricing_GetPricing()
	  {
		 var parameters = new PricingParameters() { instruments = new List<string>() };
		 m_OandaInstruments.ForEach(x => parameters.instruments.Add(x.name));

		 List<Price> prices = await Rest20.GetPricingAsync(AccountID, parameters);

		 m_Results.Verify("06.0", prices != null, string.Format("Prices retrieved successfully."));
		 m_Results.Verify("06.1", prices.Count == m_OandaInstruments.Count, string.Format("Correct count ({0}) of prices retrieved.", prices.Count));

		 m_OandaPrices = prices;
	  }
	  #endregion

	  #region Stream
	  static Semaphore _transactionReceived;
	  protected static Task Stream_GetStreamingTransactions()
	  {
		 // 07
		 TransactionsSession session = new TransactionsSession(AccountID);
		 _transactionReceived = new Semaphore(0, 100);
		 session.DataReceived += OnTransactionReceived;
		 session.StartSession();

		 // wait 10secs or until a message is received
		 bool success = _transactionReceived.WaitOne(10000);
		 m_Results.Verify("07.0", success, "Transaction events stream is functioning.");

		 return Task.Run(() =>
		 {
			// wait until a transaction is received .. max 20secs
			var autoStopTime = DateTime.UtcNow.AddSeconds(20);
			while (!_gotTransaction || DateTime.UtcNow < autoStopTime) { }
			session.StopSession();
		 });
	  }

	  static bool _gotTransaction = false;
	  protected static void OnTransactionReceived(TransactionsStreamResponse data)
	  {
		 if (!_gotTransaction)
		 {
			if (!data.IsHeartbeat())
			{
			   m_Results.Verify("07.1", data.transaction != null, "Transaction received");
			   if (data.transaction != null)
			   {
				  m_Results.Verify("07.2", data.transaction.id != 0, "Transaction has id.");
				  m_Results.Verify("07.3", data.transaction.accountID == AccountID, string.Format("Transaction has correct accountID: ({0}).", AccountID));
			   }

			   // only testing first data
			   _gotTransaction = true;
			}
		 }

		 _transactionReceived.Release();
	  }

	  static Semaphore _tickReceived;
	  protected static void Stream_GetStreamingPrices()
	  {
		 PricingSession session = new PricingSession(AccountID, m_OandaInstruments);
		 _tickReceived = new Semaphore(0, 100);
		 session.DataReceived += OnPricingReceived;
		 session.StartSession();
		 bool success = _tickReceived.WaitOne(10000);
		 session.StopSession();
		 m_Results.Verify("18.0", success, "Pricing stream is functioning.");
	  }

	  static bool _gotPrice = false;
	  protected static void OnPricingReceived(PricingStreamResponse data)
	  {
		 if (!_gotPrice)
		 {
			if (data.price != null)
			{
			   m_Results.Verify("18.1", data.price != null, "Pricing data received.");
			   m_Results.Verify("18.2", data.price.instrument != null, "Streaming price has instrument");

			   if (data.price.tradeable)
			   {
				  m_Results.Verify("18.3", data.price.bids.Count > 0, "Streaming price has bids");
				  m_Results.Verify("18.4", data.price.asks.Count > 0, "Streaming price has asks");
			   }

			   _gotPrice = true;
			}
		 }
		 _tickReceived.Release();
	  }
	  #endregion

	  #region Utilities
	  /// <summary>
	  /// Convert DateTime object to a string of the indicated format
	  /// </summary>
	  /// <param name="time">A DateTime object</param>
	  /// <param name="format">Format type (RFC3339 or UNIX only</param>
	  /// <returns>A date-time string</returns>
	  private static string ConvertDateTimeToAcceptDateFormat(DateTime time, AcceptDatetimeFormat format = AcceptDatetimeFormat.RFC3339)
	  {
		 // look into doing this within the JsonSerializer so that objects can use DateTime instead of string

		 if (format == AcceptDatetimeFormat.RFC3339)
			return XmlConvert.ToString(time, "yyyy-MM-ddTHH:mm:ssZ");
		 else if (format == AcceptDatetimeFormat.Unix)
			return ((int)(time.Subtract(new DateTime(1970, 1, 1))).TotalSeconds).ToString();
		 else
			throw new ArgumentException(string.Format("The value ({0}) of the format parameter is invalid.", (short)format));
	  }

	  /// <summary>
	  /// Reads the api key from a supplied file name
	  /// </summary>
	  /// <returns></returns>
	  private static async Task SetApiCredentials(string fileName = null)
	  {
		 fileName = fileName ?? @"C:\Users\Osita\SourceCode\GitHub\OandaV20\oandaPracticeApiCredentials.txt";

		 try
		 {
			using (StreamReader sr = new StreamReader(fileName))
			{
			   string apiCredentials = await sr.ReadToEndAsync();

			   // an OANDA trade or practice account is required to generate a valid token
			   // for info, go to: https://www.oanda.com/account/tpa/personal_token
			   m_TestToken = apiCredentials.Split('|')[0];

			   // this should be a v20 account, not a standard/legacy account
			   // if null, this will be set to the first v20 account found using the token above
			   m_TestAccount = apiCredentials.Split('|')[1];
			}
		 }
		 catch (DirectoryNotFoundException)
		 {
			// warning: do not rely on these
			//m_TestToken = "c9e9494d79013cac1d34f0e4dcb590cd-977a37b80762fb48cdb3b0b2e832628a";
			//m_TestAccount = "101-001-1913854-002";
		 }
		 catch (Exception ex)
		 {
			throw new Exception("Could not read api credentials.", ex);
		 }

		 // set this to the correct environment based on the account
		 //m_TestEnvironment = EEnvironment.Practice;

		 // set this to the correct number of v20 accounts associated with the token     
		 //m_TokenAccounts = 2;

		 Credentials.SetCredentials(m_TestEnvironment, m_TestToken, m_TestAccount);
	  }

	  private static Instrument GetOandaInstrument(string instrument = null)
	  {
		 instrument = instrument ?? m_TestInstrument;
		 return m_OandaInstruments.FirstOrDefault(x => x.name == instrument);
	  }

	  private static decimal GetOandaPrice(string instrument = null)
	  {
		 instrument = instrument ?? m_TestInstrument;
		 return m_OandaPrices.FirstOrDefault(x => x.instrument == instrument).closeoutBid;
	  }

	  /// <summary>
	  /// Computes and returns the next test characteristic (whole number)
	  /// </summary>
	  /// <returns></returns>
	  private static string GetTestNumber(bool incrementCharacteristic = false)
	  {
		 if (incrementCharacteristic)
			m_TestNumber = Math.Truncate(m_TestNumber) + 1;
		 else
			m_TestNumber = m_TestNumber + (decimal).001;

		 return m_TestNumber.ToString("0.000");
	  }

	  #endregion
   }
}
