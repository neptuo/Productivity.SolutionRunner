using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace Neptuo.Productivity.SolutionRunner.Services.Applications
{
    public static class Base64ImageCoder
    {
        public static ImageSource GetImageFromString(string data)
        {
            byte[] imageData = Convert.FromBase64String(data);

            BitmapImage image = new BitmapImage();
            image.BeginInit();
            image.StreamSource = new MemoryStream(imageData);
            image.EndInit();

            return image;
        }

        public static string GetIconFromFile(string filePath)
        {
            Icon icon = Icon.ExtractAssociatedIcon(filePath);
            using (var imageStream = new MemoryStream())
            {
                icon.ToBitmap().Save(imageStream, ImageFormat.Png);
                imageStream.Position = 0;

                string data = Convert.ToBase64String(imageStream.ToArray());
                return data;
            }
        }
    }
}
