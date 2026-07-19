using Microsoft.Identity.Client;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
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
      public HttpParameters(object payload, JsonSerializerSettings settings) : this(payload)
      {
         JsonSettings = settings;
      }

      public HttpParameters(object payload)
      {
         Rest20.TransformObjectValues(payload);

         if (payload == null) return;
         Data = payload is JToken jt ? jt : JToken.FromObject(payload);
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