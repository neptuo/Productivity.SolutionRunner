using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace Neptuo.Productivity.SolutionRunner.Services.Applications
{
    /// <summary>
    /// A helper for extracting icons from files (or associated with files).
    /// </summary>
    public static class IconExtractor
    {
        /// <summary>
        /// Gets a 32x32 icon for <paramref name="filename"/>.
        /// </summary>
        /// <param name="filename">The path to a file.</param>
        /// <returns>The 32x32 icon for <paramref name="filename"/>.</returns>
        public static ImageSource Get(string filename)
        {
            Icon icon = Icon.ExtractAssociatedIcon(filename);
            ImageSource imageSource;

            if (filename == @"C:\Program Files (x86)\Microsoft Visual Studio 14.0\Common7\IDE\devenv.exe")
            {
                using (FileStream file = File.Create(@"C:\Temp\Vs14.ico"))
                {
                    icon.Save(file);
                }
            }

            using (Icon i = Icon.FromHandle(icon.ToBitmap().GetHicon()))
            {
                imageSource = Imaging.CreateBitmapSourceFromHIcon(
                    i.Handle,
                    new Int32Rect(0, 0, 32, 32),
                    BitmapSizeOptions.FromWidthAndHeight(32, 32)
                );
            }

            return imageSource;
        }
    }
}
