using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace EVELogMonitor
{
    public delegate void NotifyLogChannel(LogChannel channel);
    public delegate void NotifyLogMessage(LogMessage message);

    public class LogFileMonitor
    {
        FileChannel fileChannel;
        private FileSystemWatcher watch = new FileSystemWatcher();

        public event NotifyLogChannel ChangedLogChannel;
        public event NotifyLogMessage ChangedLogMessage;
        
        public LogFileMonitor(FileInfo file)
        {
            this.fileChannel = new FileChannel(file);
            watch.NotifyFilter = NotifyFilters.LastWrite | NotifyFilters.FileName;
            watch.Filter = file.Name;
            watch.Changed += new FileSystemEventHandler(OnChanged);
            watch.Created += new FileSystemEventHandler(OnChanged);
        }

        public override string ToString()
        {
            if (fileChannel.Channel.ChannelName != null)
            {
                return fileChannel.Channel.ChannelName + " (" + fileChannel.Channel.Listener + ") " + fileChannel.Channel.SessionStarted.ToShortTimeString();
            }
            else
            {
                return "[" + fileChannel.File.Name + "]";
            }
        }

        public FileChannel FileChannel
        {
            get { return fileChannel; }
        }

        public bool EnableRaisingEvents
        {
            get { return watch.EnableRaisingEvents; }
            set { watch.EnableRaisingEvents = value; }
        }

        public void Refresh(bool reset)
        {
            if (reset)
            {
                fileChannel.ResetReadPoint();
            }

            // if the file has been written since it has been read
            fileChannel.File.Refresh();
            if (fileChannel.File.LastWriteTime.Ticks > fileChannel.LastReadTime)
            {
                // force update the file
                updateFileData(fileChannel);
            }
        }

        private void OnChanged(object source, FileSystemEventArgs e)
        {
            // Specify what is done when a file is changed, created, or deleted.
            updateFileData(fileChannel);
        }

        private void updateFileData(FileChannel item)
        {
            bool isNew = (item.Channel.ChannelID == null);

            string[] lines = item.ReadLastBlock();

            if (isNew && item.Channel.ChannelID != null)
            {
                if (ChangedLogChannel != null)
                {
                    ChangedLogChannel(item.Channel);
                }
            }

            if (lines != null)
            {
                foreach (string line in lines)
                {
                    // ((FileChannel)item).LogCannel
                    if (LogMessage.isValidMessage(line))
                    {
                        LogMessage message = new LogMessage(item.Channel, line);
                        if (ChangedLogMessage != null)
                        {
                           
                            ChangedLogMessage(message);
                        }
                    }
                }
            }
        }
    }
}
