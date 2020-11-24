using Neptuo;
using Neptuo.Observables.Commands;
using Neptuo.Productivity.SolutionRunner.Services.Execution;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Neptuo.Productivity.SolutionRunner.ViewModels.Commands
{
    public class OpenUrlCommand : Command
    {
        private readonly ProcessService processes;
        private readonly string url;

        public OpenUrlCommand(ProcessService processes, string url)
        {
            Ensure.NotNull(processes, "processes");
            Ensure.NotNull(url, "url");
            this.processes = processes;
            this.url = url;
        }

        public override bool CanExecute()
            => true;

        public override void Execute()
            => processes.OpenUrl(url);
    }
}
