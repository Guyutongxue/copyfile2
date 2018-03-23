using System;
using System.Windows;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using System.Windows.Forms;

namespace copyfile2
{
    class Program
    {
        bool isExit = false;
        NotifyIconHelper _notifyIconHelper; 

        static void Main(string[] args)
        {
            Program _program = new Program();
            Console.Title = "copyfile2";
            _program._notifyIconHelper= new NotifyIconHelper(@"F:\Cpp\实用工具\Copy2WSL\bash.ico", Console.Title);
            _program._notifyIconHelper.ShowNotifyIcon();
            _program._notifyIconHelper.ShowBallon("copyfile2", "copyfile2 is running in background.", 1000);

            Thread threadMonitorInput = new Thread(new ThreadStart(_program.MonitorInput));
            threadMonitorInput.Start();
            while (true)
            {
                Application.DoEvents();
                if (_program.isExit)
                {
                    threadMonitorInput.Abort();
                    break;
                }
            }
        }
        void MonitorInput()
        {
            while (true)
            {
                string input = Console.ReadLine();
                switch (input)
                {
                    case "exit":
                        {
                            isExit = true;
                            break;
                        }
                    case "hide":
                        {
                            _notifyIconHelper.Hide();
                            break;
                        }

                }
            }
        }
    }
}
