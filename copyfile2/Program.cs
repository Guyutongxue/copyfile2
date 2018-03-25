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
using System.Threading;
using System.Windows.Forms;

namespace copyfile2
{
    class Program
    {
        bool isExit = false;
        NotifyIconHelper notifyIconHelper;
        const string title = "copyfile 2.0.0 by Guyutongxue\nReleased under MIT License.\n";

        static void Main(string[] args)
        {
            Program program = new Program();
            Console.Title = "copyfile2";
            program.notifyIconHelper = new NotifyIconHelper(@"F:\Cpp\实用工具\Copy2WSL\bash.ico", Console.Title);
            program.notifyIconHelper.ShowNotifyIcon();
            //_program._notifyIconHelper.ShowBallon("copyfile2", "copyfile2 is running in background.", 1000);

            Console.Write(title);
            Thread thrdInput = new Thread(new ThreadStart(program.GetInput));
            thrdInput.Start();
            while (true)
            {
                Thread.Sleep(100); //or the CPU will boom
                Application.DoEvents();
                if (program.isExit)
                {
                    break;
                }
            }
        }
        void GetInput()
        {
            while (true)
            {
                Thread.Sleep(100);
                Console.Write("> ");
                string input = Console.ReadLine();
                string cmd = input.Split(' ')[0];
                try
                {
                    switch (cmd)
                    {
                        
                        case "exit":
                            {
                                isExit = true;
                                goto endOfThread;
                            }
                        case "hide":
                            {
                                notifyIconHelper.Hide();
                                break;
                            }
                        default:
                            {
                                Console.WriteLine("Command \"" + cmd + "\" not found.");
                                break;
                            }
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Error occured: " + ex.Message);
                }
            }
            endOfThread:;
        }

        string[] GetArgs(string command)
        {
            string[] args = new string[] { };
            for(int i = 0; i < command.Length; i++)
            {
                if (command[i] == ' ') continue;

            }
            return args;
        }
    }
}
