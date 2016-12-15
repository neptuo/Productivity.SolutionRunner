using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;

namespace Neptuo.Productivity.SolutionRunner.Services.Colors
{
    public class NamedColorGenerator : IColorGenerator
    {
        private IEnumerator<Color> colorEnumerator;

        public NamedColorGenerator()
        {
            colorEnumerator = GetColors().GetEnumerator();
        }

        private IEnumerable<Color> GetColors()
        {
            foreach (PropertyInfo propertInfo in typeof(System.Windows.Media.Colors).GetProperties())
            {
                if (propertInfo.PropertyType == typeof(Color))
                {
                    Color color = (Color)propertInfo.GetValue(null);
                    if (color == System.Windows.Media.Colors.Black)
                        continue;

                    yield return color;
                }
            }
        }

        public Color Next()
        {
            if (!colorEnumerator.MoveNext())
            {
                colorEnumerator = GetColors().GetEnumerator();
                colorEnumerator.MoveNext();
            }

            return colorEnumerator.Current;
        }
    }
}
