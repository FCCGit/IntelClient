using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace EVELogClient
{
    public class FileChannel : LogFile
    {
        LogChannel channel = new LogChannel();

        const string ChannelID = "Channel ID:";
        const string ChannelName = "Channel Name:";
        const string Listener = "Listener:";
        const string SessionStarted = "Session started:";

        public FileChannel(string path)
            : base(path)
        {
        }

        public FileChannel(FileInfo file)
            : base(file)
        {
        }

        public LogChannel Channel
        {
            get { return this.channel; }
        }

        private string parseArgument(string type, string line)
        {
            return line.Substring(line.IndexOf(type) + type.Length).Trim();

        }

        public override string[] ReadLastBlock()
        {
            bool newFile = channel.ChannelID == null;

            string[] lines = base.ReadLastBlock();

            if (newFile && lines != null)
            {
                foreach (string line in lines)
                {
                    if (line.Contains(ChannelID))
                    {
                        channel.ChannelID = parseArgument(FileChannel.ChannelID, line);
                    }
                    else if (line.Contains(FileChannel.ChannelName))
                    {
                        channel.ChannelName = parseArgument(FileChannel.ChannelName, line);
                    }
                    else if (line.Contains(FileChannel.Listener))
                    {
                        channel.Listener = parseArgument(FileChannel.Listener, line);
                    }
                    else if (line.Contains(FileChannel.SessionStarted))
                    {
                        string timestamp = parseArgument(FileChannel.SessionStarted, line);
                        DateTime SessionStarted;
                        DateTime.TryParse(timestamp, out SessionStarted);
                        channel.SessionStarted = SessionStarted;
                    }
                }
            }

            return lines;
        }

    }
}
