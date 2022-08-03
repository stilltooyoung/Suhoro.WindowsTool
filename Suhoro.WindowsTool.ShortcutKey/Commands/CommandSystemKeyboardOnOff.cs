using Suhoro.WindowsTool.Core.Interfaces;
using Suhoro.WindowsTool.ShortcutKey.Configs;
using Suhoro.WindowsTool.ShortcutKey.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Suhoro.WindowsTool.ShortcutKey.Commands
{
    public class CommandSystemKeyboardOnOff : IUserVisibleCommand
    {
        public string Name { get; set; } = "系统键盘事件 开/关";
        public string Description { get; set; } = "系统键盘事件 开/关";

        public event EventHandler? CanExecuteChanged;

        public bool CanExecute(object? parameter)
        {
            return true;
        }

        public void Execute(object? parameter)
        {
              KeyboardListener.Instance.ShouldPassKeyboard = !KeyboardListener.Instance.ShouldPassKeyboard;
        }
    }
}
