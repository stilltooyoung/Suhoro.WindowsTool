using System;
using System.Linq;
using System.Windows;
using System.Windows.Media;

namespace Suhoro.WindowsTool.Core.Utils
{
    public static class FrameworkElementExtensions
    {
        public static TTransform GetTransform<TTransform>(this FrameworkElement e) where TTransform : Transform, new()
        {
            TTransform tt;
            if (e.RenderTransform == null)
            {
                tt = new TTransform();
                e.RenderTransform = tt;
            }
            else if (e.RenderTransform is TTransform rt)
            {
                tt = rt;
            }
            else if (e.RenderTransform is TransformGroup group)
            {
                var t = group.Children.FirstOrDefault(c => c is TTransform) as TTransform;
                if (t == null)
                {
                    tt = new TTransform();
                    group.Children.Add(tt);
                }
                else
                {
                    tt = t;
                }
            }
            else
            {
                group = new TransformGroup();
                group.Children.Add(e.RenderTransform);
                tt = new TTransform();
                group.Children.Add(tt);
                e.RenderTransform = group;
            }

            return tt;
        }

        public static DirectionClosedTo IsCloseToScreenBorder(this FrameworkElement e,double paddingToBorder)
        {
            var window= Window.GetWindow(e);
            var point = e.TranslatePoint(new Point(0,0),window);

            var left=point.X;
            var top=point.Y;
            var right= SystemParameters.PrimaryScreenWidth - point.X-e.ActualWidth;
            var bottom=SystemParameters.PrimaryScreenHeight-point.Y-e.ActualHeight;

            var distances =new double[] { left, top, right, bottom };
            var min=distances.Select((d,i)=>new {distance=d,direction=(DirectionClosedTo)(i + 1) })
                .OrderBy(x=>x.distance).ThenBy(x=>x.direction).First();
            if (min.distance<paddingToBorder)
            {
                return min.direction;
            }
            return DirectionClosedTo.None;
        }

        public enum DirectionClosedTo
        {
            None,
            Left,
            Top,
            Right,
            Bottom
        }
    }
}
