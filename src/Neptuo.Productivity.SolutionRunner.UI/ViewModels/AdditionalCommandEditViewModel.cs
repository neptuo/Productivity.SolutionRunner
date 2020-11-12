using Neptuo.Observables;
using Neptuo.Productivity.SolutionRunner.Services;
using Neptuo.Productivity.SolutionRunner.Services.Applications;
using Neptuo.Productivity.SolutionRunner.ViewModels.Commands;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using System.Windows.Media;

namespace Neptuo.Productivity.SolutionRunner.ViewModels
{
    public class AdditionalCommandEditViewModel : ObservableObject, IApplicationViewModel
    {
        public bool IsNameChanged { get; set; }

        private string name;
        public string Name
        {
            get { return name; }
            set
            {
                if (name != value)
                {
                    name = value;
                    RaisePropertyChanged();
                }
            }
        }

        private string path;
        public string Path
        {
            get { return path; }
            set
            {
                if (path != value)
                {
                    path = value;
                    RaisePropertyChanged();

                    if (saveCommand != null)
                        saveCommand.RaiseCanExecuteChanged();

                    if (System.IO.File.Exists(path))
                    {
                        if (!IsNameChanged)
                            Name = System.IO.Path.GetFileNameWithoutExtension(path);

                        Icon = IconExtractor.Get(path);
                    }
                    else
                    {
                        if (!IsNameChanged)
                            Name = null;

                        Icon = null;
                    }
                }
            }
        }

        private string arguments;
        public string Arguments
        {
            get { return arguments; }
            set
            {
                if (arguments != value)
                {
                    arguments = value;
                    RaisePropertyChanged();
                }
            }
        }

        private bool isAdministratorRequired;
        public bool IsAdministratorRequired
        {
            get { return isAdministratorRequired; }
            set
            {
                if (isAdministratorRequired != value)
                {
                    isAdministratorRequired = value;
                    RaisePropertyChanged();
                }
            }
        }

        private bool isApplicationWindowShown;
        public bool IsApplicationWindowShown
        {
            get { return isApplicationWindowShown; }
            set
            {
                if (isApplicationWindowShown != value)
                {
                    isApplicationWindowShown = value;
                    RaisePropertyChanged();
                }
            }
        }

        private ImageSource icon;
        public ImageSource Icon
        {
            get { return icon; }
            set
            {
                if (icon != value)
                {
                    icon = value;
                    RaisePropertyChanged();
                }
            }
        }

        private KeyViewModel hotKey;
        public KeyViewModel HotKey
        {
            get { return hotKey; }
            set
            {
                if (hotKey != value)
                {
                    hotKey = value;
                    RaisePropertyChanged();

                    if (saveCommand != null)
                        saveCommand.RaiseCanExecuteChanged();
                }
            }
        }

        private SaveApplicationCommand saveCommand;
        public ICommand SaveCommand => saveCommand;

        public ICommand SelectCustomIcon { get; } // null

        public AdditionalCommandEditViewModel(INavigator navigator, AdditionalApplicationModel model, Action<AdditionalApplicationModel> onSaved)
        {
            if (model != null)
            {
                IsNameChanged = true;
                Name = model.Name;
                Path = model.Path;
                Arguments = model.Arguments;
                IsAdministratorRequired = model.IsAdministratorRequired;
                IsApplicationWindowShown = model.IsApplicationWindowShown;
                HotKey = model.HotKey == Key.None
                    ? null
                    : new KeyViewModel(model.HotKey, ModifierKeys.None);
            }

            saveCommand = new SaveApplicationCommand(this, model, onSaved, true);
        }

        public AdditionalApplicationModel ToModel()
        {
            return new AdditionalApplicationModel(
                Name,
                Path,
                Arguments,
                null,
                IsAdministratorRequired,
                IsApplicationWindowShown,
                HotKey.GetKey()
            );
        }
    }
}
