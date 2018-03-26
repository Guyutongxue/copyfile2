/** App Info
 * copyfile 2.0.0 by Guyutongxue
 * Released under MIT License.
 * Copyright © Guyutongxue 2018
**/

using System;
using System.Windows;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Runtime.InteropServices;
using System.Threading;
using System.Windows.Forms;
using System.Management;

namespace copyfile2
{
    class CFProgram
    {
        bool isExit = false;
        bool isWatching = false;

        NotifyIconHelper notifyIconHelper;
        public const string title = "copyfile2";
        const string info = "copyfile 2.0.0 by Guyutongxue\nReleased under MIT License.\n";

        Thread thrdInput;
        Thread thrdWatch;

        [STAThread]
        static void Main(string[] args)
        {
            CFProgram program = new CFProgram();
        }

        CFProgram()
        {
            //Force Quit EventHandler
            SetConsoleCtrlHandler(CancelHandler, true);

            Console.Title = title;
            notifyIconHelper = new NotifyIconHelper(Properties.Resources.AppIcon, title);
            notifyIconHelper.ShowNotifyIcon();

            Console.Write(info);
            thrdInput = new Thread(new ThreadStart(GetInput));

            thrdInput.Start();

            while (true)
            {
                Thread.Sleep(100); //or the CPU will boom
                if (isExit)
                {
                    break;
                }
                Application.DoEvents();
            }
        }



        private void GetInput()
        {
            while (!isExit)
            {
                try
                {
                    Thread.Sleep(100);
                    Console.Write("> ");
                    string input = Console.ReadLine();
                    string cmd = input.Split(' ')[0];
                    switch (cmd)
                    {
                        case "help":
                            {
                                Console.WriteLine(
@"copyfile 2.0.0 by Guyutongxue
Released under MIT License.

start: start watch USB Device insert event.
stop: stop watch event.
hide: hide the console window.
exit: exit programm.
help: show this message.");
                                break;
                            }


                        case "exit":
                            {
                                Exit();
                                break;
                            }
                        case "hide":
                            {
                                notifyIconHelper.Hide();
                                break;
                            }
                        case "start":
                            {
                                isWatching = true;
                                thrdWatch = new Thread(new ThreadStart(Watcher));
                                thrdWatch.Start();
                                break;
                            }
                        case "stop":
                            {
                                isWatching = false;
                                break;
                            }
                        default:
                            {
                                Console.WriteLine($"Command \"{cmd}\" not found.");
                                break;
                            }
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error occured when dealing with commands: {ex.Message}");
                }
            }
        }




        private string[] GetArgs(string command)
        {
            string[] args = new string[] { };
            for (int i = 0; i < command.Length; i++)
            {
                if (command[i] == ' ') continue;

            }
            return args;
        }

        public void Exit()
        {
            notifyIconHelper.HideNotifyIcon();
            isExit = true;
        }

        #region Drive Event Watcher
        // This part is referenced from Stack Overflow. Thanks user "learns CSharp"!

        void Watcher()
        {
            try
            {
                WqlEventQuery query = new WqlEventQuery("SELECT * FROM Win32_DeviceChangeEvent");
                ManagementEventWatcher watcher = new ManagementEventWatcher(query);
                watcher.EventArrived += new EventArrivedEventHandler(Watcher_Event_Arrived);

                watcher.Start();
                Console.WriteLine("Watcher started. Waiting for event...");
                // Start listening for events

                while (isWatching && (!isExit))
                {
                    Thread.Sleep(100);
                }
                watcher.Stop();
                Console.WriteLine("Watcher stopped.");
            }
            catch (ManagementException ex)
            {
                Console.WriteLine($"Error occured when trying to start drive watcher: {ex.Message}");
            }
        }

        void Watcher_Event_Arrived(object sender, EventArrivedEventArgs e)
        {
            //Console.WriteLine("Got it!");
            notifyIconHelper.ShowBallon("", "got", 1000);
        }

        #endregion

        #region Force Quit Event

        public delegate bool HandlerRoutine(int dwCtrlType);
        [DllImport("kernel32.dll", CharSet = CharSet.Auto)]
        public static extern bool SetConsoleCtrlHandler(HandlerRoutine HandlerRoutine, bool add);

        public bool CancelHandler(int dwCtrlType)
        {
            Exit();
            switch (dwCtrlType)
            {
                case 0:
                    Console.WriteLine("\nKeyboard-Iterrupt Force Quit."); //Ctrl+C 
                    break;
                case 2:
                    Console.WriteLine("\nClose-Button Force Quit.");//Press Button
                    break;
            }
            //Console.ReadLine();

            return false;
        }

        #endregion
    }
}
