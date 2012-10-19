using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Net;
using System.Collections.Specialized;

namespace EVELogClient
{
    class Report
    {
        public static void reportViaHTTP(string message)
        {
            WebClient webClient = new WebClient();
            webClient.Proxy = null;
            NameValueCollection values = new NameValueCollection();
            values.Add("user", IntelProperties.getProperty("USER_ID"));
            values.Add("userkey", IntelProperties.getProperty("USER_KEY"));
            values.Add("message", message);

            webClient.UploadValues(IntelProperties.HTTP_URL, "POST", values);
            webClient.Dispose();
        }
    }
}
