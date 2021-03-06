﻿using Neptuo.Observables;
using Neptuo.Observables.Collections;
using Neptuo.Productivity.SolutionRunner.Services;
using Neptuo.Productivity.SolutionRunner.Services.Applications;
using Neptuo.Productivity.SolutionRunner.ViewModels.Commands;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace Neptuo.Productivity.SolutionRunner.ViewModels
{
    public class AdditionalApplicationEditViewModel : ObservableModel, IApplicationViewModel
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

                    SetIconFromPath();
                }
            }
        }

        private string iconData;
        public string IconData
        {
            get { return iconData; }
            set
            {
                if (iconData != value)
                {
                    iconData = value;
                    RaisePropertyChanged();

                    if (iconData != null)
                        SetIconFromIconData();
                    else
                        SetIconFromPath();
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
        public ICommand CreateCommand { get; private set; }
        public ICommand SelectCustomIcon { get; }
        public ICommand ClearCustomIcon { get; }

        public AdditionalApplicationEditViewModel(INavigator navigator, AdditionalApplicationModel model, Action<AdditionalApplicationModel> onSaved)
        {
            Commands = new ObservableCollection<AdditionalApplicationListViewModel>();

            if (model != null)
            {
                IsNameChanged = true;
                Name = model.Name;
                Path = model.Path;
                Arguments = model.Arguments;
                IconData = model.IconData;
                IsAdministratorRequired = model.IsAdministratorRequired;
                IsApplicationWindowShown = model.IsApplicationWindowShown;
                HotKey = model.HotKey == Key.None
                    ? null
                    : new KeyViewModel(model.HotKey, ModifierKeys.None);

                Commands.AddRange(model.Commands.Select(m => new AdditionalApplicationListViewModel(m)));
            }
            else
            {
                IsApplicationWindowShown = true;
            }

            saveCommand = new SaveApplicationCommand(this, model, onSaved);

            EditAdditionalApplicationCommand = new EditAdditionalCommandCommand(this, navigator);
            RemoveAdditionalApplicationCommand = new RemoveAdditionalCommandCommand(this);
            CreateCommand = new CreateAdditionalCommandCommand(this, navigator);
            SelectCustomIcon = new LoadIconFromFileCommand(base64 => IconData = base64);
            ClearCustomIcon = new ClearCustomIconCommand(this);
        }

        private void SetIconFromPath()
        {
            if (IconData == null)
            {
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

        private void SetIconFromIconData()
        {
            if (iconData != null)
                Icon = Base64ImageCoder.GetImageFromString(iconData);
        }

        public AdditionalApplicationModel ToModel()
        {
            return new AdditionalApplicationModel(
                Name,
                Path,
                Arguments,
                IconData,
                IsAdministratorRequired,
                IsApplicationWindowShown,
                HotKey.GetKey(),
                Commands
                    .Select(vm => vm.Model)
                    .ToList()
            );
        }
    }
}
