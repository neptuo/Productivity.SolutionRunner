using Neptuo.Observables.Collections;
using Neptuo.Productivity.SolutionRunner.Services.Applications;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using System.Windows.Media;

namespace Neptuo.Productivity.SolutionRunner.ViewModels
{
    public class VsVersionCollection : ObservableCollection<VersionViewModel>, IApplicationCollection
    {
        public VsVersionCollection()
        {
            Add(VersionViewModel.Empty());
        }

        public IApplicationBuilder Add(string name, string path, string emptyArguments, string fileArguments, bool isAdministratorRequired, ImageSource icon, Key hotKey, bool isMain)
        {
            return new EmptyAplicationBuilder();
        }

        public IApplicationBuilder Add(string name, Version version, string path, string emptyArguments, string fileArguments, bool isAdministratorRequired, ImageSource icon, Key hotKey, bool isMain)
        {
            if (isMain && !name.Contains("Code"))
                Add(new VersionViewModel(version));

            return new EmptyAplicationBuilder();
        }
    }
}
