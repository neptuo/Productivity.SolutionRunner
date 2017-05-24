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
        public string Name { get; private set; }
        public string Path { get; private set; }
        public string Arguments { get; private set; }
        public Key HotKey { get; private set; }

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

        public ApplicationCommandViewModel(string name, string path, string arguments, Key hotKey)
        {
            Ensure.NotNullOrEmpty(name, "name");
            Ensure.NotNull(path, "path");
            Name = name;
            Path = path;
            Arguments = arguments;
            HotKey = hotKey;
        }
    }
}
