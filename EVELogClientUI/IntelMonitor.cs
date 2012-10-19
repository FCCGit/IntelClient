using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace EVELogClient
{
    public delegate void OnStatusChange(string status);
    public delegate void OnError(string status);

    //Main execution class
    public class IntelMonitor
    {
        public static event OnStatusChange onStatusChange;
        public static event OnError onError;

        Dictionary<string, Monitor> channels = new Dictionary<string, Monitor>();
        List<string> monitoredChannels = new List<string>();

        public void run()
        {
            try
            {
                onStatusChange("Adding channels");
                foreach (string channel in IntelProperties.getProperty("CHANNELS").Split(','))
                {
                    Console.WriteLine("\t"+channel);
                    monitoredChannels.Add(channel.Trim());
                }
                onStatusChange("Initializing channels");
                refreshChannels();

                onStatusChange("Monitoring channels");

                int fullRefreshCount = 0;
                while (true)
                {
                    //Console.WriteLine("Sleeping");
                    Thread.Sleep(IntelProperties.SLEEP * 1000);
                    readChannels();
                    if (fullRefreshCount++ == 5)
                    {
                        //Console.WriteLine("Full refresh");
                        refreshChannels();
                        fullRefreshCount = 0;
                    }
                }
            }
            catch (ThreadAbortException e)
            {
                //return, we've been killed
                return;
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                logError(e);
            }
        }

        private void initChannels()
        {
            LogDirectoryMonitor l = new LogDirectoryMonitor(IntelProperties.EXPIRY);
            l.Path = IntelProperties.getProperty("LOG_DIR");

            List<LogFileMonitor> monitors = l.ReadDirectory();
            foreach (LogFileMonitor m in monitors)
            {
                m.Refresh(true);
                string name = m.FileChannel.Channel.ChannelName;
                if (monitoredChannels.Contains(name))
                {
                    Console.WriteLine("Monitoring "+name);
                    channels.Add(name, new Monitor(m));
                }
            }
        }

        //check to see if theres a more up-to-date version of any channel
        private void refreshChannels()
        {
            LogDirectoryMonitor l = new LogDirectoryMonitor(IntelProperties.EXPIRY);
            l.Path = IntelProperties.getProperty("LOG_DIR");

            List<LogFileMonitor> monitors = l.ReadDirectory();
            foreach (LogFileMonitor m in monitors)
            {
                m.Refresh(true);
                string name = m.FileChannel.Channel.ChannelName;

                //if this channel is already being monitored, check to make sure the files are the same
                if (monitoredChannels.Contains(name) && channels.ContainsKey(name))
                {
                    Monitor om = channels[name];
                    if (om.logMonitor.FileChannel.File.Name != m.FileChannel.File.Name && om.logMonitor.FileChannel.File.LastWriteTime < m.FileChannel.File.LastWriteTime)
                    {
                        Console.WriteLine("Updating file "+m.FileChannel.File.Name);
                        channels.Remove(name);
                        channels.Add(name, new Monitor(m));
                    }
                }
                else if (monitoredChannels.Contains(name) && !channels.ContainsKey(name))
                {
                    //else, just add a new monitor
                    Console.WriteLine("Monitoring " + name);
                    channels.Add(name, new Monitor(m));
                }
            }
        }

        private void readChannels()
        {
            foreach (Monitor m in channels.Values)
            {
                m.read();
            }
        }

        private void logError(Exception e)
        {
            string text = DateTime.Now+"\n\n";
            text += e.Message + "\n";
            text += e.StackTrace+"\n";
            text += "---------------------------------------------------------------------------------------------------------------\n";
            System.IO.File.AppendAllText(IntelProperties.ERROR_LOG, text);
            onError("Some bad shit happened. Error logged to " + IntelProperties.ERROR_LOG + ", exiting now...");
        }

    }

    //reads log files and sends messagesto the Jabber server
    class Monitor
    {
        public readonly LogFileMonitor logMonitor;
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
            TimeSpan diff = getDiff(DateTime.UtcNow, message.Timestamp);
            if (diff.TotalMinutes > IntelProperties.EXPIRY)
            {
                return;
            }

            Console.WriteLine(message);            
            Report.reportViaHTTP(channel+": "+message.Name+"["+message.Timestamp.ToString()+"]: "+message.Message);
        }

        private TimeSpan getDiff(DateTime local, DateTime eve)
        {
            DateTime newEve = TimeZoneInfo.ConvertTimeBySystemTimeZoneId(eve, "Greenwich Standard Time", TimeZoneInfo.Local.Id);
            TimeSpan diff = local - eve;

            //Console.WriteLine(local.ToString() +"-"+ eve.ToString() + " = " + diff);
            return diff;
        }
    }

}
