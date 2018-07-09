namespace OkonkwoOandaV20.TradeLibrary.REST
{
   /// <summary>
   /// DateTime header added to each v20 request
   /// http://developer.oanda.com/rest-live-v20/primitives-df/
   /// </summary>
   public enum AcceptDatetimeFormat
   {
      /// <summary>
      /// If “UNIX” is specified DateTime fields will be specified or returned in the “12345678.000000123” format.
      /// </summary>
      Unix,

      /// <summary>
      /// If “RFC3339” is specified DateTime will be specified or returned in “YYYY-MM-DDTHH:MM:SS.nnnnnnnnnZ” format.
      /// </summary>
      RFC3339
   }

   public class InstrumentName
   {
      /// <summary>
      /// Quotes and trading of these currency pairs are supported via v20 as of 2018-07-06.
      /// Visit <see cref="https://www.oanda.com/currency/live-exchange-rates/">Oanda's Live Exchange Rates</see> page 
      /// for a complete and current list of supported pairs.
      /// </summary>
      public class Currency
      {
         public const string EURUSD = "EUR_USD";
         public const string USDJPY = "USD_JPY";
         public const string AUDUSD = "AUD_USD";
         public const string AUDJPY = "AUD_JPY";
         public const string GBPUSD = "GBP_USD";
         public const string NZDUSD = "NZD_USD";
         public const string USDCHF = "USD_CHF";
         public const string USDCAD = "USD_CAD";
      }
   }
}
