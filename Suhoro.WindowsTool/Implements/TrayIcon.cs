using Hardcodet.Wpf.TaskbarNotification;
using Microsoft.Extensions.Logging;
using Suhoro.WindowsTool.Configs;
using Suhoro.WindowsTool.Core.Interfaces;
using Suhoro.WindowsTool.Core.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace Suhoro.WindowsTool.Implements
{
    /// <summary>
    /// 系统托盘图标
    /// </summary>
    public class TrayIcon : ITrayIcon,IDisposable
    {
        private readonly TaskbarIcon taskbarIcon = new TaskbarIcon();
        private readonly ILogger<TrayIcon> logger;
        private readonly IFloatingIcon? floatingIcon;

        public TrayIcon(ILogger<TrayIcon> logger,IEnumerable<IFloatingIcon> floatingIcon)
        {
            this.logger = logger;
            this.floatingIcon = floatingIcon.FirstOrDefault();
            InitTaskbarIcon();
            logger.LogInformation("系统图标已启动");
        }

        private void InitTaskbarIcon()
        {
            taskbarIcon.Icon = new System.Drawing.Icon("Resources/icon.ico");
            taskbarIcon.ToolTipText = "hi,sir";
            ContextMenu contextMenu = new ContextMenu();
            AddAutoStartup(contextMenu);

            AddExit(contextMenu);

            taskbarIcon.ContextMenu = contextMenu;

            if (floatingIcon!=null)
            {
                taskbarIcon.TrayMouseDoubleClick += (obj, e) =>
                {
                    if (floatingIcon.FloatingIcon.Visibility == Visibility.Visible)
                    {
                        floatingIcon.HideIcon();
                    }
                    else
                    {
                        floatingIcon.ShowIcon();
                    }
                };
            }
        }
        private void AddAutoStartup(ContextMenu contextMenu)
        {
            MenuItem item = new MenuItem();
            item.Header = "开机自启";
            item.IsCheckable = true;
            item.IsChecked = SettingsApp.Default.IsAutoStart;
            item.Checked += (obj, e) => {
                if (AppStartupUtils.EnableBootStartByWindowsTask())
                {
                    SettingsApp.Default.IsAutoStart = true;
                    SettingsApp.Default.Save();
                }
                else
                {
                    (obj as MenuItem).IsChecked = false;
                }
            };
            item.Unchecked += (obj, e) => {
                if (AppStartupUtils.DisableBootStartByWindowsTask())
                {
                    SettingsApp.Default.IsAutoStart = false;
                    SettingsApp.Default.Save();
                }
                else
                {
                    (obj as MenuItem).IsChecked = true;
                }
            };
            contextMenu.Items.Add(item);
        }
        private void AddExit(ContextMenu contextMenu)
        {
            MenuItem exit = new MenuItem();
            exit.Header = "退出";
            exit.Click += (obj, e) =>
            {
                Application.Current.Shutdown();
            };

            contextMenu.Items.Add(exit);
        }

        public void RegisterPluginMenu(MenuItem pluginMenu)
        {
            taskbarIcon.ContextMenu.Items.Insert(0, pluginMenu);
        }

        public void UnregisterPluginMenu(MenuItem pluginMenu)
        {
            taskbarIcon.ContextMenu.Items.Remove(pluginMenu);
        }

        public void Dispose()
        {
            taskbarIcon?.Dispose();
        }
    }
}
