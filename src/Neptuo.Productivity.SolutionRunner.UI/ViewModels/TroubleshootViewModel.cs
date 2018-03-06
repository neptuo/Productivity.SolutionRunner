using Neptuo.Observables;
using Neptuo.Observables.Commands;
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

        public List<string> ErrorLogFileNames { get; private set; }

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

        public ICommand OpenErrorLog { get; private set; }
        public ICommand OpenLiveFileList { get; private set; }
        public ICommand OpenCacheFileList { get; private set; }

        public TroubleshootViewModel(ILogService logProvider)
        {
            Ensure.NotNull(logProvider, "logProvider");
            this.logProvider = logProvider;

            ErrorLogFileNames = new List<string>(logProvider.GetFileNames());
            OpenErrorLog = new OpenErrorLogCommand(logProvider);
        }

        private void OpenTempFile(IEnumerable<string> lines)
        {
            string filePath = Path.GetTempFileName();
            File.WriteAllLines(filePath, lines);
            Process.Start(filePath);
        }
    }
}
