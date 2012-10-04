using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using EVELogClient;

namespace EVELogClient
{
    static class Program
    {
        private static Thread logThread;
        private static EVEIntel window;

        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            window = new EVEIntel();
            new Thread(new ThreadStart(startWindow)).Start();
            initMonitor();
            runMonitor();

            while (true)
            {
                Thread.Yield();
                //Thread.Sleep(5);
            }
        }

        static void startWindow()
        {
            Application.Run(window);
            Environment.Exit(0);
        }

        public static void initMonitor()
        {
            IntelMonitor.onStatusChange += window.updateStatus;
            IntelMonitor.onError += handleError;

            IntelProperties.onConfigCreate += openSettings;
            IntelProperties.init();
        }

        public static void runMonitor()
        {
            if (logThread == null)
            {
                logThread = new Thread(new ThreadStart(new IntelMonitor().run));
                logThread.Start();
            }
        }

        public static void stopMonitor()
        {
            logThread.Abort();
            logThread.Join();
            logThread = null;
            Console.WriteLine("murder");
        }

        static void handleError(string err)
        {
            ShowError s = window.showError;
            window.Invoke(s, err);
            Environment.Exit(1);
        }

        static void openSettings()
        {
            ShowSettings s = window.showSettings;
            window.Invoke(s);
            Console.WriteLine("BOOM");
        }

    }
}
