using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;
using System.Windows.Media;

namespace Suhoro.WindowsTool.Core.Converters
{
    public class ConverterColorToBrush : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var color = (System.Drawing.Color)value;
            return new SolidColorBrush(Color.FromArgb(color.A,color.R,color.G,color.B));

        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var brush=value as SolidColorBrush;
            return System.Drawing.Color.FromArgb(brush.Color.A, brush.Color.R,brush.Color.G,brush.Color.B);
        }
    }
}
