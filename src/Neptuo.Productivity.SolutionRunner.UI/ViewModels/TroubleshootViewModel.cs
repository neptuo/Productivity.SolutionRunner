using Neptuo.Observables;
using Neptuo.Observables.Collections;
using Neptuo.Observables.Commands;
using Neptuo.Productivity.SolutionRunner.Services.Execution;
using Neptuo.Productivity.SolutionRunner.Services.Logging;
using Neptuo.Productivity.SolutionRunner.Services.Searching;
using Neptuo.Productivity.SolutionRunner.ViewModels.Commands;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace Neptuo.Productivity.SolutionRunner.ViewModels
{
    public class TroubleshootViewModel : ObservableModel
    {
        private readonly ILogService logProvider;

        public ObservableCollection<LogModel> Logs { get; private set; }

        private int liveFileCount;
        public int LiveFileCount
        {
            get { return liveFileCount; }
            set
            {
                if (liveFileCount != value)
                {
                    liveFileCount = value;
                    RaisePropertyChanged();
                }
            }
        }

        public ICommand OpenLog { get; private set; }
        public ICommand OpenIsolatedFolder { get; private set; }
        public ICommand OpenLastSearchFiles { get; private set; }
        public ICommand FlushLogMessages { get; private set; }

        public TroubleshootViewModel(ILogService logProvider, IDiagnosticService searchDiagnostics, FileLogBatchFactory executorFactory, ProcessService processes)
        {
            Ensure.NotNull(logProvider, "logProvider");
            this.logProvider = logProvider;

            Logs = new ObservableCollection<LogModel>(logProvider.GetFileNames());
            OpenLog = new OpenLogCommand(logProvider, processes);
            OpenIsolatedFolder = new OpenIsolatedFolderCommand(processes);
            OpenLastSearchFiles = new OpenLastSearchFilesCommand(searchDiagnostics, processes);
            FlushLogMessages = new DelegateCommand(executorFactory.Flush);
        }

        public void ReloadLogs()
        {
            Logs.Clear();
            Logs.AddRange(logProvider.GetFileNames());
        }
    }
}
