using Azure.Core;
using Azure.Identity;

namespace OkonkwoOandaV20Tests
{
   internal static class Utilities
   {
      internal static TokenCredential GetAzureCredential(bool isLocalEnvironment = true)
      {
         var credentialOptions = new DefaultAzureCredentialOptions
         {
            ExcludeManagedIdentityCredential = isLocalEnvironment
         };
         return new DefaultAzureCredential(credentialOptions);
      }
   }
}
