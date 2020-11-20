using Neptuo;
using Neptuo.Observables.Commands;
using Neptuo.Productivity.SolutionRunner.Services;
using Neptuo.Productivity.SolutionRunner.Services.Execution;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.IO.IsolatedStorage;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Neptuo.Productivity.SolutionRunner.ViewModels.Commands
{
    public class OpenIsolatedFolderCommand : Command
    {
        private readonly ProcessService processes;
        private FieldInfo fieldInfo;

        public OpenIsolatedFolderCommand(ProcessService processes)
        {
            Ensure.NotNull(processes, "processes");
            this.processes = processes;
        }

        private bool EnsureFieldInfo()
        {
            if (fieldInfo == null)
                fieldInfo = typeof(IsolatedStorageFile).GetField("_rootDirectory", BindingFlags.NonPublic | BindingFlags.Instance);

            return fieldInfo != null;
        }

        private string FindPath()
        {
            IsolatedStorageFile storage = SequenceIsolatedFile.GetStorage();
            if (!EnsureFieldInfo())
                return null;

            string value = (string)fieldInfo.GetValue(storage);
            if (value == null)
                return null;

            return value;
        }

        public override bool CanExecute() 
            => Directory.Exists(FindPath());

        public override void Execute()
        {
            string path = FindPath();
            processes.OpenFolder(path);
        }
    }
}
