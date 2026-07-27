using Newtonsoft.Json;

namespace OkonkwoOandaV20.TradeLibrary.REST
{
   /// <summary>
   /// Base class for API parameters, providing a common structure for all parameter classes used in API requests.
   /// </summary>
   public abstract class ApiParameters
   {
      // base class
      // can be used to introduce shared properties and behavior as needed

      /// <summary>
      /// A JsonSerializerSettings that can be applied to this request only.
      /// If null, the TSV3 JsonSerialzerSettings (if configured) will be used.
      /// </summary>
      [JsonIgnore]
      public JsonSerializerSettings JsonSettingsRequest { get; set; } = Rest20.JsonSettingsRequest;

      /// <summary>
      /// A JsonSerializerSettings that can be applied for this response only.
      /// If null, the TSV3 JsonSerialzerSettings (if configured) will be used.
      /// </summary>
      [JsonIgnore]
      public JsonSerializerSettings JsonSettingsResponse { get; set; } = Rest20.JsonSettingsResponse;

      internal bool ForInternalRequest { get; set; } = false;
   }

   /// <summary>
   /// Class for stream API parameters, providing a common structure for all parameter classes used in API requests.
   /// </summary>
   public abstract class StreamApiParameters : ApiParameters
   {
      /// <summary>
      /// A unique string used to identify the stream in client systems
      /// </summary>
      //[Required]
      //[JsonIgnore]
      //public string StreamID { get; set; } = Utilities.GenerateSecureRandomString(10);
   }
}
