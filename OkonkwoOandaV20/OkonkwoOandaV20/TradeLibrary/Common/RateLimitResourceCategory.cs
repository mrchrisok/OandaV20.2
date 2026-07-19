using System;
using System.Collections.Generic;
using System.Text;

namespace OkonkwoOandaV20.TradeLibrary.Common
{
   /// <summary>
   /// Enumeration of resource categories for rate limits
   /// </summary>
   public enum RateLimitResourceCategory
   {
      /// <summary>
      /// Account
      /// </summary>
      Account,

      /// <summary>
      /// Instrument
      /// </summary>
      Instrument,

      /// <summary>
      /// Order
      /// </summary>
      Order,

      /// <summary>
      /// Position
      /// </summary>
      Position,

      /// <summary>
      /// Pricing
      /// </summary>
      Pricing,

      /// <summary>
      /// Streaming
      /// </summary>
      Streaming,

      /// <summary>
      /// Trade
      /// </summary>
      Trade,

      /// <summary>
      /// Transaction
      /// </summary>
      Transaction
   }
}
