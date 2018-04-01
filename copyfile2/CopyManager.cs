/** Class Info
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
    public class CopyManager
    {
        public CopyManager()
        {

        }

        public bool AddMark(char drive)
        {
            try
            {
                FileStream fs = new FileStream($"{drive}:\\cfmk", FileMode.CreateNew);
                StreamWriter sw = new StreamWriter(fs);
                sw.Write("Hello :)");
                sw.Flush();
                RunCmd($"attrib +s +h {drive}:\\cfmk");
                return true;
            }
            catch(Exception ex)
            {
                ConsoleHelper.EventWriteLine($"Error occcured when adding mark: {ex.Message}");
                return false;
            }
        }

        public bool RemoveMark(char drive)
        {
            try
            {
                ConsoleHelper.EventWriteLine(RunCmd($"del /AS /F {drive}:\\cfmk"));
                return true;
            }
            catch(Exception ex)
            {
                ConsoleHelper.EventWriteLine($"Error occured when removing mark: {ex.Message}");
                return false;
            }
        }

        public string GetMark(char drive)
        {
            try
            {
                return "";
            }
            catch(Exception ex)
            {
                ConsoleHelper.EventWriteLine($"Error occured when getting mark: {ex.Message}");
                return "";
            }
        }

        public bool IsMarked(char drive)
        {
            try
            {
                return File.Exists($"{drive}:\\cfmk");
            }
            catch(Exception ex)
            {
                ConsoleHelper.EventWriteLine($"Error occured when detecting mark: {ex.Message}");
                return false;
            }
        }
        
        /// <summary>
        /// Run cmd.exe command.
        /// </summary>
        /// Thanks user SeanyBrake from CSDN.
        /// <param name="cmd"></param>
        /// <param name="output"></param>
        private static string RunCmd(string cmd)
        {
            string output = string.Empty;
            cmd = cmd.Trim().TrimEnd('&') + "&exit";
            using (Process p = new Process())
            {
                p.StartInfo.FileName = @"C:\Windows\System32\cmd.exe";
                p.StartInfo.UseShellExecute = false;
                p.StartInfo.RedirectStandardInput = true;
                p.StartInfo.RedirectStandardOutput = true;
                p.StartInfo.RedirectStandardError = true;
                p.StartInfo.CreateNoWindow = true;
                p.Start();
                
                p.StandardInput.WriteLine(cmd);
                p.StandardInput.AutoFlush = true;

                output = p.StandardOutput.ReadToEnd();
                p.WaitForExit();
                p.Close();
            }
            return output;
        }


    }
}
