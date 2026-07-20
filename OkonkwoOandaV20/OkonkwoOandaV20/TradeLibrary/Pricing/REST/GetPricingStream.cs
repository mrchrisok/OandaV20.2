using Newtonsoft.Json;
using OkonkwoOandaV20.TradeLibrary.Pricing;
using OkonkwoOandaV20.TradeLibrary.REST.Streaming;
using System;
using System.Collections.Generic;
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
      /// <param name="accountID"></param>
      /// <param name="parameters">The parameters for the request</param>
      /// <returns>The HttpResponseMessage that can be used to retrieve the prices as they stream</returns>
      public static async Task<HttpResponseMessage> GetPricingStream(string accountID, PricingStreamParameters parameters, CancellationToken cancellation = default)
      {
         var requestDict = new Dictionary<string, string>(ConvertToDictionary(parameters))
         {
            { "instruments", GetCommaSeparatedString(parameters.instruments ?? new List<string>()) }
         };

         var requestParams = new HttpParameters(requestDict)
         {
            Method = HttpMethod.Get,
            Uri = new Uri(ServerUri(EServer.PricingStream) + $"accounts/{accountID}/pricing/stream"),
            Binding = HttpParametersBinding.QueryString,
         };

         return await MakeStreamRequestAsync<PricingStreamErrorResponse>(requestParams, cancellation);
      }

      public class PricingStreamParameters : ApiParameters
      {
         public PricingStreamParameters() { snapshot = true; }

         /// <summary>
         /// List of Instruments to stream Prices for. [required]
         /// </summary>
         [JsonIgnore]
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
         return (heartbeat != null);
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

      protected override async Task<HttpResponseMessage> GetSession()
      {
         var instruments = new List<string>();
         _instruments.ForEach(instrument => instruments.Add(instrument.name));

         var parameters = new Rest20.PricingStreamParameters()
         {
            instruments = instruments,
            snapshot = _snapshot
         };

         return await Rest20.GetPricingStream(_accountID, parameters);
      }
   }
}
