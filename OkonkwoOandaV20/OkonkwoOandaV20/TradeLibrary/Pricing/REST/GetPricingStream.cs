using Newtonsoft.Json;
using OkonkwoOandaV20.TradeLibrary.Pricing;
using OkonkwoOandaV20.TradeLibrary.REST.Streaming;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace OkonkwoOandaV20.TradeLibrary.REST
{
   public partial class Rest20
   {
      /// <summary>
      /// Get a pricing stream for the specified account and instrument list.
      /// </summary>
      /// <param name="parameters">the parameters for the request</param>
      /// <param name="cancellation">a cancellation token that can cancel the operation</param>
      /// <returns>The HttpResponseMessage that can be used to retrieve the prices as they stream</returns>
      public static async Task<HttpResponseMessage> GetPricingStream(PricingStreamParameters parameters, CancellationToken cancellation = default)
      {
         var requestParams = new HttpParameters(parameters)
         {
            Method = HttpMethod.Get,
            Uri = new Uri(ServerUri(EServer.PricingStream) + $"accounts/{parameters.accountID}/pricing/stream"),
            Binding = HttpParametersBinding.QueryString,
         };

         return await MakeStreamRequestAsync<PricingStreamErrorResponse>(requestParams, cancellation);
      }

      public class PricingStreamParameters : ApiParameters
      {
         /// <summary>
         /// Account Identifier [required]
         /// </summary>
         [JsonIgnore]
         [Required]
         public string accountID { get; set; }

         public PricingStreamParameters() { snapshot = true; }

         /// <summary>
         /// List of Instruments to stream Prices for. [required]
         /// </summary>
         public List<string> instruments { get; set; }

         /// <summary>
         /// Flag that enables/disables the sending of a pricing snapshot when 
         /// initially connecting to the stream. [default=True]
         /// </summary>
         public bool snapshot { get; set; }
      }
   }

   //[JsonConverter(typeof(PricingStreamResponseConverter))]
   public class PricingStreamResponse : IStreamResponse
   {
      public PricingHeartbeat heartbeat { get; set; }
      public Price price { get; set; }

      public bool IsHeartbeat()
      {
         return heartbeat != null;
      }
   }

   public class PricingStreamErrorResponse : ErrorResponse
   {

   }

   public class PricingHeartbeat : Heartbeat
   {
   }

   public class PricingSession : StreamSession<PricingStreamResponse>
   {
      private readonly List<Instrument.Instrument> _instruments;
      private bool _snapshot;

      public PricingSession(string accountID, List<Instrument.Instrument> instruments, bool snapshot = true)
         : base(accountID)
      {
         _instruments = instruments;
         _snapshot = snapshot;
      }

      protected override async Task<HttpResponseMessage> GetSession(CancellationToken cancellation = default)
      {
         var instruments = new List<string>();
         _instruments.ForEach(instrument => instruments.Add(instrument.name));

         var parameters = new Rest20.PricingStreamParameters()
         {
            accountID = _accountID,
            instruments = instruments,
            snapshot = _snapshot
         };

         return await Rest20.GetPricingStream(parameters, cancellation);
      }
   }
}
