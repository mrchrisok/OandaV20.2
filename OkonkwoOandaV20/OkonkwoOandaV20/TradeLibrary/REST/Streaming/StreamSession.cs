using Newtonsoft.Json;
using System;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;

namespace OkonkwoOandaV20.TradeLibrary.REST.Streaming
{
   public abstract class StreamSession<T> where T : IStreamResponse
   {
      protected readonly string _accountID;
      protected HttpResponseMessage _response;
      protected bool _shutdown;

      public delegate void DataHandler(T data);
      public event DataHandler DataReceived;
      public void OnDataReceived(T data)
      {
         DataReceived?.Invoke(data);
      }

      public delegate void SessionStatusHandler(string accountID, bool started, Exception e);
      public event SessionStatusHandler SessionStatusChanged;
      public void OnSessionStatusChanged(bool started, Exception e)
      {
         SessionStatusChanged?.Invoke(_accountID, started, e);
      }

      protected StreamSession(string accountID)
      {
         _accountID = accountID;
      }

      protected abstract Task<HttpResponseMessage> GetSession();

      public virtual async Task StartSession()
      {
         _shutdown = false;

         try
         {
            _response = await GetSession();

            using (_response)
            using (var stream = await _response.Content.ReadAsStreamAsync())
            using (var reader = new StreamReader(stream))
            {
               while (!reader.EndOfStream && !_shutdown)
               {
                  string line = reader.ReadLine();
                  var data = JsonConvert.DeserializeObject<T>(line, Rest20.JsonSettingsResponse);

                  OnSessionStatusChanged(!_shutdown, null);

                  OnDataReceived(data);
               }
            }
         }
         catch (Exception e)
         {
            _shutdown = true;
            throw e;
         }
         finally
         {
            _response = null;
         }
      }

      public void StopSession()
      {
         _shutdown = true;
      }

      public bool Stopped()
      {
         return _shutdown;
      }
   }
}
