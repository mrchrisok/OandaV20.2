using OkonkwoOandaV20.TradeLibrary.Transaction;
using System.Collections.Generic;

namespace OkonkwoOandaV20.TradeLibrary.REST
{
   /// <summary>
   /// The GET success response received from accounts/accountID/transactions/idrange or similar
   /// </summary>
   public class TransactionsResponse : Response
   {
      /// <summary>
      /// The list of Transaction objects returned by the request
      /// </summary>
      public List<ITransaction> transactions { get; set; }
   }

   /// <summary>
   /// The GET error response received from accounts/accountID/transactions endpoints
   /// </summary>
   public class TransactionsErrorResponse : ErrorResponse
   {
   }
}