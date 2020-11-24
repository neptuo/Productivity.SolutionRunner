using Neptuo;
using Neptuo.Activators;
using Neptuo.Observables;
using Neptuo.Observables.Commands;
using Neptuo.Productivity.SolutionRunner.Services.Statistics;
using Neptuo.Productivity.SolutionRunner.ViewModels.Commands;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace Neptuo.Productivity.SolutionRunner.ViewModels
{
    public class StatisticsWithImportViewModel : ObservableModel
    {
        private StatisticsRootViewModel root;
        public StatisticsRootViewModel Root
        {
            get { return root; }
            set
            {
                if (root != value)
                {
                    root = value;
                    RaisePropertyChanged();
                }
            }
        }

        public ICommand Reload { get; }
        public ICommand Import { get; }
        public ICommand Export { get; }

        public StatisticsWithImportViewModel(IFactory<StatisticsRootViewModel> rootFactory, ICountingImporter importer)
        {
            Ensure.NotNull(rootFactory, "rootFactory");
            Ensure.NotNull(importer, "importer");

            Reload = new DelegateCommand(() => Root = rootFactory.Create());
            Import = new ImportStatisticsCommand(this, importer);
            Export = new ExportStatisticsCommand(importer);
        }
    }
}
