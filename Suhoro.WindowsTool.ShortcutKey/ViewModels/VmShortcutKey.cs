using AutoMapper;
using PropertyChanged;
using Suhoro.WindowsTool.Core.Interfaces;
using Suhoro.WindowsTool.Core.Utils;
using Suhoro.WindowsTool.ShortcutKey.Consts;
using Suhoro.WindowsTool.ShortcutKey.Models;
using Suhoro.WindowsTool.ShortcutKey.Utils;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using System.Windows.Media;

namespace Suhoro.WindowsTool.ShortcutKey.ViewModels
{
    public class VmShortcutKey: INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler? PropertyChanged;
        public Guid Id { get; set; }
        public DateTime CreateTime { get; set; }
        public DateTime UpdateTime { get; set; }
        public bool IsDeleted { get; set; }
        public bool IsEnable { get; set; } = true;
        public string Name { get; set; } = String.Empty;
        public string Description { get; set; } = String.Empty;
        public string ListenKeyCodes { get; set; }=String.Empty;
        public ShortcutKeyType Type { get; set; }
        public string? Mapping { get; set; }
        public string? Args { get; set; }
        public bool IsAlwaysEffect { get; set; } = false;

        /// <summary>
        /// 关联的程序命令
        /// </summary>
        public IUserVisibleCommand? Command { get; set; }
        [DependsOn(nameof(ListenKeyCodes))]
        public string ListenKeysDisplay => ListenKeyCodes.ToKeysDisplay();

        [DependsOn(nameof(Mapping))]
        public string MappingKeysDisplay => Type==ShortcutKeyType.Key?Mapping.ToKeysDisplay():string.Empty;

        public Brush Background { get; set; }= null;

        public ICommand CommandSetListenKeys { get; private set; }
        public ICommand CommandSetMappingKeys { get; private set; }

        public VmShortcutKey()
        {
            InitCommandSetListenKeys();
            InitCommandSetMappingKeys();
        }

        void KeyDown(KeyEventArgs e,bool isListenKey)
        {
            var key = e.Key == Key.ImeProcessed ? e.ImeProcessedKey : e.Key;
            var code = isListenKey ? this.ListenKeyCodes : this.Mapping;
            switch (key)
            {
                case Key.Back:
                    if (!string.IsNullOrEmpty(code))
                    {
                        code = code[0..^3];
                    }
                    break;
                default:
                    if (this.Type == ShortcutKeyType.Key && isListenKey && this.ListenKeyCodes.Length == 3)
                    {
                        //键位映射限制为单键映射
                        code = key.ToCode();
                    }
                    else
                    {
                        code += key.ToCode();
                    }
                    break;
            }
            if (isListenKey)
            {
                this.ListenKeyCodes = code;
            }
            else
            {
                this.Mapping = code;
            }
            e.Handled = true;
        }

        void InitCommandSetListenKeys()
        {
            CommandSetListenKeys = new GeneralCommand<VmShortcutKey>(this, (param, vm) =>
            {
                vm.KeyDown(param as KeyEventArgs,true);
            });
        }

        void InitCommandSetMappingKeys()
        {
            CommandSetMappingKeys = new GeneralCommand<VmShortcutKey>(this, (param, vm) =>
            {
                vm.KeyDown(param as KeyEventArgs, false);
            });
        }

        public static List<List<VmShortcutKey>> GetDuplicated(IEnumerable<VmShortcutKey> entities)
        {
            var result = new List<List<VmShortcutKey>>();
            var orders = entities.OrderBy(e => e.ListenKeyCodes.Length).ToList();
            for (int i = 0; i < orders.Count; i++)
            {
                var temp = new List<VmShortcutKey> { orders[i] };
                for (int j = i + 1; j < orders.Count; j++)
                {
                    if (orders[j].ListenKeyCodes.StartsWith(orders[i].ListenKeyCodes))
                    {
                        temp.Add(orders[j]);
                    }
                }
                if (temp.Count > 1)
                {
                    result.Add(temp);
                }
            }
            return result;
        }
    }

    public class ShortcutKeyProfile : Profile
    {
        public ShortcutKeyProfile()
        {
            CreateMap<DbShortcutKey,VmShortcutKey>().ReverseMap();
        }
    }
}
