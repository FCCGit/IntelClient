using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace EVELogClient
{
    /// <summary>
    /// sample message: [ 2011.08.16 03:29:06 ] My User > test 123
    /// </summary>
    public class LogMessage
    {
        private LogChannel channel;
        private DateTime timestamp;
        private string name;
        private string message;
        private static Regex rgx = new Regex("\\[ [0-9]{4}\\.[0-9]{2}\\.[0-9]{2} [0-9]{2}:[0-9]{2}:[0-9]{2} \\] [^\\>]* \\> [^\\n]*");

        public LogChannel Channel { get { return channel; } }
        public DateTime Timestamp { get { return timestamp; } }
        public string Name { get { return name; } }
        public string Message { get { return message; } }

        public static bool isValidMessage(string line)
        {
            return rgx.Matches(line).Count > 0;
        }

        public LogMessage()
        {
        }

        public LogMessage(LogChannel channel, string line)
        {
            this.Parse(channel, line);
        }

        public void Parse(LogChannel channel, string line)
        {
            if (isValidMessage(line))
            {
                int startDate = line.IndexOf("[ ", 0);
                int endDate = line.IndexOf(" ]", startDate + 2);
                int endName = line.IndexOf(" > ", endDate + 3);

                this.channel = channel;
                string timePart = line.Substring(startDate + 2, endDate - 2).Trim();
                bool valid = DateTime.TryParse(timePart, out this.timestamp);
                this.name = line.Substring(endDate + 2, endName - endDate - 2).Trim();
                this.message = line.Substring(endName + 2).TrimStart();
            }
        }

        public override string ToString()
        {
            string channelName = "";
            if (channel != null)
            {
                channelName = channel.ChannelName + ": ";
            }
            return channelName + "[ " + timestamp.ToLongTimeString() + " ] " + name + " > " + message;
        }
    }

}
