using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Suhoro.WindowsTool.Core.Interfaces;
using Suhoro.WindowsTool.Core.Models;
using Suhoro.WindowsTool.Core.Utils;
using Suhoro.WindowsTool.ShortcutKey.Configs;
using Suhoro.WindowsTool.ShortcutKey.Consts;
using Suhoro.WindowsTool.ShortcutKey.Implements;
using Suhoro.WindowsTool.ShortcutKey.Interfaces;
using Suhoro.WindowsTool.ShortcutKey.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

namespace Suhoro.WindowsTool.ShortcutKey.ViewModels
{
    public class VmWindow : INotifyPropertyChanged
    {
        private readonly IServiceShortcutKey service;
        private readonly IMapper mapper;

        public event PropertyChangedEventHandler? PropertyChanged;
        public ICommand CommandSave { get; private set; }
        public ICommand CommandInitializingNewItemForKey { get; private set; }
        public ICommand CommandInitializingNewItemForCmd { get; private set; }
        public ICommand CommandInitializingNewItemForCommand { get; private set; }
        public int HotKeyInterval { get; set; }
        public ObservableCollection<VmShortcutKey> ShortcutKeysForKey { get; set; }
        public ObservableCollection<VmShortcutKey> ShortcutKeysForCmd { get; set; }
        public ObservableCollection<VmShortcutKey> ShortcutKeysForCommand { get; set; }

        public ObservableCollection<IUserVisibleCommand> Commands { get; set; }

        public VmWindow(IServiceShortcutKey service, IMapper mapper, IEnumerable<IUserVisibleCommand> commands)
        {
            this.service = service;
            this.mapper = mapper;
            Commands = new ObservableCollection<IUserVisibleCommand>(commands.ToList());
            Load();
        }

        void Load()
        {
            HotKeyInterval = SettingsPlugin.Default.HotKeyInterval;
            var keys = mapper.Map<List<VmShortcutKey>>(service.GetValids().AsNoTracking().ToList());
            ShortcutKeysForKey = new ObservableCollection<VmShortcutKey>(keys.Where(x => x.Type == ShortcutKeyType.Key));
            ShortcutKeysForCmd = new ObservableCollection<VmShortcutKey>(keys.Where(x => x.Type == ShortcutKeyType.SystemCommand));
            ShortcutKeysForCommand = new ObservableCollection<VmShortcutKey>(keys.Where(x => x.Type == ShortcutKeyType.AppCommand));
            foreach (var item in ShortcutKeysForCommand)
            {
                item.Command = Commands.FirstOrDefault(c => c.GetType().FullName == item.Mapping);
            }
            InitCommandSave();
            InitCommandInitializingNewItemForKey();
            InitCommandInitializingNewItemForCmd();
            InitCommandInitializingNewItemForCommand();
        }

        void InitCommandSave()
        {
            CommandSave = new GeneralCommand<VmWindow>(this, (param, vm) =>
            {
                var e = param as CancelEventArgs;
                SettingsPlugin.Default.HotKeyInterval = HotKeyInterval;
                SettingsPlugin.Default.Save();

                var keys = vm.ShortcutKeysForKey.Concat(ShortcutKeysForCmd).Concat(ShortcutKeysForCommand);
                foreach (var key in keys)
                {
                    key.Background = null;
                }
                var duplications = VmShortcutKey.GetDuplicated(keys);
                if (duplications != null && duplications.Count > 0)
                {
                    var random = new Random();
                    foreach (var duplication in duplications)
                    {
                        var min = 150;
                        var max = 255;
                        var color = Color.FromRgb((byte)random.Next(min, max), (byte)random.Next(min, max), (byte)random.Next(min, max));
                        foreach (var item in duplication)
                        {
                            item.Background = new SolidColorBrush(color);
                        }
                    }
                    HandyControl.Controls.MessageBox.Show("快捷键存在重复路径，请重新设置");
                    e.Cancel = true;
                    return;
                }
                foreach (var item in vm.ShortcutKeysForCommand)
                {
                    item.Mapping = item.Command?.GetType().FullName;
                }
                var emptyKeys = keys.Where(k => string.IsNullOrEmpty(k.ListenKeyCodes)).ToList();
                if (emptyKeys.Any())
                {
                    emptyKeys.ForEach(k => k.Background = Brushes.Yellow);
                    HandyControl.Controls.MessageBox.Show("快捷键不能为空");
                    e.Cancel = true;
                    return;
                }
                var limitCmdAndCommandKeys = keys.Where(k => (k.Type == ShortcutKeyType.SystemCommand || k.Type == ShortcutKeyType.AppCommand) && k.ListenKeyCodes.Length < 6).ToList();
                if (limitCmdAndCommandKeys.Any())
                {
                    limitCmdAndCommandKeys.ForEach(k => k.Background = Brushes.Yellow);
                    HandyControl.Controls.MessageBox.Show("非键位映射快捷键至少要设置两个键");
                    e.Cancel = true;
                    return;
                }
                var vmKeys = vm.ShortcutKeysForKey.Concat(ShortcutKeysForCmd).Concat(vm.ShortcutKeysForCommand);
                var dbKeys = vm.mapper.Map<List<DbShortcutKey>>(vmKeys);
                vm.service.SaveData(dbKeys);
                vm.service.ReloadHotKey();
                HandyControl.Controls.MessageBox.Show("保存成功！");
            });
        }
        void InitCommandInitializingNewItemForKey() 
        {
            CommandInitializingNewItemForKey = new GeneralCommand(param =>
            {
                var e = param as InitializingNewItemEventArgs;
                var newItem= e.NewItem as VmShortcutKey;
                newItem.Type = ShortcutKeyType.Key;
                newItem.IsAlwaysEffect = true;
            });
        }
        void InitCommandInitializingNewItemForCmd() 
        {
            CommandInitializingNewItemForCmd = new GeneralCommand(param =>
            {
                var e = param as InitializingNewItemEventArgs;
                var newItem = e.NewItem as VmShortcutKey;
                newItem.Type = ShortcutKeyType.SystemCommand;
            });
        }
        void InitCommandInitializingNewItemForCommand() 
        {
            CommandInitializingNewItemForCommand = new GeneralCommand(param =>
            {
                var e = param as InitializingNewItemEventArgs;
                var newItem = e.NewItem as VmShortcutKey;
                newItem.Type = ShortcutKeyType.AppCommand;
            });
        }
    }
}
