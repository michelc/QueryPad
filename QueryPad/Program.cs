using System;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace QueryPad
{
    public static class Program
    {
        [DllImport("USER32.DLL")]
        public static extern bool SetForegroundWindow(IntPtr hWnd);

        [DllImport("user32.dll")]
        private static extern Boolean ShowWindow(IntPtr hWnd, Int32 nCmdShow);
        private const int SW_SHOWNORMAL = 1;

        [STAThread]
        public static void Main()
        {
            // What is the correct way to create a single instance application?
            // http://stackoverflow.com/questions/19147/what-is-the-correct-way-to-create-a-single-instance-application
            // (although I chose the shortest and good enough)

            // Check if app is already running
            Process currentProcess = Process.GetCurrentProcess();
            var runningProcess = (from process in Process.GetProcesses()
                                  where process.Id != currentProcess.Id
                                     && process.ProcessName.Equals(currentProcess.ProcessName, StringComparison.Ordinal)
                                  select process).FirstOrDefault();

            if (runningProcess != null)
            {
                // Activate already running app
                ShowWindow(runningProcess.MainWindowHandle, SW_SHOWNORMAL);
                SetForegroundWindow(runningProcess.MainWindowHandle);
                return;
            }

            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new MainForm());
        }
    }
}
