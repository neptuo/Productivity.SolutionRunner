using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;

namespace Neptuo.Productivity.SolutionRunner.Services.Colors
{
    public class RandomColorGenerator : IColorGenerator
    {
        private int index = 20;

        private Color RandomAtIndex(int index)
        {
            byte red = 0;
            byte green = 0;
            byte blue = 0;

            for (int t = 0; t <= index / 8; t++)
            {
                int index_a = (index + t) % 8;
                int index_b = index_a / 2;

                // Color writers, take on values of 0 and 1
                int color_red = index_a % 2;
                int color_blue = index_b % 2;
                int color_green = ((index_b + 1) % 3) % 2;

                int add = 255 / (t + 1);

                red = (byte)(red + color_red * add);
                green = (byte)(green + color_green * add);
                blue = (byte)(blue + color_blue * add);
            }

            Color color = Color.FromRgb(red, green, blue);
            return color;
        }

        public Color Next()
        {
            Color next = RandomAtIndex(index);
            index++;
            return next;
        }
    }
}
