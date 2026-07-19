using Microsoft.Identity.Client;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using OkonkwoOandaV20.TradeLibrary.Common;
using System;
using System.Net.Http;
using System.Security.Cryptography;

namespace OkonkwoOandaV20.TradeLibrary.REST
{
   public enum HttpParametersBinding
   {
      QueryString,
      Body,
      FormUrlEncoded
   }

   public class HttpParameters
   {
      public HttpParameters(object payload, JsonSerializerSettings settings)
      {
         if (payload == null) return;

         Rest20.TransformObjectValues(payload);

         JsonSettings = settings ?? (payload as ApiParameters)?.JsonSerializerSettingsRequest;
         JsonSettings = JsonSettings ?? Rest20.JsonSettingsRequest;
         var jsonSerializer = JsonSerializer.CreateDefault(JsonSettings);

         Data = payload is JToken jt ? jt : JToken.FromObject(payload, jsonSerializer);
      }

      public HttpParameters(object payload) : this(payload, null)
      {
      }

      public HttpParameters() { }

      public HttpMethod Method { get; set; } = HttpMethod.Get;
      public Uri Uri { get; set; }
      public HttpParametersBinding Binding { get; set; } = HttpParametersBinding.QueryString;
      public HttpCompletionOption CompletionOption { get; set; } = HttpCompletionOption.ResponseHeadersRead;

      // Can be JObject / JArray / primitive token
      public JToken Data { get; set; }

      public string AcceptType { get; set; } = "application/json";
      public string ContentType { get; set; } = "application/json";

      public JsonSerializerSettings JsonSettings { get; set; }
   }
}