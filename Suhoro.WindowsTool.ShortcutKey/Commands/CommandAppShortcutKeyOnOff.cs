using Suhoro.WindowsTool.Core.Interfaces;
using Suhoro.WindowsTool.ShortcutKey.Implements;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Suhoro.WindowsTool.ShortcutKey.Commands
{
    public class CommandAppShortcutKeyOnOff : IUserVisibleCommand
    {
        public string Name { get; set; } = "应用快捷键 开/关";
        public string Description { get; set; } = "应用快捷键 开/关";

        public event EventHandler? CanExecuteChanged;

        public bool CanExecute(object? parameter)
        {
            return true;
        }

        public void Execute(object? parameter)
        {
            ServiceShortcutKey.IsAppShortcutKeyOn = !ServiceShortcutKey.IsAppShortcutKeyOn;
        }
    }
}
