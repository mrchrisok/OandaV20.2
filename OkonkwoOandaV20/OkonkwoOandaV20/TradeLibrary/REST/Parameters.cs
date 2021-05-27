using OkonkwoOandaV20.Framework;
using OkonkwoOandaV20.Framework.TypeConverters;
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

	  internal virtual IDictionary<string, object> GetRequestParameters<P>(IList<ITypeConverter<string>> converters = null, bool excludeNulls = true)
		 where P : RequestAttribute
	  {
		 var parametersProperties = GetObjectParameters<P>(this, converters, excludeNulls);

		 var order = this.GetType().GetProperties().Where(prop => prop.GetType() == typeof(OrderRequest)).FirstOrDefault();

		 if (order != default)
		 {
			var orderParameters = GetObjectParameters<P>(order, converters, excludeNulls);
			foreach (var parameter in orderParameters.ToList())
			{
			   parametersProperties.Add(parameter.Key, parameter.Value);
			}
		 }

		 return parametersProperties;
	  }

	  private Dictionary<string, object> GetObjectParameters<P>(object obj, IList<ITypeConverter<string>> converters = null, bool excludeNulls = true)
		 where P : RequestAttribute
	  {
		 var requestParameters = obj.GetType()
			.GetProperties(BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance | BindingFlags.Static)
			.Where(prop => prop.GetCustomAttribute<P>() != null)
			.Select(prop =>
			{
			   var customName = prop.GetCustomAttribute<P>().Name;
			   var propName = !string.IsNullOrWhiteSpace(customName) ? customName : prop.Name;
			   var propValue = GetPropertyValue<P>(prop, converters);

			   return new KeyValuePair<string, object>(propName, propValue);
			})
			.Where(kvp => !excludeNulls || kvp.Value != null)
			.ToDictionary(kvp => kvp.Key, kvp => kvp.Value);

		 return requestParameters;
	  }

	  private object GetPropertyValue<P>(PropertyInfo prop, IList<ITypeConverter<string>> converters)
		 where P : RequestAttribute
	  {
		 var converter = converters?.FirstOrDefault(converter => converter.CanConvert(prop.GetType()));

		 if (converter != null)
		 {
			return converter.ToOutput(prop.GetValue(this));
		 }

		 return prop.GetCustomAttribute<P>().GetValue(prop.GetValue(this));
	  }

	  /// <summary>
	  /// The type converters used to write parameter values.
	  /// </summary>
	  internal List<ITypeConverter<string>> TypeConverters
	  {
		 get
		 {
			var converters = new List<ITypeConverter<string>>();

			if (AcceptDatetimeFormat.HasValue)
			{
			   converters.Add(new AcceptDateTimeToStringConverter(AcceptDatetimeFormat.Value));
			}

			return converters;
		 }
	  }
   }
}
