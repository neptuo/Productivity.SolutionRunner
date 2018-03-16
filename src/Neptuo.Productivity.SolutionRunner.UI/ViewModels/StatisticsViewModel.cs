using Neptuo.Observables;
using Neptuo.Observables.Collections;
using Neptuo.Productivity.SolutionRunner.Services.Colors;
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
        private readonly IColorGenerator generator;

        public ObservableCollection<StatisticsItemViewModel> Applications { get; private set; }
        public ObservableCollection<StatisticsItemViewModel> Files { get; private set; }

        public StatisticsViewModel(IColorGenerator generator)
        {
            Ensure.NotNull(generator, "generator");
            this.generator = generator;

            Applications = new ObservableCollection<StatisticsItemViewModel>();
            Files = new ObservableCollection<StatisticsItemViewModel>();
        }

        public void AddApplication(string path, int count)
        {
            string safePath = path.ToLowerInvariant();
            StatisticsItemViewModel item = Applications.FirstOrDefault(a => a.Path.ToLowerInvariant() == safePath);
            if (item == null)
                Applications.Add(new StatisticsItemViewModel(path, count, generator.Next()));
            else
                item.Count += count;
        }

        public void AddFile(string path, int count)
        {
            string safePath = path.ToLowerInvariant();
            StatisticsItemViewModel item = Files.FirstOrDefault(a => a.Path.ToLowerInvariant() == safePath);
            if (item == null)
                Files.Add(new StatisticsItemViewModel(path, count, generator.Next()));
            else
                item.Count += count;
        }
    }
}
