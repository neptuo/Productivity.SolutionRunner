using Neptuo.Productivity.SolutionRunner.Services.Configuration;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;

namespace Neptuo.Productivity.SolutionRunner.Views.Converters
{
    public class PathConverter : IValueConverter
    {
        public static ISettings Settings { get; set; }

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            string path = (string)value;

            if (Settings != null && Settings.IsFileNameRemovedFromDisplayedPath)
            {
                path = Path.GetDirectoryName(path);

                if (Settings.IsDisplayedPathTrimmedToLastFolderName)
                    path = Path.GetFileNameWithoutExtension(path);
            }

            return path;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
