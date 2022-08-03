using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace Suhoro.WindowsTool.Core.Interfaces
{
    public interface IFloatingIcon
    {
        Image FloatingIcon { get;}
        void HideToBorder();
        void ShowIcon();
        void HideIcon();
    }
}
