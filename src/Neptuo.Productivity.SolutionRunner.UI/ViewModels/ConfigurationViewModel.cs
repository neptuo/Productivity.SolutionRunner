using Neptuo.Activators;
using Neptuo.Observables;
using Neptuo.Productivity.SolutionRunner.Services.Searching;
using Neptuo.Productivity.SolutionRunner.ViewModels.Commands;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace Neptuo.Productivity.SolutionRunner.ViewModels
{
    public class ConfigurationViewModel : ObservableObject
    {
        private string sourceDirectoryPath;
        public string SourceDirectoryPath
        {
            get { return sourceDirectoryPath; }
            set
            {
                if (sourceDirectoryPath != value)
                {
                    sourceDirectoryPath = value;
                    RaisePropertyChanged();
                    saveCommand.RaiseCanExecuteChanged();
                }
            }
        }

        private string preferedApplicationPath;
        public string PreferedApplicationPath
        {
            get { return preferedApplicationPath; }
            set
            {
                if (preferedApplicationPath != value)
                {
                    preferedApplicationPath = value;
                    RaisePropertyChanged();
                }
            }
        }

        private KeyViewModel runKey;
        public KeyViewModel RunKey
        {
            get { return runKey; }
            set
            {
                if (runKey != value)
                {
                    runKey = value;
                    RaisePropertyChanged();
                }
            }
        }

        private FileSearchMode fileSearchMode;
        public FileSearchMode FileSearchMode
        {
            get { return fileSearchMode; }
            set
            {
                if (fileSearchMode != value)
                {
                    fileSearchMode = value;
                    RaisePropertyChanged();
                }
            }
        }

        private int fileSearchCount;
        public int FileSearchCount
        {
            get { return fileSearchCount; }
            set
            {
                if (fileSearchCount != value)
                {
                    fileSearchCount = value;
                    RaisePropertyChanged();
                    saveCommand.RaiseCanExecuteChanged();
                }
            }
        }

        private bool isFileSearchPatternSaved;
        public bool IsFileSearchPatternSaved
        {
            get { return isFileSearchPatternSaved; }
            set
            {
                if (isFileSearchPatternSaved != value)
                {
                    isFileSearchPatternSaved = value;
                    RaisePropertyChanged();
                }
            }
        }

        private bool isLastUsedApplicationSavedAsPrefered;
        public bool IsLastUsedApplicationSavedAsPrefered
        {
            get { return isLastUsedApplicationSavedAsPrefered; }
            set
            {
                if (isLastUsedApplicationSavedAsPrefered != value)
                {
                    isLastUsedApplicationSavedAsPrefered = value;
                    RaisePropertyChanged();
                }
            }
        }

        private bool isDismissedWhenLostFocus;
        public bool IsDismissedWhenLostFocus
        {
            get { return isDismissedWhenLostFocus; }
            set
            {
                if (isDismissedWhenLostFocus != value)
                {
                    isDismissedWhenLostFocus = value;
                    RaisePropertyChanged();
                }
            }
        }

        public string Version { get; private set; }

        private SaveConfigurationCommand saveCommand;
        public ICommand SaveCommand
        {
            get { return saveCommand; }
        }

        public ConfigurationViewModel(IFactory<SaveConfigurationCommand, ConfigurationViewModel> commandFactory)
        {
            Version version = Assembly.GetExecutingAssembly().GetName().Version;
            Version = String.Format("v{0}.{1}.{2}", version.Major, version.Minor, version.Build);

            saveCommand = commandFactory.Create(this);
        }
    }
}
