using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace Suhoro.WindowsTool.Core.Interfaces
{
    public interface ITrayIcon
    {
        void RegisterPluginMenu(MenuItem pluginMenu);
        void UnregisterPluginMenu(MenuItem pluginMenu);
    }
}
