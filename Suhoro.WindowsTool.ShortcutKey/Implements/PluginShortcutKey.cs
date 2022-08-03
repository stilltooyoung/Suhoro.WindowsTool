using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Suhoro.WindowsTool.Core.Interfaces;
using Suhoro.WindowsTool.Core.Models;
using Suhoro.WindowsTool.ShortcutKey.Commands;
using Suhoro.WindowsTool.ShortcutKey.Configs;
using Suhoro.WindowsTool.ShortcutKey.Consts;
using Suhoro.WindowsTool.ShortcutKey.Interfaces;
using Suhoro.WindowsTool.ShortcutKey.Models;
using Suhoro.WindowsTool.ShortcutKey.Utils;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Threading;

namespace Suhoro.WindowsTool.ShortcutKey.Implements
{
    public class PluginShortcutKey : IPlugin
    {
        private readonly ILogger<PluginShortcutKey> logger;
        private readonly IServiceShortcutKey shortcutKeyService;
        private readonly ITrayIcon trayIcon;
        private readonly IServiceProvider service;
        private MenuItem pluginMenu;
        private bool isEnabled=false;
        
        public string Name => "快捷键";

        public bool IsEnabled => isEnabled;
        public MenuItem PluginMenu { get => pluginMenu; set => pluginMenu = value; }

        public PluginShortcutKey(ILogger<PluginShortcutKey> logger,IServiceShortcutKey shortcutKeyService,ITrayIcon trayIcon, IServiceProvider service)
        {
            this.logger = logger;
            this.shortcutKeyService = shortcutKeyService;
            this.trayIcon = trayIcon;
            this.service = service;
        }

        MenuItem BuildPluginTrayMenu()
        {
            var menu = new MenuItem();
            menu.Name = Name;
            menu.Header = Name;
            menu.Click += (obj, e) =>
            {
                var window = service.GetService<WindowSetShortcutKey>();
                window.WindowStartupLocation = WindowStartupLocation.CenterScreen;
                window.Closed += (obj1, e) => {
                    (obj as MenuItem).IsEnabled = true;
                };
                window.Show();
                (obj as MenuItem).IsEnabled = false;
            };
            return menu;
        }

        public void Enable()
        {
            trayIcon.RegisterPluginMenu(pluginMenu);
            shortcutKeyService.Enable();
            isEnabled = true;
        }

        public void Disable()
        {
            trayIcon.UnregisterPluginMenu(pluginMenu);
            shortcutKeyService.Disable();
            isEnabled = false;
        }

        public void Init()
        {
            pluginMenu = BuildPluginTrayMenu();
        }

        public void Exit()
        {
            SettingsPlugin.Default.Save();
        }
    }
}