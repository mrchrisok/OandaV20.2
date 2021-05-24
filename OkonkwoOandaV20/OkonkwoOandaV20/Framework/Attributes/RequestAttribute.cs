using System;

namespace OkonkwoOandaV20.Framework
{
   public abstract class RequestAttribute : Attribute
   {
	  protected RequestAttribute(string name = null)
	  {
		 Name = name ?? Name;
	  }

	  public string Name { get; set; }
   }
}
