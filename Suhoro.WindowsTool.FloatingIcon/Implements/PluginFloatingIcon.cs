using Microsoft.Extensions.DependencyInjection;
using Suhoro.WindowsTool.Core.Converters;
using Suhoro.WindowsTool.Core.Interfaces;
using Suhoro.WindowsTool.FloatingIcon.Configs;
using Suhoro.WindowsTool.FloatingIcon.Windows;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;

namespace Suhoro.WindowsTool.FloatingIcon.Implements
{
    public class PluginFloatingIcon : IPlugin
    {
        private bool isEnabled;
        private MenuItem pluginMenu;
        private readonly WindowFloating window;
        private readonly ITrayIcon trayIcon;
        private readonly IServiceProvider service;

        public string Name => "浮动图标";

        public bool IsEnabled { get => isEnabled; set => isEnabled = value; }

        public MenuItem PluginMenu { get => pluginMenu; private set => pluginMenu = value; }

        public PluginFloatingIcon(WindowFloating window,ITrayIcon trayIcon,IServiceProvider service)
        {
            this.window = window;
            this.trayIcon = trayIcon;
            this.service = service;
        }

        void BuildPluginMenu()
        {
            pluginMenu = new MenuItem();
            pluginMenu.Name = Name;
            pluginMenu.Header = Name;
            pluginMenu.Click += (obj, e) =>
            {
                var window= service.GetService<WindowFloatingSettings>() as Window;
                window.WindowStartupLocation = WindowStartupLocation.CenterScreen;
                window.Closed += (obj1, e) => {
                    (obj as MenuItem).IsEnabled = true;
                };
                window.Show();
                (obj as MenuItem).IsEnabled = false;
            };
        }

        public void Disable()
        {
            trayIcon.UnregisterPluginMenu(pluginMenu);
            window.Visibility = Visibility.Hidden;
        }

        public void Enable()
        {
            trayIcon.RegisterPluginMenu(pluginMenu);
            window.Visibility= Visibility.Visible;
        }

        public void Init()
        {
            BuildPluginMenu();
        }

        public void Exit()
        {
            SettingsPlugin.Default.Save();
        }
    }
}
