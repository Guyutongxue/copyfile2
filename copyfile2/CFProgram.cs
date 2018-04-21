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
using System.Xml;

namespace copyfile2
{
    class CFProgram
    {
        bool isExit = false;
        bool isWatching = false;

        public const string title = "copyfile2";
        const string info = "copyfile 2.0.0 by Guyutongxue\nReleased under MIT License.\n";
        string appPath = System.AppDomain.CurrentDomain.BaseDirectory;

        NotifyIconHelper notifyIconHelper;

        Thread thrdInput;
        Thread thrdWatch;

        DriveInfo[] diOrigin;

        DictionaryHelper dictMark = new DictionaryHelper();

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

            if (File.Exists(appPath + "cfmk.xml"))
            {
                dictMark.ReadFromFile(appPath + "cfmk.xml");
            }
            else
            {
                dictMark.WriteToFile(appPath + "cfmk.xml");
            }

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
                    string[] args = GetArgs(input);

                    switch (args[0])
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
rm-mark <DRIVE1><DRIVE2>...: remove the mark of several drive.
view-dict: view the dictionary of mark in alias and GUID.
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
                                if (!isWatching)
                                {
                                    isWatching = true;
                                    thrdWatch = new Thread(new ThreadStart(Watcher));
                                    thrdWatch.Start();
                                }
                                else
                                {
                                    Console.WriteLine("Watcher has been started.");
                                }
                                break;
                            }
                        case "stop":
                            {
                                if (isWatching)
                                {
                                    isWatching = false;
                                }
                                else
                                {
                                    Console.WriteLine("Watcher hasn't been started yet.");
                                }

                                break;
                            }
                        case "rm-mark":
                            {
                                foreach (char i in args[1])
                                    CopyManager.RemoveMark(i);
                                break;
                            }
                        case "dict":
                            {
                                Console.WriteLine(dictMark.Operate(args));
                                dictMark.WriteToFile(appPath + "cfmk.xml");
                                break;
                            }
                        default:
                            {
                                Console.WriteLine($"Command \"{args[0]}\" not found.");
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

        /// <summary>
        /// For split a command to arguments array by spaces.
        /// </summary>
        /// <param name="command">The required command</param>
        /// <returns>An array of arguments.</returns>
        private string[] GetArgs(string command)
        {
            List<string> args = new List<string>();
            bool isCmd = false;
            string part = string.Empty;
            for (int i = 0; i < command.Length; i++)
            {
                if (!isCmd)
                {
                    if (command[i] == ' ') continue;
                    else
                    {
                        isCmd = true;
                        part += command[i];
                    }
                }
                else
                {
                    if (command[i] == ' ')
                    {
                        args.Add(part);
                        part = string.Empty;
                        isCmd = false;
                    }
                    else
                    {
                        part += command[i];
                    }
                }
            }
            if (isCmd) args.Add(part);
            return args.ToArray();
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
                
                diOrigin = DriveInfo.GetDrives();

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
            string diff = DriveHelper.DICompare(diOrigin, DriveInfo.GetDrives());
            if (diff != "")
            {
                ConsoleHelper.EventWriteLine($"Got it!{diff}");
                foreach (char i in diff)
                {
                    CopyManager.DoCopy(i, appPath + Analyse(i));
                }

            }
            diOrigin = DriveInfo.GetDrives();
            //notifyIconHelper.ShowBallon("", "got", 1000);
        }
        #endregion

        /// <summary>
        /// Return the alias of drive by analysis.
        /// </summary>
        /// <param name="drive">Drive root directory.</param>
        /// <returns>Alias.</returns>
        string Analyse(char drive)
        {
            if (!CopyManager.IsMarked(drive))
            {
                Guid guid = Guid.NewGuid();
                string alias = guid.ToString().Substring(0, 8);
                CopyManager.AddMark(drive, guid.ToString());
                dictMark.Add(alias, guid);
                dictMark.WriteToFile(appPath + "cfmk.xml");

                ConsoleHelper.EventWriteLine($"New Disk. Init in ({alias},{guid.ToString()}).");
                return alias;
            }
            else
            {
                string mark = CopyManager.GetMark(drive);
                foreach (var i in dictMark.dict)
                {
                    if (i.Value.ToString().Equals(mark))
                    {
                        ConsoleHelper.EventWriteLine($"Found Disk of ({i.Key},{i.Value}).");
                        return i.Key;
                    }
                }
                string alias = mark.Substring(0, 8);
                dictMark.Add(alias, new Guid(mark));
                dictMark.WriteToFile(appPath + "cfmk.xml");

                ConsoleHelper.EventWriteLine($"New Disk but has mark. Init in ({alias},{mark}).");
                return alias;
            }
        }

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





    //Class of dealing drive problems.
    static class DriveHelper
    {

        private static string DItoStr(DriveInfo[] di)
        {
            StringBuilder res = new StringBuilder();
            foreach (DriveInfo i in di)
            {
                res.Append(i.Name.ToString()[0]);
            }
            return res.ToString();
        }

        public static string DICompare(DriveInfo[] diA, DriveInfo[] diB)
        {
            string strA = DItoStr(diA);
            string strB = DItoStr(diB);
            StringBuilder diff = new StringBuilder();
            for (int i = 0; i < strB.Length; i++)
            {
                if (!strA.Contains(strB[i])) diff.Append(strB[i]);
            }
            return diff.ToString();
        }
    }

    static class ConsoleHelper
    {

        /// <summary>
        /// WriteLine when event occured.
        /// </summary>
        /// <param name="value"></param>
        static public void EventWriteLine(string value)
        {
            Console.CursorLeft = 0;
            Console.WriteLine(value);
            Console.Write("> ");
        }
    }
}
