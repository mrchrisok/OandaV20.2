using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using System.Net.Http;
using System;
using Newtonsoft.Json;
using System.ComponentModel.DataAnnotations;

namespace OkonkwoOandaV20.TradeLibrary.REST
{
   public partial class Rest20
   {
      /// <summary>
      /// Retrieves all positions for a given account
      /// </summary>
      /// <param name="parameters">the parameters for the request</param>
      /// <param name="cancellation">a cancellation token that can cancel the operation</param>
      /// <returns>List of Position objects with the details for each position (or empty list iff no positions)</returns>
      public static async Task<PositionsResponse> GetPositionsAsync(PositionsParameters parameters, CancellationToken cancellation = default)
      {
         var requestParams = new HttpParameters(parameters)
         {
            Method = HttpMethod.Get,
            Uri = new Uri(ServerUri(EServer.Account) + $"accounts/{parameters.accountID}/positions"),
         };

         var response = await MakeRequestAsync<PositionsResponse, PositionsErrorResponse>(requestParams, cancellation);

         return response;
      }
   }

   public class PositionsParameters : ApiParameters
   {
      /// <summary>
      /// Account Identifier [required]
      /// </summary>
      [JsonIgnore]
      [Required]
      public string accountID { get; set; }
   }

   /// <summary>
   /// The GET success response received from accounts/accountID/positions/instrument/close
   /// </summary>
   public class PositionsResponse : Response
   {
      /// <summary>
      /// The list of Account Positions
      /// </summary>
      public List<Position.Position> positions { get; set; }
   }

   /// <summary>
   /// The GET error response received from accounts/accountID/positions/instrument/close
   /// </summary>
   public class PositionsErrorResponse : ErrorResponse
   {
   }
}
