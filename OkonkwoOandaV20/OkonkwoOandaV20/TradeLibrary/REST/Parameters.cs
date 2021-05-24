using OkonkwoOandaV20.Framework;
using OkonkwoOandaV20.TradeLibrary.REST.OrderRequests;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace OkonkwoOandaV20.TradeLibrary.REST
{
   public abstract class Parameters
   {
	  #region headers

	  [Header("Accept-Datetime-Format")]
	  public virtual AcceptDatetimeFormat? AcceptDatetimeFormat { get; set; } = REST.AcceptDatetimeFormat.RFC3339;

	  #endregion

	  internal virtual IDictionary<string, object> GetRequestParameters<P>() where P : RequestAttribute
	  {
		 var parametersProperties = GetObjectParameters<P>(this);

		 var order = this.GetType().GetProperties().Where(prop => prop.GetType() == typeof(OrderRequest)).FirstOrDefault();

		 if (order != default)
		 {
			var orderParameters = GetObjectParameters<P>(order);
			foreach (var parameter in orderParameters.ToList())
			{
			   parametersProperties.Add(parameter.Key, parameter.Value);
			}
		 }

		 return parametersProperties;
	  }

	  private Dictionary<string, object> GetObjectParameters<P>(object obj) where P : RequestAttribute
	  {
		 var requestParameters = obj.GetType()
			.GetProperties()
			.Where(prop => prop.GetCustomAttribute<P>() != null)
			.Select(prop =>
			{
			   var customName = prop.GetCustomAttribute<P>().Name;
			   var propName = !string.IsNullOrWhiteSpace(customName) ? customName : prop.Name;
			   return new KeyValuePair<string, object>(propName, prop.GetValue(this));
			})
			.ToDictionary(x => x.Key, x => x.Value);

		 return requestParameters;
	  }
   }
}
