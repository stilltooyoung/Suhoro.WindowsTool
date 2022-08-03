using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;
using System.Windows.Interop;
using System.Windows.Media;

namespace Suhoro.WindowsTool.Core.Utils
{
    public static class WindowExtensions
    {
        #region 去系统菜单
        [DllImport("user32.dll", EntryPoint = "GetSystemMenu")]
        private static extern IntPtr GetSystemMenu(IntPtr hwnd, int revert);

        [DllImport("user32.dll", EntryPoint = "GetMenuItemCount")]
        private static extern int GetMenuItemCount(IntPtr hmenu);

        [DllImport("user32.dll", EntryPoint = "RemoveMenu")]
        private static extern int RemoveMenu(IntPtr hmenu, int npos, int wflags);

        [DllImport("user32.dll", EntryPoint = "DrawMenuBar")]
        private static extern int DrawMenuBar(IntPtr hwnd);

        private const int MF_BYPOSITION = 0x0400;
        private const int MF_DISABLED = 0x0002;

        static void Window_SourceInitialized(object sender, EventArgs e)
        {
            var window = (Window)sender;
            IntPtr windowHandle = new WindowInteropHelper(window).Handle;
            IntPtr hmenu = GetSystemMenu(windowHandle, 0);
            int cnt = GetMenuItemCount(hmenu);

            for (int i = cnt - 1; i >= 0; i--)
            {
                RemoveMenu(hmenu, i, MF_DISABLED | MF_BYPOSITION);
            }
        }
        public static void RemoveWindowSystemMenu(this Window window)
        {
            if (window == null)
            {
                return;
            }
            window.SourceInitialized += Window_SourceInitialized;
        }
        public static void RemoveWindowSystemMenuItem(this Window window, int itemIndex)
        {
            if (window == null)
            {
                return;
            }

            window.SourceInitialized += delegate
            {
                var helper = new WindowInteropHelper(window);
                IntPtr windowHandle = helper.Handle; //Get the handle of this window
                IntPtr hmenu = GetSystemMenu(windowHandle, 0);

                //remove the menu item
                RemoveMenu(hmenu, itemIndex, MF_DISABLED | MF_BYPOSITION);

                DrawMenuBar(windowHandle); //Redraw the menu bar
            };
        }

        #endregion

        public static void Floating(this Window mainWindow,Window floatingWindow)
        {
            floatingWindow.BorderThickness=new Thickness(0, 0, 0, 0);
            floatingWindow.Top = 0;
            floatingWindow.Left = 0;
            floatingWindow.Width = CommonVariables.ScreenWidth;
            floatingWindow.Height = CommonVariables.ScreenHeight;
            floatingWindow.ShowInTaskbar = false;
            floatingWindow.WindowStyle = WindowStyle.None;
            floatingWindow.AllowsTransparency=true;
            floatingWindow.Background = Brushes.Transparent;
            floatingWindow.Topmost = true;
            floatingWindow.RemoveWindowSystemMenu();
            mainWindow.WindowStyle = WindowStyle.ToolWindow;
            mainWindow.ShowInTaskbar = false;
            mainWindow.Show();
            floatingWindow.Owner = mainWindow;
            mainWindow.Hide();
        }
    }
}
