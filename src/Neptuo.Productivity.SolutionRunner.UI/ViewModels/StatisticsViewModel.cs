using Neptuo.Observables;
using Neptuo.Observables.Collections;
using Neptuo.Productivity.SolutionRunner.Services.Statistics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;

namespace Neptuo.Productivity.SolutionRunner.ViewModels
{
    public class StatisticsViewModel : ObservableObject
    {
        private IEnumerator<Color> colorEnumerator;

        public ObservableCollection<StatisticsItemViewModel> Applications { get; private set; }
        public ObservableCollection<StatisticsItemViewModel> Files { get; private set; }

        public StatisticsViewModel()
        {
            Applications = new ObservableCollection<StatisticsItemViewModel>();
            Files = new ObservableCollection<StatisticsItemViewModel>();
            colorEnumerator = GetColors().GetEnumerator();
        }

        private IEnumerable<Color> GetColors()
        {
            foreach (PropertyInfo propertInfo in typeof(Colors).GetProperties())
            {
                if (propertInfo.PropertyType == typeof(Color))
                    yield return (Color)propertInfo.GetValue(null);
            }
        }

        private Color NextColor()
        {
            if (!colorEnumerator.MoveNext())
            {
                colorEnumerator = GetColors().GetEnumerator();
                colorEnumerator.MoveNext();
            }

            return colorEnumerator.Current;
        }

        public void AddApplication(string path, int count)
        {
            Color nextColor = NextColor();
            Applications.Add(new StatisticsItemViewModel(path, count, nextColor));
        }

        public void AddFile(string path, int count)
        {
            Color nextColor = NextColor();
            Files.Add(new StatisticsItemViewModel(path, count, nextColor));
        }
    }
}
