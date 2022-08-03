using Microsoft.Xaml.Behaviors;
using Suhoro.WindowsTool.Core.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;

namespace Suhoro.WindowsTool.Core.Behaviors
{
    public class MouseDrogMovingBehavior: Behavior<FrameworkElement>
    {
        Point mouseBefore=new Point(0,0);
        protected override void OnAttached()
        {
            base.OnAttached();
            AssociatedObject.MouseLeftButtonDown += AssociatedObject_MouseLeftButtonDown;
            AssociatedObject.MouseLeftButtonUp += AssociatedObject_MouseLeftButtonUp;
            AssociatedObject.MouseMove += AssociatedObject_MouseMove;
        }
        protected override void OnDetaching()
        {
            base.OnDetaching();
            AssociatedObject.MouseLeftButtonDown -= AssociatedObject_MouseLeftButtonDown;
            AssociatedObject.MouseLeftButtonUp -= AssociatedObject_MouseLeftButtonUp;
            AssociatedObject.MouseMove -= AssociatedObject_MouseMove;
        }

        private void AssociatedObject_MouseMove(object sender, MouseEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed)
            {
                var mousePosition = e.GetPosition(AssociatedObject);
                TranslateTransform tt = AssociatedObject.GetTransform<TranslateTransform>();
                tt.X += mousePosition.X - mouseBefore.X;
                tt.Y += mousePosition.Y - mouseBefore.Y;
            }
        }
        private void AssociatedObject_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            AssociatedObject.ReleaseMouseCapture();
            AssociatedObject.Focus();
        }
        private void AssociatedObject_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            AssociatedObject.CaptureMouse();
            mouseBefore = e.GetPosition(AssociatedObject);
        }
    }
}