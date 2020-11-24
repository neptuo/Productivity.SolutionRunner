using Neptuo;
using Neptuo.Observables;
using Neptuo.Productivity.SolutionRunner.Services;
using Neptuo.Productivity.SolutionRunner.Services.Execution;
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
    public class AboutViewModel : ObservableModel
    {
        public string Version { get; }

        public ICommand OpenDocumentation { get; }
        public ICommand OpenSourceCode { get; }
        public ICommand OpenNewIssue { get; }

        public AboutViewModel(ApplicationVersion applicationVersion, ProcessService processes)
        {
            Ensure.NotNull(applicationVersion, "applicationVersion");
            Ensure.NotNull(processes, "processes");

            Version = applicationVersion.GetDisplayString();

            OpenDocumentation = new OpenUrlCommand(processes, "http://www.neptuo.com/project/desktop/solutionrunner");
            OpenSourceCode = new OpenUrlCommand(processes, "https://github.com/neptuo/Productivity.SolutionRunner");
            OpenNewIssue = new OpenUrlCommand(processes, "https://github.com/neptuo/Productivity.SolutionRunner/issues/new");
        }
    }
}
