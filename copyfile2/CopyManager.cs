﻿/** Class Info
 * CopyManager by Guyutongxue
 * For managing copying file.
 * Released under MIT License.
**/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;
using System.IO;

namespace copyfile2
{
    static public class CopyManager
    {
        
        static public bool AddMark(char drive, string text)
        {
            try
            {
                FileStream fs = new FileStream($"{drive}:\\cfmk", FileMode.CreateNew);
                StreamWriter sw = new StreamWriter(fs);
                sw.Write(text);
                sw.Flush();
                RunProcess("attrib", $"+s +h {drive}:\\cfmk");
                fs.Close();
                return true;
            }
            catch (Exception ex)
            {
                ConsoleHelper.EventWriteLine($"Error occcured when adding mark: {ex.Message}");
                return false;
            }
        }

        static public bool RemoveMark(char drive)
        {
            try
            {
                //ConsoleHelper.EventWriteLine(RunCmd($"del /AS /F {drive}:\\cfmk"));
                File.Delete($"{drive}:\\cfmk");
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error occured when removing mark: {ex.Message}");
                return false;
            }
        }

        static public string GetMark(char drive)
        {
            try
            {
                FileStream fs = new FileStream($"{drive}:\\cfmk", FileMode.Open);
                StreamReader reader = new StreamReader(fs);
                string res = reader.ReadToEnd();
                fs.Close();
                return res;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error occured when getting mark: {ex.Message}");
                return "";
            }
        }

        static public bool IsMarked(char drive)
        {
            try
            {
                return File.Exists($"{drive}:\\cfmk");
            }
            catch (Exception ex)
            {
                ConsoleHelper.EventWriteLine($"Error occured when detecting mark: {ex.Message}");
                return false;
            }
        }

        static void RunProcess(string filename, string argument)
        {
            string output = "";
            try
            {
                Process cmd = new Process();

                cmd.StartInfo.FileName = filename;
                cmd.StartInfo.Arguments = argument;

                cmd.StartInfo.UseShellExecute = false;

                cmd.StartInfo.RedirectStandardInput = true;
                cmd.StartInfo.RedirectStandardOutput = true;

                cmd.StartInfo.CreateNoWindow = true;
                cmd.StartInfo.WindowStyle = ProcessWindowStyle.Hidden;

                cmd.Start();

                output = cmd.StandardOutput.ReadToEnd();
                ConsoleHelper.EventWriteLine(output);
                cmd.WaitForExit();
                cmd.Close();
            }
            catch (Exception ex)
            {
                ConsoleHelper.EventWriteLine($"Error occured when copying file:{ex.Message}");
            }
        }

        static public void DoCopy(char drive, string dist)
        {
            ConsoleHelper.EventWriteLine("Start copying...");
            RunProcess("xcopy", $"/s /q /e /h /y {drive}:\\ {dist}\\");
            ConsoleHelper.EventWriteLine("Copy finished!");
        }
    }
}
