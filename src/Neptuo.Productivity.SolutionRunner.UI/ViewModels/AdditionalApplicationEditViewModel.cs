using Neptuo.Observables;
using Neptuo.Observables.Collections;
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
    public class AdditionalApplicationEditViewModel : ObservableObject, CreateAdditionalApplicationCommand.IContainer, EditAdditionalApplicationCommand.IContainer, RemoveAdditionalApplicationCommand.IContainer
    {
        private readonly INavigator navigator;

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
                }
            }
        }

        private SaveApplicationCommand saveCommand;
        public ICommand SaveCommand
        {
            get { return saveCommand; }
        }

        public ObservableCollection<AdditionalApplicationListViewModel> Commands { get; private set; }

        public ICommand RemoveAdditionalApplicationCommand { get; private set; }
        public ICommand EditAdditionalApplicationCommand { get; private set; }
        public ICommand CreateAdditionalApplicationCommand { get; private set; }

        public AdditionalApplicationEditViewModel(INavigator navigator, AdditionalApplicationModel model, Action<AdditionalApplicationModel> onSaved)
        {
            Commands = new ObservableCollection<AdditionalApplicationListViewModel>();

            if (model != null)
            {
                IsNameChanged = true;
                Name = model.Name;
                Path = model.Path;
                Arguments = model.Arguments;
                HotKey = model.HotKey == Key.None 
                    ? null 
                    : new KeyViewModel(model.HotKey, ModifierKeys.None);

                Commands.AddRange(model.Commands.Select(m => new AdditionalApplicationListViewModel(m)));
            }

            saveCommand = new SaveApplicationCommand(this, model, onSaved);

            EditAdditionalApplicationCommand = new EditAdditionalApplicationCommand(this, navigator);
            RemoveAdditionalApplicationCommand = new RemoveAdditionalApplicationCommand(this);
            CreateAdditionalApplicationCommand = new CreateAdditionalApplicationCommand(this, navigator);
        }

        #region Additional Application Commands

        void CreateAdditionalApplicationCommand.IContainer.Add(AdditionalApplicationListViewModel viewModel)
        {
            if (Commands != null)
                Commands.Add(viewModel);
        }

        void EditAdditionalApplicationCommand.IContainer.Remove(AdditionalApplicationListViewModel viewModel)
        {
            if (Commands != null)
                Commands.Remove(viewModel);
        }

        void RemoveAdditionalApplicationCommand.IContainer.Remove(AdditionalApplicationListViewModel viewModel)
        {
            if (Commands != null)
                Commands.Remove(viewModel);
        }

        #endregion
    }
}
