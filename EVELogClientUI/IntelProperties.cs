using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.CompilerServices;

namespace EVELogClient
{
    public delegate void OnConfigCreate();

    public static class IntelProperties
    {
        public static event OnConfigCreate onConfigCreate;

        //expiration of messages in intel channels
        public static readonly int EXPIRY = 60;
        //time to sleep between refreshes of logs
        public static readonly int SLEEP = 15;
        //config file path
        public static readonly string CONFIG_FILE = "config.ini";
        //error log path
        public static readonly string ERROR_LOG = "error.log";
        //HTTP service URL
        public static readonly string HTTP_URL = "https://www.fatal-ascension.com/eve_intel/post_data.php";

        static readonly Dictionary<string, string> props = new Dictionary<string, string>();
        
        public static void init()
        {

            try
            {
                //try loading the config file
                foreach (var row in File.ReadAllLines(CONFIG_FILE))
                {
                    string[] arr = row.Split('=');
                    props.Add(arr[0], arr[1]);
                }
                if (!validateProperties())
                {
                    Console.WriteLine("Config item missing");
                    onConfigCreate();
                }
            }
            catch
            {
                Console.WriteLine("No config.ini file found, using defaults");
                props["LOG_DIR"] = Environment.GetEnvironmentVariable("HOMEDRIVE") + Environment.GetEnvironmentVariable("HOMEPATH") + "\\Documents\\EVE\\logs\\Chatlogs";
                props["CHANNELS"] = "CR.Intel,PB_F_INTEL,DK_Surveillance,Fountaindef,honeybadger intel,synd.int,VNL_INT,brantel,EC_Gate,The Clusterfuck";
                onConfigCreate();
                save();
            }
        }

        public static Boolean validateProperties()
        {
            if (!keyHasValue("USER_ID") || !keyHasValue("USER_KEY") || !keyHasValue("CHANNELS") || !keyHasValue("LOG_DIR"))
            {
                return false;
            }
            return true;
        }

        private static Boolean keyHasValue(string key)
        {
            return props.ContainsKey(key) && props[key] != null && props[key] != "";
        }

        public static void setProperty(string prop, string val)
        {
            props[prop] = val;
        }

        public static string getProperty(string prop)
        {
            if (props.ContainsKey(prop))
            {
                return props[prop];
            }
            return "";
        }

        [MethodImpl(MethodImplOptions.Synchronized)]
        public static void save()
        {
            string conf = "";
            foreach(KeyValuePair<String,String> entry in props)
            {
                conf += entry.Key + "=" + entry.Value + "\n";
            }
            System.IO.File.WriteAllText(CONFIG_FILE, conf);
        }
    }
}
