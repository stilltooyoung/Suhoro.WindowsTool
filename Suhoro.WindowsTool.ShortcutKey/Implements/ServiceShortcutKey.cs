using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Suhoro.WindowsTool.Core.Interfaces;
using Suhoro.WindowsTool.Core.Models;
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
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Threading;

namespace Suhoro.WindowsTool.ShortcutKey.Implements
{
    public class ServiceShortcutKey : IServiceShortcutKey
    {
        private readonly ILogger<ServiceShortcutKey> logger;
        private readonly LocalDb localDb;

        public IEnumerable<IUserVisibleCommand> Commands => commands;

        public static bool IsAppShortcutKeyOn { get; set; } = true;
        private readonly IEnumerable<IUserVisibleCommand> commands;
        private TimeSpan interval;
        private Queue<(Key key, DateTime time)> keyQueue = new Queue<(Key, DateTime)>();
        volatile int skipMappingKeyCount = 0;

        //快捷键字典 键为监听键的字符串表示
        private Dictionary<string, DbShortcutKey> hotKeyDic;
        private MenuItem pluginMenu;
        public MenuItem PluginMenu { get => pluginMenu; set => pluginMenu = value; }
        public ServiceShortcutKey(ILogger<ServiceShortcutKey> logger,LocalDb localDb,IEnumerable<IUserVisibleCommand> commands)
        {
            this.logger = logger;
            this.localDb = localDb;
            this.commands = commands;
            LoadHotKey();
        }

        #region Db
        public void AddRange(IEnumerable<DbShortcutKey> keys)
        {
            localDb.AddRange(keys);
            localDb.SaveChanges();
        }

        public IQueryable<DbShortcutKey> GetEnables()
        {
            return localDb.Set<DbShortcutKey>().Where(x => !x.IsDeleted && x.IsEnable);
        }

        public IQueryable<DbShortcutKey> GetValids()
        {
            return localDb.Set<DbShortcutKey>().Where(x => !x.IsDeleted);
        }

        public void SaveData(List<DbShortcutKey> toSyncList)
        {
            var dbData = GetValids().ToList();
            var toDelete = dbData.Where(x => !toSyncList.Any(y => x.Id == y.Id));
            foreach (var item in toDelete)
            {
                item.IsDeleted = true;
            }
            foreach (var item in toSyncList)
            {
                var dbEntity = dbData.FirstOrDefault(x => x.Id == item.Id);
                if (dbEntity == null)
                {
                    localDb.Set<DbShortcutKey>().Add(item);
                }
                else
                {
                    localDb.Entry(dbEntity).CurrentValues.SetValues(item);
                }
            }
            localDb.SaveChanges();
        }
        #endregion

        void LoadHotKey()
        {
            interval = TimeSpan.FromMilliseconds(SettingsPlugin.Default.HotKeyInterval);
            hotKeyDic = new Dictionary<string, DbShortcutKey>();
            var dbEntities = GetEnables().AsNoTracking().ToList();
            foreach (var item in dbEntities)
            {
                if (item.Type == ShortcutKeyType.AppCommand)
                {
                    item.Command = commands.FirstOrDefault(c => c.GetType().FullName == item.Mapping);
                }
                hotKeyDic.Add(item.ListenKeyCodes, item);
            }
        }

        void Execute(DbShortcutKey shortcutKey)
        {
            if (!IsAppShortcutKeyOn && !shortcutKey.IsAlwaysEffect)
            {
                return;
            }
            switch (shortcutKey.Type)
            {
                case ShortcutKeyType.SystemCommand:
                    try
                    {
                        if (string.IsNullOrEmpty(shortcutKey.Args))
                        {
                            Process.Start(shortcutKey.Mapping);
                        }
                        else
                        {
                            Process.Start(shortcutKey.Mapping, shortcutKey.Args);
                        }
                    }
                    catch (Exception ex)
                    {
                        Dispatcher.CurrentDispatcher.Invoke(() => HandyControl.Controls.MessageBox.Show(ex.Message));
                    }
                    break;
                case ShortcutKeyType.AppCommand:
                    shortcutKey.Command?.Execute(shortcutKey.Args);
                    break;
                default:
                    break;
            }
        }

        void KeyEventHandler(object? obj, HotKeyEventArgs args)
        {
            //优先处理键位映射
            var keyCode = args.Key.ToCode();
            if (hotKeyDic.ContainsKey(keyCode) && hotKeyDic[keyCode].Type == ShortcutKeyType.Key)
            {
                if (skipMappingKeyCount == 0)
                {
                    if (!string.IsNullOrEmpty(hotKeyDic[keyCode].Mapping))
                    {
                        var mappingKeys = hotKeyDic[keyCode].Mapping!.ToKeys();
                        if (args.KeyEvent == KeyboardEvent.KEYDOWN)
                        {
                            Interlocked.Add(ref skipMappingKeyCount, mappingKeys.Count() * 2);
                            KeyboardImitator.Instance.KeysDownUp(mappingKeys);
                        }
                    }
                    args.ShouldCallNext = false;
                    return;
                }
            }
            if (skipMappingKeyCount > 0)
            {
                Interlocked.Add(ref skipMappingKeyCount, -1);
            }

            //处理cmd命令和程序命令映射
            if (args.KeyEvent == KeyboardEvent.KEYDOWN  || args.KeyEvent == KeyboardEvent.SYSKEYDOWN)
            {
                var now = DateTime.Now;
                while (keyQueue.Count > 0 && now - keyQueue.Peek().time > interval)
                {
                    keyQueue.Dequeue();
                }
                keyQueue.Enqueue((args.Key, now));
                var keyCodes = keyQueue.Select(x => x.key.ToCode()).ToArray();
                for (int i = 0; i < keyCodes.Length; i++)
                {
                    var hotKeyCode = string.Concat(keyCodes[i..]);
                    if (hotKeyDic.ContainsKey(hotKeyCode))
                    {
                        keyQueue.Clear();
                        Execute(hotKeyDic[hotKeyCode]);
                        break;
                    }
                }
            }
        }

        public void ReloadHotKey()
        {
            LoadHotKey();
        }
        public void Disable()
        {
            KeyboardListener.Instance.KeyboardEventHandler -= KeyEventHandler;
        }

        public void Enable()
        {
            KeyboardListener.Instance.KeyboardEventHandler += KeyEventHandler;
        }
    }
}
