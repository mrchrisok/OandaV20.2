﻿using OkonkwoOandaV20.TradeLibrary.REST;
using System.Collections.Generic;
using System.Threading.Tasks;
using static OkonkwoOandaV20.TradeLibrary.REST.Rest20;

namespace OkonkwoOandaV20.Framework
{
   public class Utilities
   {
      /// <summary>
      /// Determines if trading is halted for the provided instrument.
      /// </summary>
      /// <param name="instrument">Instrument to check if halted. Default is EUR_USD.</param>
      /// <returns>True if trading is halted, false if trading is not halted.</returns>
      public static async Task<bool> IsMarketHalted(string instrument = InstrumentName.Currency.EURUSD, bool throwIfHalted = false)
      {
         var accountID = Credentials.GetDefaultCredentials().DefaultAccountId;
         var parameters = new PricingParameters() { instruments = new List<string>() { instrument } };

         var prices = await Rest20.GetPricingAsync(accountID, parameters);

         bool isTradeable = false, hasBids = false, hasAsks = false;

         if (prices[0] != null)
         {
            isTradeable = prices[0].tradeable;
            hasBids = prices[0].bids.Count > 0;
            hasAsks = prices[0].asks.Count > 0;
         }

         if (isTradeable && hasBids && hasAsks)  // not halted
            return false;

         if (throwIfHalted)
            throw new MarketHaltedException($"Market is halted for this instrument: {instrument}");

         return true;
      }
   }
}
