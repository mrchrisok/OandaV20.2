using OkonkwoCore.Common.Contracts;

namespace OkonkwoOandaV20.TradeLibrary.REST.Streaming
{
   public class StreamResponse : IStreamChunkResponse
   {
      /// <summary>
      /// Sent every 5 seconds to keep the stream alive
      /// </summary>
      public virtual IHeartbeat heartbeat { get; set; }

      /// <summary>
      /// Determines if the stream response is a Heartbeat
      /// </summary>
      /// <returns>True if a Heartbeat, otherwise False.</returns>
      public bool IsHeartbeat()
      {
         return heartbeat != null;
      }

      /// <summary>
      /// Determines if the stream response is a StreamStatus
      /// </summary>
      /// <returns>True if a StreamStatus, otherwise False.</returns>
      public bool IsStatus()
      {
         // Oanda does not return status in the stream, so this will always be false.
         return false;
      }

      /// <summary>
      /// Determines if the stream response is an error
      /// </summary>
      /// <returns>True if an error, otherwise False.</returns>
      public bool IsError()
      {
         // Oanda does not return errors in the stream, so this will always be false.
         return false;
      }
   }
}
