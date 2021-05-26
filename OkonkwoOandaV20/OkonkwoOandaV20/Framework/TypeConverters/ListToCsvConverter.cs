using System;
using System.Collections;

namespace OkonkwoOandaV20.Framework.TypeConverters
{
   public class ListToCsvConverter : ITypeConverter<string>
   {
	  public ListToCsvConverter() : this(",")
	  {
	  }

	  public ListToCsvConverter(string delimiter)
	  {
		 _delimeter = delimiter;
	  }

	  private string _delimeter;

	  public string ToOutput(object input)
	  {
		 if (input == null)
		 {
			return null;
		 }

		 if (input is IList list)
		 {
			var output = Utilities.ConvertListToDelimitedValues(list, _delimeter);

			return output;
		 }

		 throw new ArgumentException($"The input type '{input.GetType().Name}' is invalid.");
	  }
   }
}
