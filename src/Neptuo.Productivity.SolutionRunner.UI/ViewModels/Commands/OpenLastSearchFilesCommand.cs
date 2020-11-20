using Neptuo;
using Neptuo.Observables.Commands;
using Neptuo.Productivity.SolutionRunner.Services.Execution;
using Neptuo.Productivity.SolutionRunner.Services.Searching;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace Neptuo.Productivity.SolutionRunner.ViewModels.Commands
{
    public class OpenLastSearchFilesCommand : Command
    {
        private readonly IDiagnosticService searchDiagnostics;
        private readonly ProcessService processes;

        public OpenLastSearchFilesCommand(IDiagnosticService searchDiagnostics, ProcessService processes)
        {
            Ensure.NotNull(searchDiagnostics, "searchDiagnostics");
            Ensure.NotNull(processes, "processes");
            this.searchDiagnostics = searchDiagnostics;
            this.processes = processes;
        }

        public override bool CanExecute()
            => searchDiagnostics.IsAvailable;

        public override void Execute()
        {
            if (CanExecute())
            {
                string content = String.Join(Environment.NewLine, searchDiagnostics.EnumerateFiles());
                if (String.IsNullOrEmpty(content))
                {
                    MessageBox.Show("No files available.");
                    return;
                }

                string temp = Path.GetTempFileName();
                File.WriteAllText(temp, content);
                processes.OpenTextFile(temp);
            }
        }
    }
}
