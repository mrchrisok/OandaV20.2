using System;

namespace OkonkwoOandaV20.Framework
{
   /// <summary>
   /// Indicates that a property is the only one to serialize into the body of a request.
   /// </summary>
   [AttributeUsage(AttributeTargets.Property | AttributeTargets.Method | AttributeTargets.Field, Inherited = false)]
   internal sealed class BodyAttribute : Attribute
   {
   }

   public class IsOptionalAttribute : Attribute
   {
      public override string ToString()
      {
         return "Is Optional";
      }
   }
}
