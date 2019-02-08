using Neptuo.Observables;
using Neptuo.Productivity.SolutionRunner.Services.Execution;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace Neptuo.Productivity.SolutionRunner.ViewModels
{
    public class ApplicationCommandViewModel : ObservableObject, IApplication
    {
        public string Name { get; }
        public string Path { get; }
        public string EmptyArguments { get; }
        public string FileArguments { get; }
        public bool IsAdministratorRequired { get; }
        public Key HotKey { get; }

        private bool isHotKeyActive;
        public bool IsHotKeyActive
        {
            get { return isHotKeyActive; }
            set
            {
                if (isHotKeyActive != value)
                {
                    isHotKeyActive = value;
                    RaisePropertyChanged();
                }
            }
        }

        public ApplicationCommandViewModel(string name, string path, string emptyArguments, string fileArguments, bool isAdministratorRequired, Key hotKey)
        {
            Ensure.NotNullOrEmpty(name, "name");
            Ensure.NotNull(path, "path");
            Name = name;
            Path = path;
            EmptyArguments = emptyArguments;
            FileArguments = fileArguments;
            IsAdministratorRequired = isAdministratorRequired;
            HotKey = hotKey;
        }
    }
}
