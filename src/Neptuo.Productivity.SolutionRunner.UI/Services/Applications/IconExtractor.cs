using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace Neptuo.Productivity.SolutionRunner.Services.Applications
{
    public static class IconExtractor
    {
        public static ImageSource Get(string filename)
        {
            Icon icon = Icon.ExtractAssociatedIcon(filename);
            ImageSource imageSource;

            using (Icon i = Icon.FromHandle(icon.ToBitmap().GetHicon()))
                imageSource = Imaging.CreateBitmapSourceFromHIcon(i.Handle, new Int32Rect(0, 0, 32, 32), BitmapSizeOptions.FromEmptyOptions());

            return imageSource;
        }
    }
}
