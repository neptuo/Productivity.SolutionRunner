using Neptuo;
using Neptuo.Observables.Commands;
using Neptuo.Productivity.SolutionRunner.Services.Execution;
using Neptuo.Productivity.SolutionRunner.Services.Logging;
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
    public class OpenLogCommand : Command<string>
    {
        private readonly ILogService logProvider;
        private readonly ProcessService processes;

        public OpenLogCommand(ILogService logProvider, ProcessService processes)
        {
            Ensure.NotNull(logProvider, "logProvider");
            Ensure.NotNull(processes, "processes");
            this.logProvider = logProvider;
            this.processes = processes;
        }

        public override bool CanExecute(string filePath)
            => true;

        public override void Execute(string filePath)
        {
            if (CanExecute(filePath))
            {
                if (filePath == null)
                {
                    MessageBox.Show("No errors");
                    return;
                }

                string content = logProvider.FindFileContent(filePath);
                if (content == null)
                {
                    MessageBox.Show("No errors");
                    return;
                }

                string temp = Path.GetTempFileName();
                File.WriteAllText(temp, content);
                processes.OpenTextFile(temp);
            }
        }
    }
}
