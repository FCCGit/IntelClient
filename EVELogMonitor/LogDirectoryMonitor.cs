using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace EVELogMonitor
{

    public class LogDirectoryMonitor
    {
        private FileSystemWatcher watch = new FileSystemWatcher();
        private IDictionary<string, LogFileMonitor> fileSteams = new Dictionary<string, LogFileMonitor>();
        private string channelName;

        public LogDirectoryMonitor()
        {
            watch.NotifyFilter = NotifyFilters.LastWrite | NotifyFilters.FileName;
            watch.Filter = "*.txt";
            watch.Changed += new FileSystemEventHandler(OnChanged);
            watch.Created += new FileSystemEventHandler(OnChanged);
            ReadLogsAferSeconds = 3600;
        }

        public int ReadLogsAferSeconds
        {
            get;
            set;
        }

        public bool EnableRaisingEvents
        {
            get { return watch.EnableRaisingEvents; }
            set { watch.EnableRaisingEvents = value; }
        }

        public ICollection<LogFileMonitor> Channels
        {
            get { return fileSteams.Values; }
        }

        public string Path
        {
            get { return watch.Path; }
            set { watch.Path = value; }
        }

        public string FilenameFilter
        {
            get { return watch.Filter; }
            set { watch.Filter = value; }
        }
        
        public string ChannelName
        {
            get { return channelName; }
            set
            {
                channelName = value;
                watch.Filter = channelName.Replace(' ', '_')
                                          .Replace('\\','_')
                                          .Replace('/', '_')
                                          .Replace('?', '_')
                                          .Replace(':', '_')
                                          .Replace('*', '_')
                                          .Replace('"', '_')
                                          .Replace('>', '_')
                                          .Replace('<', '_')
                                          .Replace('|', '_')
                                          + "*.txt";
            }
        }

        private void OnChanged(object source, FileSystemEventArgs e)
        {
            LogFileMonitor monitor = GetFileItem(e.FullPath);
        }

        private LogFileMonitor GetFileItem(string FullPath)
        {
            FileInfo fileInfo = new FileInfo(FullPath);
            LogFileMonitor monitor;
            
            lock (fileSteams)
            {
                if (fileSteams.ContainsKey(FullPath))
                {
                    monitor = fileSteams[FullPath];
                }
                else
                {
                    fileSteams[FullPath] = new LogFileMonitor(fileInfo);
                }

                return fileSteams[FullPath];
            }
        }

        public List<LogFileMonitor> ReadDirectory()
        {
            List<LogFileMonitor> monitors = new List<LogFileMonitor>();

            DirectoryInfo dir = new DirectoryInfo(Path);
            if (dir.Exists)
            {
                FileInfo[] files = dir.GetFiles(watch.Filter);

                files = files.OrderByDescending(x => x.LastWriteTime).ToArray<FileInfo>();

                foreach (FileInfo file in files)
                {
                    file.Refresh();
                    // only read log files that have been updated in the last 2 hours
                    if (file.LastWriteTime >= DateTime.Now.AddSeconds(-ReadLogsAferSeconds))
                    {
                        monitors.Add(GetFileItem(file.FullName));
                    }
                }
            }

            return monitors;
        }
    }
}
