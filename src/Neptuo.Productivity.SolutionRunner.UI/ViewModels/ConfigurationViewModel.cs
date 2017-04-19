using Neptuo.Activators;
using Neptuo.Observables;
using Neptuo.Observables.Collections;
using Neptuo.Productivity.SolutionRunner.Services;
using Neptuo.Productivity.SolutionRunner.Services.Positions;
using Neptuo.Productivity.SolutionRunner.Services.Searching;
using Neptuo.Productivity.SolutionRunner.ViewModels.Commands;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using System.Collections.Specialized;
using Neptuo.Productivity.SolutionRunner.Services.Themes;

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

        private bool isHiddentOnStartup;
        public bool IsHiddentOnStartup
        {
            get { return isHiddentOnStartup; }
            set
            {
                if (isHiddentOnStartup != value)
                {
                    isHiddentOnStartup = value;
                    RaisePropertyChanged();
                }
            }
        }

        private bool isAutoSelectApplicationVersion;
        public bool IsAutoSelectApplicationVersion
        {
            get { return isAutoSelectApplicationVersion; }
            set
            {
                if (isAutoSelectApplicationVersion != value)
                {
                    isAutoSelectApplicationVersion = value;
                    RaisePropertyChanged();
                }
            }
        }

        private VersionViewModel autoSelectApplicationMinimalVersion;
        public VersionViewModel AutoSelectApplicationMinimalVersion
        {
            get { return autoSelectApplicationMinimalVersion; }
            set
            {
                if (autoSelectApplicationMinimalVersion != value)
                {
                    autoSelectApplicationMinimalVersion = value;
                    RaisePropertyChanged();
                }
            }
        }

        private bool isFileNameRemovedFromDisplayedPath;
        public bool IsFileNameRemovedFromDisplayedPath
        {
            get { return isFileNameRemovedFromDisplayedPath; }
            set
            {
                if (isFileNameRemovedFromDisplayedPath != value)
                {
                    isFileNameRemovedFromDisplayedPath = value;
                    RaisePropertyChanged();
                }
            }
        }

        private bool isDisplayedPathTrimmedToLastFolderName;
        public bool IsDisplayedPathTrimmedToLastFolderName
        {
            get { return isDisplayedPathTrimmedToLastFolderName; }
            set
            {
                if (isDisplayedPathTrimmedToLastFolderName != value)
                {
                    isDisplayedPathTrimmedToLastFolderName = value;
                    RaisePropertyChanged();
                }
            }
        }

        private bool isAutoStartup;
        public bool IsAutoStartup
        {
            get { return isAutoStartup; }
            set
            {
                if (isAutoStartup != value)
                {
                    isAutoStartup = value;
                    RaisePropertyChanged();
                }
            }
        }

        private bool isTrayIcon;
        public bool IsTrayIcon
        {
            get { return isTrayIcon; }
            set
            {
                if (isTrayIcon != value)
                {
                    isTrayIcon = value;
                    RaisePropertyChanged();
                }
            }
        }

        private bool isStatisticsCounted;
        public bool IsStatisticsCounted
        {
            get { return isStatisticsCounted; }
            set
            {
                if (isStatisticsCounted != value)
                {
                    isStatisticsCounted = value;
                    RaisePropertyChanged();
                }
            }
        }

        private PositionMode positionMode;
        public PositionMode PositionMode
        {
            get { return positionMode; }
            set
            {
                if (positionMode != value)
                {
                    positionMode = value;
                    RaisePropertyChanged();
                }
            }
        }

        private double? positionLeft;
        public double? PositionLeft
        {
            get { return positionLeft; }
            set
            {
                if (positionLeft != value)
                {
                    positionLeft = value;
                    RaisePropertyChanged();
                }
            }
        }

        private double? positionTop;
        public double? PositionTop
        {
            get { return positionTop; }
            set
            {
                if (positionTop != value)
                {
                    positionTop = value;
                    RaisePropertyChanged();
                }
            }
        }

        private ThemeMode themeMode;
        public ThemeMode ThemeMode
        {
            get { return themeMode; }
            set
            {
                if (themeMode != value)
                {
                    themeMode = value;
                    RaisePropertyChanged();
                }
            }
        }

        public ObservableCollection<IPreferedApplicationViewModel> PreferedApplications { get; set; }

        private IPreferedApplicationViewModel preferedApplication;
        public IPreferedApplicationViewModel PreferedApplication
        {
            get { return preferedApplication; }
            set
            {
                if (preferedApplication != value)
                {
                    preferedApplication = value;
                    RaisePropertyChanged();
                }
            }
        }

        public ObservableCollection<AdditionalApplicationListViewModel> AdditionalApplications { get; set; }
        public ObservableCollection<MainApplicationListViewModel> MainApplications { get; set; }
        public ObservableCollection<VersionViewModel> VsVersions { get; set; }

        public string Version { get; private set; }

        private SaveConfigurationCommand saveCommand;
        public ICommand SaveCommand
        {
            get { return saveCommand; }
        }

        public ICommand RemoveAdditionalApplicationCommand { get; private set; }
        public ICommand EditAdditionalApplicationCommand { get; private set; }
        public ICommand CreateAdditionalApplicationCommand { get; private set; }

        public ConfigurationViewModel(IFactory<SaveConfigurationCommand, ConfigurationViewModel> commandFactory, INavigator navigator)
        {
            string version = Assembly.GetExecutingAssembly().GetCustomAttribute<AssemblyInformationalVersionAttribute>().InformationalVersion;
            Version = String.Format("v{0}", version);

            saveCommand = commandFactory.Create(this);
            EditAdditionalApplicationCommand = new EditAdditionalApplicationCommand(this, navigator);
            RemoveAdditionalApplicationCommand = new RemoveAdditionalApplicationCommand(this);
            CreateAdditionalApplicationCommand = new CreateAdditionalApplicationCommand(this, navigator);
        }
    }
}
