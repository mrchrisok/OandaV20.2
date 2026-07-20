using Newtonsoft.Json;
using System.Collections.Generic;

namespace OkonkwoOandaV20.TradeLibrary.REST.Streaming
{
   public interface IStreamResponse
   {
      bool IsHeartbeat();
   }

   public abstract class StreamResponse
   {
      public Heartbeat heartbeat { get; set; }

      public bool IsHeartbeat()
      {
         return heartbeat != null;
      }
   }

   public abstract class Heartbeat
   {
      public string type { get; set; }
      public string time { get; set; }
   }
}
