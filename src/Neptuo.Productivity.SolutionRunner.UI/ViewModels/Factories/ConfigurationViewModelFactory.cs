using Neptuo.Activators;
using Neptuo.Observables.Collections;
using Neptuo.Productivity.SolutionRunner.Services;
using Neptuo.Productivity.SolutionRunner.Services.Applications;
using Neptuo.Productivity.SolutionRunner.Services.Configuration;
using Neptuo.Productivity.SolutionRunner.Services.StartupShortcuts;
using Neptuo.Productivity.SolutionRunner.ViewModels.Commands.Factories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Neptuo.Productivity.SolutionRunner.ViewModels.Factories
{
    public class ConfigurationViewModelFactory : IFactory<ConfigurationViewModel>
    {
        private readonly IApplicationLoader mainApplicationLoader;
        private readonly ShortcutService shortcutService;
        private readonly DefaultRunHotKeyService runHotKey;
        private readonly ISettingsService settingsService;
        private readonly ISettings settings;
        private readonly INavigator navigator;

        internal ConfigurationViewModelFactory(IApplicationLoader mainApplicationLoader, ShortcutService shortcutService, DefaultRunHotKeyService runHotKey, ISettingsService settingsService, ISettings settings, INavigator navigator)
        {
            Ensure.NotNull(mainApplicationLoader, "mainApplicationLoader");
            Ensure.NotNull(shortcutService, "shortcutService");
            Ensure.NotNull(runHotKey, "runHotKey");
            Ensure.NotNull(settingsService, "settingsService");
            Ensure.NotNull(settings, "settings");
            Ensure.NotNull(navigator, "navigator");
            this.mainApplicationLoader = mainApplicationLoader;
            this.shortcutService = shortcutService;
            this.runHotKey = runHotKey;
            this.settingsService = settingsService;
            this.settings = settings;
            this.navigator = navigator;
        }

        public ConfigurationViewModel Create()
        {
            ConfigurationViewModel viewModel = new ConfigurationViewModel(new SaveConfigurationCommandFactory(settingsService, settings, runHotKey, shortcutService), navigator);

            viewModel.ConfigurationPath = Properties.Configuration.Default.Path;

            viewModel.SourceDirectoryPath = settings.SourceDirectoryPath;
            viewModel.FileSearchMode = settings.FileSearchMode;
            viewModel.FileSearchCount = settings.FileSearchCount;
            viewModel.IsFileSearchPatternSaved = settings.IsFileSearchPatternSaved;
            viewModel.IsLastUsedApplicationSavedAsPrefered = settings.IsLastUsedApplicationSavedAsPrefered;
            viewModel.IsDismissedWhenLostFocus = settings.IsDismissedWhenLostFocus;
            viewModel.IsHiddentOnStartup = settings.IsHiddentOnStartup;
            viewModel.IsAutoSelectApplicationVersion = settings.IsAutoSelectApplicationVersion;
            viewModel.IsFileNameRemovedFromDisplayedPath = settings.IsFileNameRemovedFromDisplayedPath;
            viewModel.IsDisplayedPathTrimmedToLastFolderName = settings.IsDisplayedPathTrimmedToLastFolderName;
            viewModel.IsAutoStartup = shortcutService.Exists(Environment.SpecialFolder.Startup);
            viewModel.IsTrayIcon = settings.IsTrayIcon;
            viewModel.IsStatisticsCounted = settings.IsStatisticsCounted;
            viewModel.IsProjectCountEnabled = settings.IsProjectCountEnabled;
            viewModel.AdditionalApplications = new ObservableCollection<AdditionalApplicationListViewModel>(settings.AdditionalApplications.Select(a => new AdditionalApplicationListViewModel(a)));

            MainApplicationCollection mainApplications = new MainApplicationCollection();
            mainApplicationLoader.Add(mainApplications);
            viewModel.MainApplications = mainApplications;

            foreach (MainApplicationListViewModel application in mainApplications)
                application.IsEnabled = !settings.HiddenMainApplications.Contains(application.Path);

            PreferedApplicationCollection preferedApplications = new PreferedApplicationCollection()
                .AddCollectionChanged(viewModel.AdditionalApplications)
                .AddCollectionChanged(mainApplications);

            viewModel.PreferedApplications = preferedApplications;
            viewModel.PreferedApplication = preferedApplications.FirstOrDefault(a => String.Equals(a.Path, settings.PreferedApplicationPath, StringComparison.InvariantCultureIgnoreCase));

            VsVersionCollection vsVersions = new VsVersionCollection();
            mainApplicationLoader.Add(vsVersions);
            viewModel.VsVersions = vsVersions;
            viewModel.AutoSelectApplicationMinimalVersion = vsVersions.FirstOrDefault(vm => vm.Model == settings.AutoSelectApplicationMinimalVersion);

            viewModel.RunKey = runHotKey.FindKeyViewModel();
            viewModel.PositionMode = settings.PositionMode;
            viewModel.PositionLeft = settings.PositionLeft;
            viewModel.PositionTop = settings.PositionTop;

            viewModel.ThemeMode = settings.ThemeMode;

            return viewModel;
        }
    }
}
