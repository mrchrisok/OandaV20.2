using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace OkonkwoOandaV20.Framework
{
   /// <summary>
   /// Validates objects using context provided by its DataAnnotation attributes
   /// </summary>
   public class SimpleObjectValidator
   {
      /// <summary>
      /// Validates an object using context provided by its DataAnnotation attributes
      /// </summary>
      /// <param name="instance">The object to validate</param>
      public static void Validate(object instance)
      {
         var validationResults = new List<ValidationResult>();
         var validationContext = new ValidationContext(instance);

         try
         {
            Validator.ValidateObject(instance, validationContext, validateAllProperties: true);
         }
         catch (ValidationException)
         {
            throw;

            // If you want all errors instead of just the first one:
            //validationResults.Clear();
            //bool isValid = Validator.TryValidateObject(instance, validationContext, validationResults, true);

            //if (!isValid)
            //{
            //   Console.WriteLine("All validation errors:");
            //   foreach (var result in validationResults)
            //   {
            //      Console.WriteLine($" - {result.ErrorMessage}");
            //   }
            //}
         }
      }
   }
}