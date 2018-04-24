/** Class Info
 * NotifyIconHelper by Guyutongxue
 * for showing ConsoleApp's NotifyIcon
 * Released under MIT License.
**/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.InteropServices;
using System.Drawing;
using System.Windows.Forms;

namespace copyfile2
{
    class NotifyIconHelper
    {
        private NotifyIcon notifyIcon = new NotifyIcon();
        private ContextMenu menu = new ContextMenu();
        private MenuItem itemSH = new MenuItem
        {
            Text = "Hide",
            Index = 0
        };
        private MenuItem itemExit = new MenuItem
        {
            Text = "Exit",
            Index = 1
        };

        bool isHidden = false;
        string windowTitle = String.Empty;

        public NotifyIconHelper(Icon icon,string title)
        {
            notifyIcon.Icon = icon;
            windowTitle = title;
            notifyIcon.Visible = false;
            notifyIcon.Text = windowTitle;
            AddMenuItems();
        }

        public void ShowNotifyIcon()
        {
            notifyIcon.Visible = true;
        }

public void HideNotifyIcon()
        {
            notifyIcon.Visible = false;
        }

        public void ShowBallon(string tipTitle, string tipText, int timeout)
        {
            notifyIcon.ShowBalloonTip(timeout, tipTitle, tipText, ToolTipIcon.None);
        }

        
        private void _itemSH_Click(object sender, EventArgs e)
        {
            if (isHidden) Show();
            else Hide();
        }

        private void _itemExit_Click(object sender,EventArgs e)
        {
            CopyManager.RunProcess("taskkill", "/f /im xcopy.exe");
            HideNotifyIcon();
            Environment.Exit(0);
        }

        private void _notifyIcon_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            Show();
        }


        private void AddMenuItems()
        {
            
            menu.MenuItems.Add(itemSH);
            itemSH.Click += new EventHandler(_itemSH_Click);
            menu.MenuItems.Add(itemExit);
            itemExit.Click += new EventHandler(_itemExit_Click);

            notifyIcon.ContextMenu = menu;
            notifyIcon.MouseDoubleClick += new MouseEventHandler(_notifyIcon_MouseDoubleClick);
        }


        [DllImport("user32.dll", EntryPoint = "FindWindow")]
        static extern IntPtr FindWindow(string lpClassName, string lpWindowName);
        [DllImport("user32.dll", EntryPoint = "GetSystemMenu")]
        static extern IntPtr GetSystemMenu(IntPtr hWnd, IntPtr bRevert);
        [DllImport("user32.dll", EntryPoint = "RemoveMenu")]
        static extern IntPtr RemoveMenu(IntPtr hMenu, uint uPosition, uint uFlags);
        [DllImport("user32.dll", EntryPoint = "ShowWindow")]
        public static extern bool ShowWindow(IntPtr hwind, int cmdShow);
        [DllImport("user32.dll", EntryPoint = "SetForegroundWindow")]
        public static extern bool SetForegroundWindow(IntPtr hwind);

        /* IsExistsConsole
        public static bool IsExistsConsole(string title)
        {
            IntPtr windowHandle = FindWindow(null, title);
            if (windowHandle.Equals(IntPtr.Zero))
            {
                return false;
            }
            return true;
        }
        */

        public void Hide()
        {
            IntPtr ParenthWnd = new IntPtr(0);
            IntPtr et = new IntPtr(0);
            ParenthWnd = FindWindow(null,windowTitle);
            int normalState = 0;//Hide
            ShowWindow(ParenthWnd, normalState);
            isHidden = true;
            itemSH.Text = "Show";
        }

        public void Show()
        {
            IntPtr ParenthWnd = new IntPtr(0);
            IntPtr et = new IntPtr(0);
            ParenthWnd = FindWindow(null,windowTitle);
            int normalState = 9; //Show
            ShowWindow(ParenthWnd, normalState);
            isHidden = false;
            itemSH.Text = "Hide";
        }

    }
}
