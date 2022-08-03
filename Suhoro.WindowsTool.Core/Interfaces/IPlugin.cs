using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace Suhoro.WindowsTool.Core.Interfaces
{
    public interface IPlugin
    {
        string Name { get; }
        bool IsEnabled { get; }
        MenuItem PluginMenu { get; }

        void Init();
        void Exit();
        void Enable();
        void Disable();
    }
}
