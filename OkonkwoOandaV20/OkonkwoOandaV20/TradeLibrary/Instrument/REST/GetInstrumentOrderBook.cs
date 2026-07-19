using Newtonsoft.Json;
using OkonkwoOandaV20.Framework.Factories;
using OkonkwoOandaV20.TradeLibrary.Instrument;
using System;
using System.ComponentModel.DataAnnotations;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace OkonkwoOandaV20.TradeLibrary.REST
{
   /// <summary>
   /// http://developer.oanda.com/rest-live-v20/account-ep/
   /// </summary>
   public partial class Rest20
   {
      /// <summary>
      /// Retrieves the order book for an instrument
      /// </summary>
      /// <param name="instrument">Name of the Instrument [required]</param>
      /// <param name="parameters">the parameters for the request</param>
      /// <returns>an OrderBook object</returns>
      public static async Task<OrderBook> GetInstrumentOrderBookAsync(InstrumentOrderBookParameters parameters, CancellationToken cancellation = default)
      {
         HttpParameters requestParams() => new HttpParameters(parameters)
         {
            Method = HttpMethod.Put,
            Uri = new Uri(ServerUri(EServer.Account) + "instruments/" + parameters.instrument + "/orderBook"),
            Binding = HttpParametersBinding.QueryString
         };

         InstrumentOrderBookResponse response = null;
         try { response = await MakeRequestAsync<InstrumentOrderBookResponse, InstrumentOrderBookErrorResponse>(
                                                   requestParams(), cancellation); 
         }
         catch (Exception ex)
         {
            if (parameters.getLastTimeOnFailure)
            {
               parameters.time = null;
               response = await MakeRequestAsync<InstrumentOrderBookResponse, InstrumentOrderBookErrorResponse>(
                                                   requestParams(), cancellation);
            }
            else
               throw ex;
         }

         return response.orderBook;
      }

      public class InstrumentOrderBookParameters
      {
         public InstrumentOrderBookParameters(bool getLastTimeOnFailure = true)
         {
            this.getLastTimeOnFailure = getLastTimeOnFailure;
         }

         /// <summary>
         /// Name of the Instrument [required]
         /// </summary>
         [JsonIgnore]
         [Required]
         public string instrument { get; set; }

         /// <summary>
         /// The time of the snapshot to fetch. If not specified, then the most recent snapshot 
         /// is fetched.
         /// </summary>
         public string time { get; set; }

         /// <summary>
         /// 
         /// </summary>
         [JsonIgnore]
         public bool getLastTimeOnFailure { get; set; }
      }
   }

   /// <summary>
   /// The GET success response received from instruments/instrument/orderBook
   /// </summary>
   public class InstrumentOrderBookResponse : Response
   {
      /// <summary>
      /// The instrument’s order book
      /// </summary>
      public OrderBook orderBook;
   }

   /// <summary>
   /// The GET error response received from instruments/instrument/orderBook
   /// </summary>
   public class InstrumentOrderBookErrorResponse : ErrorResponse
   {
   }
}