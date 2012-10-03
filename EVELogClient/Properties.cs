using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EVELogClient
{
    static class Properties
    {
        //expiration of messages in intel channels
        public static readonly int EXPIRY = 60;
        //time to sleep between refreshes of logs
        public static readonly int SLEEP = 15;
        //config file path
        public static readonly string CONFIG_FILE = "config.ini";
        public static readonly string HTTP_URL = "https://www.fatal-ascension.com/eve_intel/post_data.php";
        //jabber info stored internally to obfuscate the password data
        public static readonly string JABBER_URL = "jabber.fatal-ascension.com";
        public static readonly int JABBER_PORT = 5222;
        public static readonly string JABBER_USER = "ryshar";
        public static readonly string JABBER_PASS = "******";
        public static readonly string JABBER_TARGET = "ryshar@jabber.fatal-ascension.com/Home";


        static readonly Dictionary<string, string> props = new Dictionary<string, string>();
        static Properties()
        {
            try
            {
                //try loading the config file
                foreach (var row in File.ReadAllLines(CONFIG_FILE))
                {
                    string[] arr = row.Split('=');
                    props.Add(arr[0], arr[1]);
                }
            }
            catch
            {
                Console.WriteLine("No config.ini file found, using defaults");
                //use defaults if no config file found
                props["LOG_DIR"] = Environment.GetEnvironmentVariable("HOMEDRIVE") + Environment.GetEnvironmentVariable("HOMEPATH") + "\\Documents\\EVE\\logs\\Chatlogs";
                props["JABBER_TIMEOUT"] = "10";
                props["CHANNELS"] = "CR.Intel,PB_F_INTEL";
            }
        }

        public static string getProperty(string prop)
        {
            return props[prop];
        }
    }
}
