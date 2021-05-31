using OkonkwoOandaV20.TradeLibrary.Transaction;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace OkonkwoOandaV20.Framework.Factories
{
   public class TransactionFactory
   {
	  public static List<ITransaction> Create(IEnumerable<ITransaction> data)
	  {
		 var transactions = new List<ITransaction>();

		 foreach (ITransaction transaction in data)
		 {
			transactions.Add(Create(transaction.type));
		 }

		 return transactions;
	  }

	  private static FieldInfo[] _transactionTypeFields = typeof(TransactionType).GetFields();

	  private static IEnumerable<Type> _transactionTypes = typeof(ITransaction).Assembly.GetTypes()
		 .Where(type => type.GetTypeInfo().ImplementedInterfaces.Contains(typeof(ITransaction)));

	  public static ITransaction Create(string typeName)
	  {
		 var thisTransactionTypeField = _transactionTypeFields.FirstOrDefault(field => field.GetRawConstantValue().ToString() == typeName);
		 var transactionTypeName = $"{thisTransactionTypeField.Name}Transaction";
		 var transactionType = _transactionTypes.FirstOrDefault(type => type.Name == transactionTypeName);
		 var typeInstance = Activator.CreateInstance(transactionType ?? typeof(Transaction));

		 return typeInstance as ITransaction;
	  }
   }
}
