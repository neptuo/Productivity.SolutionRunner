﻿using Neptuo.Observables;
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
    public class AdditionalApplicationEditViewModel : ObservableObject
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

        private SaveApplicationCommand saveCommand;
        public ICommand SaveCommand
        {
            get { return saveCommand; }
        }

        public AdditionalApplicationEditViewModel(AdditionalApplicationModel model, Action<AdditionalApplicationModel> onSaved)
        {
            if (model != null)
            {
                IsNameChanged = true;
                Name = model.Name;
                Path = model.Path;
                Arguments = model.Arguments;
            }

            saveCommand = new SaveApplicationCommand(this, model, onSaved);
        }
    }
}