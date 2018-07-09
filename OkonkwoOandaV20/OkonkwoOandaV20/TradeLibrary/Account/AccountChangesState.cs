using OkonkwoOandaV20.TradeLibrary.DataTypes.Position;
using OkonkwoOandaV20.TradeLibrary.DataTypes.Order;
using OkonkwoOandaV20.TradeLibrary.DataTypes.Trade;
using System.Collections.Generic;

namespace OkonkwoOandaV20.TradeLibrary.DataTypes.Account
{
   /// <summary>
   /// http://developer.oanda.com/rest-live-v20/account-df/#AccountChangesState
   /// </summary>
   public class AccountChangesState : CalculatedAccountState
   {
      public List<DynamicOrderState> orders { get; set; }
      public List<CalculatedTradeState> trades { get; set; }
      public List<CalculatedPositionState> positions { get; set; }
   }
}
