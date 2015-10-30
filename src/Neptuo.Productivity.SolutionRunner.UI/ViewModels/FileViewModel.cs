using Neptuo.Observables;
using Neptuo.Productivity.SolutionRunner.Services;
using Neptuo.Productivity.SolutionRunner.ViewModels.Commands;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace Neptuo.Productivity.SolutionRunner.ViewModels
{
    public class FileViewModel : ObservableObject
    {
        public string Name { get; private set; }
        public string Path { get; private set; }

        private bool isPinned;
        public bool IsPinned
        {
            get { return isPinned; }
            set
            {
                if (isPinned != value)
                {
                    isPinned = value;
                    RaisePropertyChanged();
                    EventManager.RaiseFilePinned(this);
                }
            }
        }

        private readonly PinCommand pinCommand;
        public ICommand PinCommand
        {
            get { return pinCommand; }
        }

        private readonly UnPinCommand unPinCommand;
        public ICommand UnPinCommand
        {
            get { return unPinCommand; }
        }
        
        public FileViewModel(string name, string path, bool isPinned = false)
        {
            Ensure.NotNullOrEmpty(name, "name");
            Ensure.NotNullOrEmpty(path, "path");
            Name = name;
            Path = path;
            IsPinned = isPinned;

            this.pinCommand = new PinCommand(this);
            this.unPinCommand = new UnPinCommand(this);
        }

        public override bool Equals(object obj)
        {
            FileViewModel other = obj as FileViewModel;
            if(other == null)
                return false;

            return Path.Equals(other.Path);
        }

        public override int GetHashCode()
        {
            return 151 ^ Path.GetHashCode();
        }
    }
}
