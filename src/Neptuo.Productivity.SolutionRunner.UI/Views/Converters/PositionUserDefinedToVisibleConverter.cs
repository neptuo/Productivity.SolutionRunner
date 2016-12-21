using Neptuo.Productivity.SolutionRunner.Services.Positions;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;

namespace Neptuo.Productivity.SolutionRunner.Views.Converters
{
    public class PositionUserDefinedToVisibleConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            PositionMode mode = (PositionMode)value;
            return mode == PositionMode.UserDefined
                ? Visibility.Visible
                : Visibility.Collapsed;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
