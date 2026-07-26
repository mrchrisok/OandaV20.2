using Newtonsoft.Json;
using OkonkwoOandaV20.TradeLibrary.Pricing;
using OkonkwoOandaV20.TradeLibrary.REST.Streaming;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using static OkonkwoOandaV20.TradeLibrary.REST.Rest20;

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
      public virtual async Task<HttpResponseMessage> GetPricingStream(PricingStreamParameters parameters, CancellationToken cancellation = default)
      {
         var requestParams = new HttpParameters(this, parameters)
         {
            Method = HttpMethod.Get,
            Uri = new Uri(ServerUri(EServer.PricingStream) + $"accounts/{parameters.accountID}/pricing/stream"),
            Binding = HttpParametersBinding.QueryString,
         };

         return await MakeStreamRequestAsync<PricingStreamErrorResponse>(requestParams, cancellation);
      }

      public class PricingStreamParameters : ApiParameters
      {
         public PricingStreamParameters() { snapshot = true; }

         /// <summary>
         /// Account Identifier [required]
         /// </summary>
         [JsonIgnore]
         [Required]
         public string accountID { get; set; }

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
      public PricingSession(Rest20 client, PricingSessionParameters parameters)
         : base(client, parameters.accountID)
      {
         _parameters = parameters;
      }

      protected readonly PricingSessionParameters _parameters;

      protected override async Task<HttpResponseMessage> GetSession(CancellationToken cancellation = default)
      {
         //var instruments = new List<string>(_parameters.instruments);
         //_parameters.instruments.ForEach(instrument => instruments.Add(instrument.name));

         var parameters = new PricingSessionParameters()
         {
            accountID = _parameters.accountID,
            instruments = new List<string>(_parameters.instruments),
            snapshot = _parameters.snapshot
         };

         return await _client.GetPricingStream(_parameters, cancellation);
      }
   }

   public class PricingSessionParameters : PricingStreamParameters
   {
   }
}
