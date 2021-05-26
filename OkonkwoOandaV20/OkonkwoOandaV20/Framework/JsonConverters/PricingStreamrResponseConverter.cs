﻿using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using OkonkwoOandaV20.TradeLibrary.Pricing;
using OkonkwoOandaV20.TradeLibrary.REST;
using OkonkwoOandaV20.TradeLibrary.REST.Streaming;
using System;
using System.Linq;
using System.Reflection;

namespace OkonkwoOandaV20.Framework.JsonConverters
{
   public class PricingStreamResponseConverter : AbstractJsonConverter
   {
	  public override bool CanConvert(Type objectType)
	  {
		 bool canConvert = objectType.GetTypeInfo().ImplementedInterfaces.Contains(typeof(IHeartbeat));
		 return canConvert;
	  }

	  public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
	  {
		 var response = new PricingStreamResponse();

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
			throw new ArgumentException($"Unexpected JTokenType ({jsonToken.Type}) in reader.");
	  }
   }
}
