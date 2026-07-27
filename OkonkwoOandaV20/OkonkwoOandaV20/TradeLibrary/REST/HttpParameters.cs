using Microsoft.Identity.Client;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json.Serialization;
using OkonkwoOandaV20.Framework;
using OkonkwoOandaV20.TradeLibrary.Transaction;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Reflection;
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
      public HttpParameters(Rest20 client, object parameters, JsonSerializerSettings jsonSettings)
      {
         if (parameters == null) return;

         if (parameters is ApiParameters apiParameters && !apiParameters.ForInternalRequest)
            client.TransformObjectValues(parameters, HttpAction.Request);

         jsonSettings = jsonSettings ?? (parameters as ApiParameters)?.JsonSettingsRequest;
         jsonSettings = jsonSettings ?? client.JsonSettingsRequest;
         //jsonSettings.ContractResolver = new CamelCasePropertyNamesContractResolver();
         var jsonSerializer = JsonSerializer.CreateDefault(jsonSettings);

         if (parameters.GetType().GetProperties().FirstOrDefault(
               p => Attribute.IsDefined(p, typeof(BodyAttribute))) is PropertyInfo bodyProperty)
         {
            Data = JToken.FromObject(bodyProperty.GetValue(parameters), jsonSerializer);
            return;
         }

         Data = parameters is JToken jt ? jt : JToken.FromObject(parameters, jsonSerializer);
      }

      public HttpParameters(Rest20 client, object payload) 
         : this(client, payload, null)
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
      internal bool ForInternalResponse { get; set; } = false;
   }
}