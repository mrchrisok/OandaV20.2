using Microsoft.Identity.Client;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Net.Http;

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
      public HttpParameters() { }

      public HttpParameters(object payload)
      {
         if (payload == null) return;
         Data = payload is JToken jt ? jt : JToken.FromObject(payload);
      }

      public HttpParameters(object payload, JsonSerializerSettings settings)
      {
         JsonSettings = settings;
         if (payload == null) return;
         Data = payload is JToken jt ? jt : JToken.FromObject(payload);
      }

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