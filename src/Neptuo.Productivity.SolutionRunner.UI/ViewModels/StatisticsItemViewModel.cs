using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;

namespace Neptuo.Productivity.SolutionRunner.ViewModels
{
    public class StatisticsItemViewModel
    {
        public string Path { get; private set; }
        public int Count { get; private set; }
        public Brush ColorBrush { get; private set; }

        public StatisticsItemViewModel(string path, int count, Color color)
        {
            Path = path;
            Count = count;
            ColorBrush = new SolidColorBrush(color);
        }
    }
}
