using Neptuo;
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
            Color nextColor = generator.Next();
            Applications.Add(new StatisticsItemViewModel(path, count, nextColor));
        }

        public void AddFile(string path, int count)
        {
            Color nextColor = generator.Next();
            Files.Add(new StatisticsItemViewModel(path, count, nextColor));
        }
    }
}
