using Neptuo.Observables;
using Neptuo.Observables.Collections;
using Neptuo.Productivity.SolutionRunner.Services.Logging;
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
    public class TroubleshootViewModel : ObservableObject
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
        public ICommand OpenLiveFileList { get; private set; }
        public ICommand OpenCacheFileList { get; private set; }

        public TroubleshootViewModel(ILogService logProvider)
        {
            Ensure.NotNull(logProvider, "logProvider");
            this.logProvider = logProvider;

            Logs = new ObservableCollection<LogModel>(logProvider.GetFileNames());
            OpenLog = new OpenLogCommand(logProvider);
        }

        public void ReloadLogs()
        {
            Logs.Clear();
            Logs.AddRange(logProvider.GetFileNames());
        }
    }
}
