namespace OkonkwoOandaV20.TradeLibrary.Instrument
{
   public class Candlestick
   {
      public string time { get; set; }
      public CandleStickData bid { get; set; }
      public CandleStickData ask { get; set; }
      public CandleStickData mid { get; set; }
      public int volume { get; set; }
      public bool complete { get; set; }
      public string instrument { get; set; }
      public string granularity { get; set; }
   }
}
