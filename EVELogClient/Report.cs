using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Security.Cryptography.X509Certificates;
using agsXMPP;
using agsXMPP.protocol.client;
using agsXMPP.Xml.Dom;

namespace EVELogClient
{
    class Report
    {
        private static XmppClientConnection xmppClient;

        static Report()
        {
            init();
        }

        private static void init()
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


        
        public static void reportViaXMPP(string message)
        {
            if (xmppClient.XmppConnectionState != XmppConnectionState.SessionStarted)
            {
                Console.WriteLine("Lost Jabber connection, trying to reestablish...");
            }
            xmppClient.Send(new Message("ryshar@jabber.fatal-ascension.com/Home", MessageType.chat, message));
        }
    }
}
