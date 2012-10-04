using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace EVELogClient
{
    public class LogChannel
    {
        public string ChannelID { get; set; }
        public string ChannelName { get; set; }
        public string Listener { get; set; }
        public DateTime SessionStarted { get; set; }

        public override string ToString()
        {
            return ChannelName;
        }
    }
}
