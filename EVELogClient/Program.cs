using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using EVELogMonitor;

namespace EVELogClient
{

    //Main execution class
    class Program
    {
        static Dictionary<string, Monitor> channels = new Dictionary<string, Monitor>();
        static List<string> monitoredChannels = new List<string>();

        static void Main(string[] args)
        {
            foreach (string channel in Properties.getProperty("CHANNELS").Split(','))
            {
                monitoredChannels.Add(channel.Trim());
            }
            initChannels();

            while (true)
            {
                //Console.WriteLine("Sleeping " + Properties.SLEEP+"s");
                Thread.Sleep(Properties.SLEEP *1000);
                readChannels();
            }
        }

        public static void initChannels()
        {
            LogDirectoryMonitor l = new LogDirectoryMonitor();
            l.Path = Properties.getProperty("LOG_DIR");

            List<LogFileMonitor> monitors = l.ReadDirectory();
            foreach (LogFileMonitor m in monitors)
            {
                m.Refresh(true);
                string name = m.FileChannel.Channel.ChannelName;
                if (monitoredChannels.Contains(name))
                {
                    Console.WriteLine("Monitoring "+name);
                    channels.Remove(name);
                    channels.Add(name, new Monitor(m));
                }
            }
        }

        public static void readChannels()
        {
            foreach (Monitor m in channels.Values)
            {
                m.read();
            }
        }

    }

    //reads log files and sends messagesto the Jabber server
    class Monitor
    {
        private LogFileMonitor logMonitor;
        private string channel;

        public Monitor(LogFileMonitor l)
        {
            logMonitor = l;
            logMonitor.ChangedLogMessage += this.parse;
            channel = l.FileChannel.Channel.ChannelName;
            logMonitor.Refresh(true);
        }

        public void read()
        {
            logMonitor.Refresh(false);
        }

        private void parse(LogMessage message)
        {
            //ignore the MOTD
            if (message.Message.StartsWith("Channel MOTD"))
            {
                return;
            }

            //check the timestamp, make sure this is relevant
            TimeSpan diff = DateTime.Now - TimeZoneInfo.ConvertTimeBySystemTimeZoneId(message.Timestamp, "Greenwich Standard Time", TimeZoneInfo.Local.Id);
            if (diff.TotalMinutes > Properties.EXPIRY)
            {
                return;
            }
            
            Report.reportViaHTTP(channel+": "+message.Name+"["+message.Timestamp.ToString()+"]: "+message.Message);
        }
    }

}
