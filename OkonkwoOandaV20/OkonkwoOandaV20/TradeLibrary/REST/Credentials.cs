using System.Collections.Generic;

namespace OkonkwoOandaV20.TradeLibrary.REST
{
   public enum Server
   {
      Account,
      Labs,
      PricingStream,
      TransactionsStream
   }

   public enum Environment
   {
      Practice,
      Trade
   }

   public class Credentials
   {
      public bool HasServer(Server server)
      {
         return Servers[Environment].ContainsKey(server);
      }

      public string GetServer(Server server)
      {
         if (HasServer(server))
         {
            return Servers[Environment][server];
         }
         return null;
      }

      private static readonly Dictionary<Environment, Dictionary<Server, string>> Servers = new Dictionary<Environment, Dictionary<Server, string>>
      {
         {  Environment.Practice, new Dictionary<Server, string>
            {
               {Server.Account, "https://api-fxpractice.oanda.com/v3/"},
               {Server.Labs, "https://api-fxpractice.oanda.com/labs/v3/"},
               {Server.PricingStream, "https://stream-fxpractice.oanda.com/v3/"},
               {Server.TransactionsStream, "https://stream-fxpractice.oanda.com/v3/"},
            }
         },
         {  Environment.Trade, new Dictionary<Server, string>
            {
               {Server.Account, "https://api-fxtrade.oanda.com/v3/"},
               {Server.Labs, "https://api-fxtrade.oanda.com/labs/v3/"},
               {Server.PricingStream, "https://stream-fxtrade.oanda.com/v3/"},
               {Server.TransactionsStream, "https://stream-fxtrade.oanda.com/v3/"}
            }
         }
      };

      private static Credentials m_DefaultCredentials;

      public string AccessToken { get; set; }
      public string DefaultAccountId { get; set; }
      public Environment Environment { get; set; }

      public static Credentials GetDefaultCredentials()
      {
         return m_DefaultCredentials;
      }

      public static void SetCredentials(Environment environment, string accessToken, string defaultAccount = "0")
      {
         m_DefaultCredentials = new Credentials
         {
            Environment = environment,
            AccessToken = accessToken,
            DefaultAccountId = defaultAccount
         };
      }
   }
}
