using Suhoro.WindowsTool.Core.Interfaces;
using Suhoro.WindowsTool.Core.Utils;
using Suhoro.WindowsTool.FloatingIcon.Configs;
using System;
using System.Collections.Generic;
using System.IO;
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
using static Suhoro.WindowsTool.Core.Utils.FrameworkElementExtensions;

namespace Suhoro.WindowsTool.FloatingIcon.Windows
{
    /// <summary>
    /// FloatingWindow.xaml 的交互逻辑
    /// </summary>
    public partial class WindowFloating : Window,IFloatingIcon
    {
        double paddingToHide = 25;
        double hiddenThickness = 3;
        DirectionClosedTo directionClosedTo;
        public Image FloatingIcon
        {
            get => icon;
        }
        public WindowFloating()
        {
            InitializeComponent();
            var mainWindow = new Window();
            mainWindow.Floating(this);
            Show();
            if (!string.IsNullOrEmpty(SettingsPlugin.Default.CustomFloatingIconName))//&&File.Exists(SettingsPlugin.Default.CustomFloatingIconPath))
            {
                //var bitmap = new BitmapImage();
                //bitmap.BeginInit();
                //bitmap.StreamSource = new MemoryStream(File.ReadAllBytes(SettingsPlugin.Default.CustomFloatingIconPath));
                //bitmap.EndInit();
                //icon.Source = bitmap;
                icon.Uri = new Uri(System.IO.Path.Combine(SettingsPlugin.Default.UriResources, SettingsPlugin.Default.CustomFloatingIconName));
            }
            else
            {
                //var path = System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, SettingsPlugin.Default.DefaultFloatingIconPath);
                //icon.Source = new BitmapImage(new Uri(path));
                icon.Uri = new Uri(System.IO.Path.Combine(SettingsPlugin.Default.UriResources, SettingsPlugin.Default.DefaultFloatingIconName));
            }
        }

        private void SetCanvas(double left, double top, double right, double bottom, FrameworkElement element)
        {
            Canvas.SetLeft(element, left);
            Canvas.SetTop(element, top);
            Canvas.SetRight(element, right);
            Canvas.SetBottom(element, bottom);
        }
        private Point LimitPoint(Point point)
        {
            if (point.X < 0)
            {
                point.X = 0;
            }
            else if (point.X + FloatingIcon.ActualWidth > SystemParameters.PrimaryScreenWidth)
            {
                point.X = SystemParameters.PrimaryScreenWidth - FloatingIcon.ActualWidth;
            }
            if (point.Y < 0)
            {
                point.Y = 0;
            }
            else if (point.Y + FloatingIcon.ActualHeight > SystemParameters.PrimaryScreenHeight)
            {
                point.Y = SystemParameters.PrimaryScreenHeight - FloatingIcon.ActualHeight;
            }
            return point;
        }
        private void icon_MouseLeave(object sender, MouseEventArgs e)
        {
            directionClosedTo = FloatingIcon.IsCloseToScreenBorder(paddingToHide);
            if (directionClosedTo == DirectionClosedTo.None)
            {
                return;
            }
            HideToBorder();
        }
        private void hiddenRectangle_MouseEnter(object sender, MouseEventArgs e)
        {
            ShowIcon();
        }
        protected override void OnClosed(EventArgs e)
        {
            icon.Dispose();
            base.OnClosed(e);
        }
        public void ShowIcon()
        {
            hiddenRectangle.Visibility = Visibility.Hidden;
            FloatingIcon.Visibility = Visibility.Visible;
        }
        public void HideToBorder()
        {
            var point = FloatingIcon.TranslatePoint(new Point(0, 0), canvas);
            point = LimitPoint(point);
            FloatingIcon.Visibility = Visibility.Hidden;
            FloatingIcon.RenderTransform = null;
            switch (directionClosedTo)
            {
                case DirectionClosedTo.Left:
                    hiddenRectangle.Width = hiddenThickness;
                    hiddenRectangle.Height = FloatingIcon.ActualHeight;
                    SetCanvas(0, point.Y, double.NaN, double.NaN, hiddenRectangle);
                    SetCanvas(paddingToHide, point.Y, double.NaN, double.NaN, FloatingIcon);
                    break;
                case DirectionClosedTo.Top:
                    hiddenRectangle.Width = FloatingIcon.ActualWidth;
                    hiddenRectangle.Height = hiddenThickness;
                    SetCanvas(point.X, 0, double.NaN, double.NaN, hiddenRectangle);
                    SetCanvas(point.X, paddingToHide, double.NaN, double.NaN, FloatingIcon);
                    break;
                case DirectionClosedTo.Bottom:
                    hiddenRectangle.Width = FloatingIcon.ActualWidth;
                    hiddenRectangle.Height = hiddenThickness;
                    SetCanvas(point.X, double.NaN, double.NaN, 0, hiddenRectangle);
                    SetCanvas(point.X, double.NaN, double.NaN, paddingToHide, FloatingIcon);
                    break;
                default:
                    hiddenRectangle.Width = hiddenThickness;
                    hiddenRectangle.Height = FloatingIcon.ActualHeight;
                    SetCanvas(double.NaN, point.Y, 0, double.NaN, hiddenRectangle);
                    SetCanvas(double.NaN, point.Y, paddingToHide, double.NaN, FloatingIcon);
                    break;
            }
            hiddenRectangle.Visibility = Visibility.Visible;
        }

        public void HideIcon()
        {
            HideToBorder();
            hiddenRectangle.Visibility = Visibility.Hidden;
        }
    }
}
