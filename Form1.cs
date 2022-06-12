using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Fivem_Console_Manager
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }
        private const int STD_OUTPUT_HANDLE = -11;
        private const uint ENABLE_VIRTUAL_TERMINAL_PROCESSING = 0x0004;
        private const uint DISABLE_NEWLINE_AUTO_RETURN = 0x0008;

        [DllImport("kernel32.dll")]
        private static extern bool GetConsoleMode(IntPtr hConsoleHandle, out uint lpMode);

        [DllImport("kernel32.dll")]
        private static extern bool SetConsoleMode(IntPtr hConsoleHandle, uint dwMode);

        [DllImport("kernel32.dll", SetLastError = true)]
        private static extern IntPtr GetStdHandle(int nStdHandle);

        [DllImport("kernel32.dll")]
        public static extern uint GetLastError();

        public class Win32
        {
            /// <summary>
            /// Allocates a new console for current process.
            /// </summary>
            [DllImport("kernel32.dll")]
            public static extern Boolean AllocConsole();

            /// <summary>
            /// Frees the console.
            /// </summary>
            [DllImport("kernel32.dll")]
            public static extern Boolean FreeConsole();
        }


        public void ExtractFile(string sourceArchive, string destination)
        {
            SevenZipNET.SevenZipExtractor zz = new SevenZipNET.SevenZipExtractor(sourceArchive);
            zz.ExtractAll(destination);
        }
        private void button1_Click(object sender, EventArgs e)
        {
            button1.Enabled = false;
            label1.Text = "Installing please wait....";
            using (var client = new WebClient())
            {
                client.DownloadFileCompleted += Client_DownloadFileCompleted;
                Uri uri = new Uri("https://runtime.fivem.net/artifacts/fivem/build_server_windows/master/5562-25984c7003de26d4a222e897a782bb1f22bebedd/server.7z");
                client.DownloadFileAsync(uri, "artifacts.7z");
            }
        }

        private void Client_DownloadFileCompleted(object sender, AsyncCompletedEventArgs e)
        {
            if (e.Cancelled == true)
            {

            }
            else
            {
                Directory.CreateDirectory(Application.StartupPath + @"/artifacts/");
                Thread.Sleep(1000);
                ExtractFile(Application.StartupPath + @"/artifacts.7z", Application.StartupPath + @"/artifacts/");
                label1.Text = "Completed!";
            }
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            if(File.Exists(Application.StartupPath + @"/artifacts/FXServer.exe"))
            {
                button1.Enabled = false;
            }
        }
        Process p = new Process();
        private void button2_Click(object sender, EventArgs e)
        {
            Invoke((MethodInvoker)delegate
            {
                Task.Factory.StartNew(() =>
                {
                    p.StartInfo.CreateNoWindow = true;
                    p.StartInfo.WindowStyle = ProcessWindowStyle.Hidden;
                    p.StartInfo.WorkingDirectory = Application.StartupPath + @"/artifacts/";
                    p.StartInfo.FileName = Application.StartupPath + @"/artifacts/FXServer.exe";
                    p.StartInfo.Arguments = $"+set serverProfile {profile.Text} +set txAdminPort {port.Text}";
                    p.StartInfo.UseShellExecute = false;
                    p.StartInfo.RedirectStandardInput = true;
                    p.StartInfo.RedirectStandardOutput = true;
                    p.StartInfo.RedirectStandardError = true;
                    p.Start();
                while (!p.StandardOutput.EndOfStream)
                {
                    var line = p.StandardOutput.ReadLine();
                    richTextBox1.Invoke((MethodInvoker)delegate
                    {
                        richTextBox1.AppendText(line + "\n");
                    });
                    
                        
                }
                });
            });
        }

        private void button3_Click(object sender, EventArgs e)
        {
            p.Kill();
        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            //Win32.FreeConsole();
        }
    }
}
