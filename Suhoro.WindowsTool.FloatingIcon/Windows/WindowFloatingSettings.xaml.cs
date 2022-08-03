using Suhoro.WindowsTool.FloatingIcon.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace Suhoro.WindowsTool.FloatingIcon.Windows
{
    /// <summary>
    /// WindowFloatingSettings.xaml 的交互逻辑
    /// </summary>
    public partial class WindowFloatingSettings : Window
    {
        public WindowFloatingSettings(VmFloatingSettings vmFloatingSettings)
        {
            InitializeComponent();
            DataContext = vmFloatingSettings;
        }
    }
}
