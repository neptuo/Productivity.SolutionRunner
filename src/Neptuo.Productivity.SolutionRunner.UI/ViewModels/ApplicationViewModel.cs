﻿using Neptuo.Observables;
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
    public class ApplicationViewModel : ObservableObject, IApplication, IPreferedApplicationViewModel, IApplicationBuilder
    {
        public string Name { get; private set; }
        public string Path { get; private set; }
        public Version Version { get; private set; }
        public string Arguments { get; private set; }
        public bool IsAdministratorRequired { get; private set; }
        public ImageSource Icon { get; private set; }
        public bool IsMain { get; private set; }
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
        
        public ObservableCollection<ApplicationCommandViewModel> Commands { get; private set; }

        public ApplicationViewModel(string name, Version version, string path, string arguments, bool isAdministratorRequired, ImageSource icon, Key hotKey, bool isMain)
        {
            Ensure.NotNullOrEmpty(name, "name");
            Ensure.NotNull(path, "path");
            Ensure.NotNull(icon, "icon");
            Name = name;
            Version = version;
            Path = path;
            Arguments = arguments;
            IsAdministratorRequired = isAdministratorRequired;
            Icon = icon;
            IsMain = isMain;
            HotKey = hotKey;
            Commands = new ObservableCollection<ApplicationCommandViewModel>();
        }
        
        public IApplicationBuilder AddCommand(string name, string path, string arguments, bool isAdministratorRequired, Key hotKey)
        {
            Commands.Add(new ApplicationCommandViewModel(name, path, arguments, isAdministratorRequired, hotKey));
            return this;
        }
    }
}
