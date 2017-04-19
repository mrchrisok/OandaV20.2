﻿using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using OANDAV20.TradeLibrary.DataTypes.Pricing;
using OANDAV20.TradeLibrary.DataTypes.Stream;
using System;

namespace OANDAV20.Framework.JsonConverters
{
   public class PricingStreamResponseConverter : JsonConverterBase
   {
      public override bool CanConvert(Type objectType)
      {
         bool canConvert = objectType.GetInterface("IHeartbeat") != null;
         return canConvert;
      }

      public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
      {
         PricingStreamResponse response = new PricingStreamResponse();

         var jsonToken = JToken.Load(reader);

         if (jsonToken.Type == JTokenType.Object)
         {
            bool isHeartbeat = jsonToken["type"].Value<string>() == "HEARTBEAT";

            if (isHeartbeat)
            {
               var heartbeat = new PricingHeartbeat();
               serializer.Populate(jsonToken.CreateReader(), heartbeat);
               response.heartbeat = heartbeat;
            }
            else
            {
               Price price = new Price();
               serializer.Populate(jsonToken.CreateReader(), price);
               response.price = price;
            }

            return response;
         }
         else
            throw new ArgumentException(string.Format("Unexpected JTokenType ({0}) in reader.", jsonToken.Type.ToString()));
      }
   }
}