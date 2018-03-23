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
        private NotifyIcon _notifyIcon = new NotifyIcon();
        private ContextMenu _menu = new ContextMenu();
        private MenuItem _item = new MenuItem
        {
            Text = "Hide",
            Index = 0
        };

        bool isHidden = false;
        string windowTitle = String.Empty;

        public NotifyIconHelper(string iconPath, string title)
        {
            
            _notifyIcon.Icon = new Icon(iconPath);
            windowTitle = title;
            _notifyIcon.Visible = false;
            _notifyIcon.Text = windowTitle;
            AddMenuItems();
        }

        public void ShowNotifyIcon()
        {
            _notifyIcon.Visible = true;
        }

        public void ShowBallon(string tipTitle, string tipText, int timeout)
        {
            _notifyIcon.ShowBalloonTip(timeout, tipTitle, tipText, ToolTipIcon.None);
        }

        public void HideNotifyIcon()
        {
            _notifyIcon.Visible = false;
        }

        private void _item_Click(object sender, EventArgs e)
        {
            if (isHidden)
            {
                Show();
            }
            else
            {
                Hide();
            }
        }

        private void _notifyIcon_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            Show();
        }


        private void AddMenuItems()
        {
            
            _menu.MenuItems.Add(_item);
            _item.Click += new EventHandler(_item_Click);
            _notifyIcon.ContextMenu = _menu;
            _notifyIcon.MouseDoubleClick += new MouseEventHandler(_notifyIcon_MouseDoubleClick);
        }


        [DllImport("User32.dll", EntryPoint = "FindWindow")]
        static extern IntPtr FindWindow(string lpClassName, string lpWindowName);
        [DllImport("user32.dll", EntryPoint = "GetSystemMenu")]
        static extern IntPtr GetSystemMenu(IntPtr hWnd, IntPtr bRevert);
        [DllImport("user32.dll", EntryPoint = "RemoveMenu")]
        static extern IntPtr RemoveMenu(IntPtr hMenu, uint uPosition, uint uFlags);
        [DllImport("User32.dll", EntryPoint = "ShowWindow")]
        public static extern bool ShowWindow(IntPtr hwind, int cmdShow);
        [DllImport("User32.dll", EntryPoint = "SetForegroundWindow")]
        public static extern bool SetForegroundWindow(IntPtr hwind);

        /*
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
            int normalState = 0;//窗口状态(隐藏)
            ShowWindow(ParenthWnd, normalState);
            isHidden = true;
            _item.Text = "Show";
        }

        public void Show()
        {
            IntPtr ParenthWnd = new IntPtr(0);
            IntPtr et = new IntPtr(0);
            ParenthWnd = FindWindow(null,windowTitle);
            int normalState = 9;//窗口状态(隐藏)
            ShowWindow(ParenthWnd, normalState);
            isHidden = false;
            _item.Text = "Hide";
        }

    }
}
