using Suhoro.WindowsTool.Core.Models;
using Suhoro.WindowsTool.Core.Utils;
using Suhoro.WindowsTool.ShortcutKey.Consts;
using Suhoro.WindowsTool.ShortcutKey.Interfaces;
using Suhoro.WindowsTool.ShortcutKey.Models;
using Suhoro.WindowsTool.ShortcutKey.ViewModels;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace Suhoro.WindowsTool.ShortcutKey
{
    /// <summary>
    /// ShortcutKeyWindow.xaml 的交互逻辑
    /// </summary>
    public partial class WindowSetShortcutKey : Window
    {
        public WindowSetShortcutKey(VmWindow vmWindow)
        {
            
            InitializeComponent();
            this.DataContext = vmWindow;
        }
    }
}
