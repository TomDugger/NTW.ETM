using NTW.Communication.Service;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;

namespace NTW.Communication.Beginers
{
    public static class ClientBeginer
    {
        public static void Send(string ip, int port, string serviceName, int typeCommand, int index, int[] parametry, int timeout = 20000)
        {
            if (ip != string.Empty)
            {
                var con = new CommQueryStringConverter();

                System.Threading.Tasks.Task t = new System.Threading.Tasks.Task(() => {
                    HttpWebRequest http = (HttpWebRequest)HttpWebRequest.Create(string.Format("http://{0}:{1}/{2}/com?t={3}&i={4}&ai={5}", ip, port, serviceName, typeCommand, index, con.ConvertValueToString(parametry, typeof(int[]))));
                    http.Timeout = timeout;
                    try
                    {
                        http.GetResponse();
                    }
                    catch {
                    }
                });
                t.Start();
            }
        }
    }
}
