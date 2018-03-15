using Neptuo.Observables.Commands;
using Neptuo.Productivity.SolutionRunner.Services;
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
        private string FindPath()
        {
            IsolatedStorageFile storage = SequenceIsolatedFile.GetStorage();
            FieldInfo fieldInfo = storage.GetType().GetField("m_RootDir", BindingFlags.NonPublic | BindingFlags.Instance);
            if (fieldInfo == null)
                return null;

            string value = (string)fieldInfo.GetValue(storage);
            if (value == null)
                return null;

            return value;
        }

        public override bool CanExecute()
        {
            return Directory.Exists(FindPath());
        }

        public override void Execute()
        {
            string path = FindPath();
            if (Directory.Exists(path))
                Process.Start(path);
        }
    }
}
