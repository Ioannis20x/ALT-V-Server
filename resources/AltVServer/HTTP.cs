using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace AltVServer
{
    class HTTP
    {
        public static byte[] Post (string url, NameValueCollection pairs)
        {
            using (WebClient webclient = new WebClient())
            {
                return webclient.UploadValues(url, pairs);
            }
        }
    }
}
