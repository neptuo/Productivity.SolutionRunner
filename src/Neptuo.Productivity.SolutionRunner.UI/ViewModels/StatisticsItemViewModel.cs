using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;

namespace Neptuo.Productivity.SolutionRunner.ViewModels
{
    public class StatisticsItemViewModel
    {
        public string Name { get; private set; }
        public string Path { get; private set; }
        public int Count { get; private set; }
        public Brush ColorBrush { get; private set; }

        public StatisticsItemViewModel(string path, int count, Color color)
        {
            Name = System.IO.Path.GetFileName(path);
            Path = path;
            Count = count;
            ColorBrush = new SolidColorBrush(color);
        }
    }
}
