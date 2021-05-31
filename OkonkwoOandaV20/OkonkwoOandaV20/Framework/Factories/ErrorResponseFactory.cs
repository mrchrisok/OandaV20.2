using Newtonsoft.Json;
using OkonkwoOandaV20.TradeLibrary.REST;
using System;
using System.Collections.Generic;
using System.Linq;

namespace OkonkwoOandaV20.Framework.Factories
{
   public class ErrorResponseFactory
   {
	  private readonly static IEnumerable<Type> _errorResponseTypes =
		 typeof(ErrorResponse).Assembly.GetTypes().Where(type => type.GetInterfaces().Contains(typeof(IErrorResponse)));

	  public static IErrorResponse Create(string message)
	  {
		 var dynamicError = JsonConvert.DeserializeObject<dynamic>(message);
		 var errorType = dynamicError.type.ToString();

		 // the 'type' property is injected by the Rest20.GetWebResponse catch block

		 var errorResponseType = _errorResponseTypes.FirstOrDefault(type => type.Name == errorType);

		 var errorResponse = JsonConvert.DeserializeObject(message, errorResponseType ?? typeof(ErrorResponse));

		 return errorResponse as IErrorResponse;
	  }
   }
}
