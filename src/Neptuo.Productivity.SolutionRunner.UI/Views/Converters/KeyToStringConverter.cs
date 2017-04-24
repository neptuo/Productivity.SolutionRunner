using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;
using System.Windows.Input;

namespace Neptuo.Productivity.SolutionRunner.Views.Converters
{
    public class KeyToStringConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            Key? key = value as Key?;
            if (key == null)
                return value;

            switch (key.Value)
            {
                case Key.None:
                    return string.Empty;
                case Key.D0:
                case Key.D1:
                case Key.D2:
                case Key.D3:
                case Key.D4:
                case Key.D5:
                case Key.D6:
                case Key.D7:
                case Key.D8:
                case Key.D9:
                    return key.Value.ToString().Substring(1);
                case Key.LWin:
                    return "Left Windows";
                case Key.RWin:
                    return "Right Windows";
                case Key.NumPad0:
                case Key.NumPad1:
                case Key.NumPad2:
                case Key.NumPad3:
                case Key.NumPad4:
                case Key.NumPad5:
                case Key.NumPad6:
                case Key.NumPad7:
                case Key.NumPad8:
                case Key.NumPad9:
                    return key.Value.ToString().Substring(6);
                case Key.LeftShift:
                    return "Left Shift";
                case Key.RightShift:
                    return "Right Shift";
                case Key.LeftCtrl:
                    return "Left Control";
                case Key.RightCtrl:
                    return "Right Control";
                case Key.LeftAlt:
                    return "Left Alt";
                case Key.RightAlt:
                    return "Right Alt";
                default:
                    return key.Value.ToString();
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
