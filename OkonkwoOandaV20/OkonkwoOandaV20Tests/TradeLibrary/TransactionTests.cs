using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Linq;

namespace OkonkwoOandaV20Tests.TradeLibrary
{
   [TestClass]
   public class TransactionTests : Rest20TestsBase
   {
	  #region Transaction

	  [TestMethod]
	  public void success_get_transactions_by_date_range()
	  {
		 var transactionsReceived = Results.Items.FirstOrDefault(x => x.Key == "09.0").Value;
		 var clientConfigureReceived = Results.Items.FirstOrDefault(x => x.Key == "09.1").Value;
		 var notClientConfigureReceived = Results.Items.FirstOrDefault(x => x.Key == "09.2").Value;

		 Assert.IsTrue(transactionsReceived.Success, transactionsReceived.Success.ToString() + ": " + transactionsReceived.Details);
		 Assert.IsTrue(clientConfigureReceived.Success, clientConfigureReceived.Success.ToString() + ": " + clientConfigureReceived.Details);
		 Assert.IsTrue(transactionsReceived.Success, transactionsReceived.Success.ToString() + ": " + transactionsReceived.Details);
		 Assert.IsFalse(notClientConfigureReceived.Success, notClientConfigureReceived.Success.ToString() + ": " + notClientConfigureReceived.Details);
	  }

	  [TestMethod]
	  public void success_get_transactions_since_id()
	  {
		 var transactionsReceived = Results.Items.FirstOrDefault(x => x.Key == "10.0").Value;
		 var firstIdIsNextId = Results.Items.FirstOrDefault(x => x.Key == "10.1").Value;
		 var allIdsGreaterThanLastId = Results.Items.FirstOrDefault(x => x.Key == "10.2").Value;
		 var clientConfigureReceived = Results.Items.FirstOrDefault(x => x.Key == "10.3").Value;

		 Assert.IsTrue(transactionsReceived.Success, transactionsReceived.Success.ToString() + ": " + transactionsReceived.Details);
		 Assert.IsTrue(firstIdIsNextId.Success, firstIdIsNextId.Success.ToString() + ": " + firstIdIsNextId.Details);
		 Assert.IsTrue(allIdsGreaterThanLastId.Success, allIdsGreaterThanLastId.Success.ToString() + ": " + allIdsGreaterThanLastId.Details);
		 Assert.IsTrue(clientConfigureReceived.Success, clientConfigureReceived.Success.ToString() + ": " + clientConfigureReceived.Details);
	  }

	  [TestMethod]
	  public void success_get_transaction_detail()
	  {
		 var transactionReceived = Results.Items.FirstOrDefault(x => x.Key == "15.0").Value;
		 var transactionHasId = Results.Items.FirstOrDefault(x => x.Key == "15.1").Value;
		 var transactionHasType = Results.Items.FirstOrDefault(x => x.Key == "15.2").Value;
		 var transactionHasTime = Results.Items.FirstOrDefault(x => x.Key == "15.3").Value;

		 Assert.IsTrue(transactionReceived.Success, transactionReceived.Success.ToString() + ": " + transactionReceived.Details);
		 Assert.IsTrue(transactionHasId.Success, transactionHasId.Success.ToString() + ": " + transactionHasId.Details);
		 Assert.IsTrue(transactionHasType.Success, transactionHasType.Success.ToString() + ": " + transactionHasType.Details);
		 Assert.IsTrue(transactionHasTime.Success, transactionHasTime.Success.ToString() + ": " + transactionHasTime.Details);
	  }

	  [TestMethod]
	  public void success_get_transactions_by_id_range()
	  {
		 var transactionsReceived = Results.Items.FirstOrDefault(x => x.Key == "16.0").Value;
		 var firstIdIsCorrect = Results.Items.FirstOrDefault(x => x.Key == "16.1").Value;
		 var allIdsGreaterThanFirst = Results.Items.FirstOrDefault(x => x.Key == "16.2").Value;
		 var clientConfiguredReturned = Results.Items.FirstOrDefault(x => x.Key == "16.3").Value;
		 var marketOrdersReturned = Results.Items.FirstOrDefault(x => x.Key == "16.4").Value;

		 Assert.IsTrue(transactionsReceived.Success, transactionsReceived.Success.ToString() + ": " + transactionsReceived.Details);
		 Assert.IsTrue(firstIdIsCorrect.Success, firstIdIsCorrect.Success.ToString() + ": " + firstIdIsCorrect.Details);
		 Assert.IsTrue(allIdsGreaterThanFirst.Success, allIdsGreaterThanFirst.Success.ToString() + ": " + allIdsGreaterThanFirst.Details);
		 Assert.IsTrue(clientConfiguredReturned.Success, clientConfiguredReturned.Success.ToString() + ": " + clientConfiguredReturned.Details);
		 Assert.IsTrue(marketOrdersReturned.Success, marketOrdersReturned.Success.ToString() + ": " + marketOrdersReturned.Details);
	  }

	  [TestMethod]
	  public void success_transaction_stream_functional()
	  {
		 // 7
		 var streamFunctional = Results.Items.FirstOrDefault(x => x.Key == "07.0").Value;
		 var dataReceived = Results.Items.FirstOrDefault(x => x.Key == "07.1").Value;
		 var dataHasId = Results.Items.FirstOrDefault(x => x.Key == "07.2").Value;
		 var dataHasAccountId = Results.Items.FirstOrDefault(x => x.Key == "07.3").Value;

		 Assert.IsTrue(streamFunctional.Success, streamFunctional.Success.ToString() + ": " + streamFunctional.Details);
		 Assert.IsTrue(dataReceived.Success, dataReceived.Success.ToString() + ": " + dataReceived.Details);
		 Assert.IsTrue(dataHasId.Success, dataHasId.Success.ToString() + ": " + dataHasId.Details);
		 Assert.IsTrue(dataHasAccountId.Success, dataHasAccountId.Success.ToString() + ": " + dataHasAccountId.Details);
	  }

	  #endregion
   }
}
