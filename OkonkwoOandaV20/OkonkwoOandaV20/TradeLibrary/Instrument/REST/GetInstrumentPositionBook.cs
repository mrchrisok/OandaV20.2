using Newtonsoft.Json;
using OkonkwoOandaV20.Framework;
using OkonkwoOandaV20.TradeLibrary.Instrument;
using System;
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
      /// Fetch a position book for an instrument
      /// </summary>
      /// <param name="parameters">the parameters for the request</param>
      /// <param name="cancellation">a cancellation token that can cancel the operation</param>
      /// <returns>a PositionBook object</returns>
      public static async Task<InstrumentPositionBookResponse> GetInstrumentPositionBookAsync(InstrumentPositionBookParameters parameters , CancellationToken cancellation = default)
      {
         var requestParams = new HttpParameters(parameters)
         {
            Method = HttpMethod.Get,
            Uri = new Uri(ServerUri(EServer.Account) + "instruments/" + parameters.instrument + "/positionBook"),
            Binding = HttpParametersBinding.QueryString,
         };

         InstrumentPositionBookResponse response = null;
         try { response = await MakeRequestAsync<InstrumentPositionBookResponse, InstrumentPositionBookErrorResponse>(
                                                   requestParams, cancellation); }
         catch (Exception ex)
         {
            if (parameters.getLastTimeOnFailure)
            {
               parameters.time = null;
               response = await MakeRequestAsync<InstrumentPositionBookResponse, InstrumentPositionBookErrorResponse>(
                                                   requestParams, cancellation);
            }            
            else
               throw ex;
         }

         return response;
      } 

      public class InstrumentPositionBookParameters : InstrumentParameters
      {
         public InstrumentPositionBookParameters(bool getLastTimeOnFailure = true)
         {
            this.getLastTimeOnFailure = getLastTimeOnFailure;
         }

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
   /// The GET success response received from instruments/instrument/positionBook
   /// </summary>
   public class InstrumentPositionBookResponse : Response
   {
      /// <summary>
      /// The instrument’s position book
      /// </summary>
      public PositionBook positionBook;
   }

   /// <summary>
   /// The GET error response received from instruments/instrument/positionBook
   /// </summary>
   public class InstrumentPositionBookErrorResponse : ErrorResponse
   {
   }
}