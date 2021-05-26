namespace OkonkwoOandaV20.Framework.TypeConverters
{
   public interface ITypeConverter<TOutput>
   {
	  TOutput ToOutput(object input);
   }
}
