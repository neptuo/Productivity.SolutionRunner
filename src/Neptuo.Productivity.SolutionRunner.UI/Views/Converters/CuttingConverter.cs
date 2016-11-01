﻿using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;

namespace Neptuo.Productivity.SolutionRunner.Views.Converters
{
    public class CuttingConverter : IValueConverter
    {
        public int EdgeValue { get; set; }

        public object LowerValue { get; set; }
        public object GreaterValue { get; set; }

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            int intValue = (int)value;
            if (intValue > EdgeValue)
                return GreaterValue;

            return LowerValue;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}