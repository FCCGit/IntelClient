using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace LogMonitor
{
    public class LogFile
    {
        private FileInfo file;
        private long lastKnownSize;
        private long lastReadTime;

        private long maxBlockReadSize;

        public LogFile(FileInfo file)
        {
            this.file = file;
            this.lastKnownSize = 0;
            this.lastReadTime = file.LastWriteTime.Ticks;
            
            this.maxBlockReadSize = -1;
        }

        public LogFile(string path)
        {
            this.file = new FileInfo(path);
            this.lastKnownSize = 0;
            this.lastReadTime = file.LastWriteTime.Ticks;

            this.maxBlockReadSize = -1;
        }

        public FileInfo File
        {
            get { return file; }
        }

        public string Path
        {
            get { return file.FullName; }
        }

        public long LastReadTime
        {
            get { return this.lastReadTime; }
        }

        public long MaxBlockReamSize
        {
            get { return maxBlockReadSize; }
            set { this.maxBlockReadSize = value; }
        }

        public void ResetReadPoint()
        {
            this.lastKnownSize = 0;
            this.lastReadTime = 0;
        }

        public override string ToString()
        {
            return file.Name;
        }

        /// <summary>
        /// Read the next block of text from the file. This is done so a we would only read the contact that changed since the last update.
        /// </summary>
        /// <returns></returns>
        public virtual string[] ReadLastBlock()
        {
            // make sure the file information is up to date (so that last write time is correct).
            file.Refresh();

            using (FileStream fs = file.Open(FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
            {
                if (!fs.CanSeek || lastKnownSize >= fs.Length)
                {
                    // file is already read or seek is not supported
                    return null;
                }

                long bytesToRead = fs.Length - lastKnownSize;

                byte[] array = new byte[bytesToRead];

                fs.Position = fs.Length - bytesToRead;

                // cap maximum byes to read
                if (maxBlockReadSize > -1 && bytesToRead > maxBlockReadSize)
                {
                    bytesToRead = maxBlockReadSize;
                }

                fs.Read(array, 0, (int)bytesToRead);

                lastKnownSize += bytesToRead;
                lastReadTime = file.LastWriteTime.Ticks;

                Encoding enc = new UnicodeEncoding();
                string block = enc.GetString(array);

                return block.Split(System.Environment.NewLine.ToCharArray(), StringSplitOptions.RemoveEmptyEntries);
            }
        }

    }
}
