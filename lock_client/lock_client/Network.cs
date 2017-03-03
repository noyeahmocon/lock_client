using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace lock_client
{
    class Network
    {
        public static void GETLockInfo()
        {
            var request = (HttpWebRequest)WebRequest.Create("Server URL");
        }
    }
}
