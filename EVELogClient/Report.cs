using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Net;
using System.Collections.Specialized;
using agsXMPP;
using agsXMPP.protocol.client;
using agsXMPP.Xml.Dom;

namespace EVELogClient
{
    class Report
    {
        private static XmppClientConnection xmppClient;

        private static void initXMPP()
        {
            xmppClient = new XmppClientConnection(Properties.JABBER_URL, Properties.JABBER_PORT);
            xmppClient.Open(Properties.JABBER_USER, Properties.JABBER_PASS);

            //wait for connection to complete
            int i = 0;
            while (xmppClient.XmppConnectionState != XmppConnectionState.SessionStarted && i < Int32.Parse(Properties.getProperty("JABBER_TIMEOUT")))
            {
                Thread.Sleep(1000);
                i++;
            }

            if (xmppClient.XmppConnectionState != XmppConnectionState.SessionStarted)
            {
                Console.WriteLine("Could not establish connection to Jabber server, exiting in three seconds.");
                Thread.Sleep(3000);
                Environment.Exit(1);
            }
        }

        public static void reportViaHTTP(string message)
        {
            WebClient client = new WebClient();
            client.Proxy = null;
            NameValueCollection values = new NameValueCollection();
            values.Add("user", Properties.getProperty("USER_ID"));
            values.Add("userkey", Properties.getProperty("USER_KEY"));
            values.Add("message", message);
            client.UploadValues(Properties.HTTP_URL, "POST", values);
            client.Dispose();
        }


        
        public static void reportViaXMPP(string message)
        {
            if (xmppClient == null)
            {
                initXMPP();
            }
            else if (xmppClient.XmppConnectionState != XmppConnectionState.SessionStarted)
            {
                Console.WriteLine("Lost Jabber connection, trying to reestablish...");
                initXMPP();
            }
            xmppClient.Send(new Message(Properties.JABBER_TARGET, MessageType.chat, message));
        }
    }
}
