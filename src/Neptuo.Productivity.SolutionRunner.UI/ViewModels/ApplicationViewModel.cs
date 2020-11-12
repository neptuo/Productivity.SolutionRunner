using Neptuo.Observables;
using Neptuo.Observables.Collections;
using Neptuo.Productivity.SolutionRunner.Services.Applications;
using Neptuo.Productivity.SolutionRunner.Services.Execution;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using System.Windows.Media;

namespace Neptuo.Productivity.SolutionRunner.ViewModels
{
    public class ApplicationViewModel : ApplicationCommandViewModel, IPreferedApplicationViewModel, IApplicationBuilder
    {
        public Version Version { get; }
        public ImageSource Icon { get; }
        public bool IsMain { get; }

        public ObservableCollection<ApplicationCommandViewModel> Commands { get; }

        public ApplicationViewModel(string name, Version version, string path, string emptyArguments, string fileArguments, bool isAdministratorRequired, bool isApplicationWindowShown, ImageSource icon, Key hotKey, bool isMain)
            : base(name, path, emptyArguments, fileArguments, isAdministratorRequired, isApplicationWindowShown, hotKey)
        {
            Ensure.NotNullOrEmpty(name, "name");
            Ensure.NotNull(path, "path");
            Ensure.NotNull(icon, "icon");
            Version = version;
            Icon = icon;
            IsMain = isMain;
            Commands = new ObservableCollection<ApplicationCommandViewModel>();
        }
        
        public IApplicationBuilder AddCommand(string name, string path, string emptyArguments, string fileArguments, bool isAdministratorRequired, bool isApplicationWindowShown, Key hotKey)
        {
            Commands.Add(new ApplicationCommandViewModel(name, path, emptyArguments, fileArguments, isAdministratorRequired, isApplicationWindowShown, hotKey));
            return this;
        }
    }
}
