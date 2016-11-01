using Neptuo.Observables;
using Neptuo.Productivity.SolutionRunner.Services.Execution;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;

namespace Neptuo.Productivity.SolutionRunner.ViewModels
{
    public class ApplicationViewModel : ObservableObject, IApplication
    {
        public string Name { get; private set; }
        public string Path { get; private set; }
        public Version Version { get; private set; }
        public string Arguments { get; private set; }
        public ImageSource Icon { get; private set; }
        public bool IsMain { get; private set; }

        public ApplicationViewModel(string name, Version version, string path, string arguments, ImageSource icon, bool isMain)
        {
            Ensure.NotNullOrEmpty(name, "name");
            Ensure.NotNull(path, "path");
            Ensure.NotNull(icon, "icon");
            Name = name;
            Version = version;
            Path = path;
            Arguments = arguments;
            Icon = icon;
            IsMain = isMain;
        }
    }
}
