﻿using Neptuo;
using Neptuo.Activators;
using Neptuo.Observables.Collections;
using Neptuo.Productivity.SolutionRunner.Services;
using Neptuo.Productivity.SolutionRunner.Services.Applications;
using Neptuo.Productivity.SolutionRunner.Services.Configuration;
using Neptuo.Productivity.SolutionRunner.Services.Execution;
using Neptuo.Productivity.SolutionRunner.Services.Logging;
using Neptuo.Productivity.SolutionRunner.Services.Searching;
using Neptuo.Productivity.SolutionRunner.Services.StartupShortcuts;
using Neptuo.Productivity.SolutionRunner.Services.Statistics;
using Neptuo.Productivity.SolutionRunner.ViewModels.Commands.Factories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Neptuo.Productivity.SolutionRunner.ViewModels.Factories
{
    public class ConfigurationViewModelFactory : IAsyncFactory<ConfigurationViewModel>, IConfigurationViewModelMapper
    {
        private readonly IApplicationLoader mainApplicationLoader;
        private readonly IAutoStartup autoStartup;
        private readonly DefaultRunHotKeyService runHotKey;
        private readonly ISettingsService settingsService;
        private readonly ISettings settings;
        private readonly ISettingsFactory settingsFactory;
        private readonly INavigator navigator;
        private readonly ILogService logProvider;
        private readonly IDiagnosticService searchDiagnostics;
        private readonly FileLogBatchFactory executorFactory;
        private readonly ProcessService processes;
        private readonly ApplicationVersion applicationVersion;
        private readonly ICountingImporter countingImporter;
        private readonly IFactory<StatisticsRootViewModel> statisticsFactory;

        internal ConfigurationViewModelFactory(
            IApplicationLoader mainApplicationLoader,
            IAutoStartup autoStartup,
            DefaultRunHotKeyService runHotKey,
            ISettingsService settingsService,
            ISettings settings,
            ISettingsFactory settingsFactory,
            INavigator navigator,
            ILogService logProvider,
            IDiagnosticService searchDiagnostics,
            FileLogBatchFactory executorFactory,
            ProcessService processes,
            ApplicationVersion applicationVersion,
            ICountingImporter countingImporter,
            IFactory<StatisticsRootViewModel> statisticsFactory
        )
        {
            Ensure.NotNull(mainApplicationLoader, "mainApplicationLoader");
            Ensure.NotNull(autoStartup, "autoStartup");
            Ensure.NotNull(runHotKey, "runHotKey");
            Ensure.NotNull(settingsService, "settingsService");
            Ensure.NotNull(settings, "settings");
            Ensure.NotNull(settingsFactory, "settingsFactory");
            Ensure.NotNull(navigator, "navigator");
            Ensure.NotNull(logProvider, "logProvider");
            Ensure.NotNull(searchDiagnostics, "searchDiagnostics");
            Ensure.NotNull(executorFactory, "executorFactory");
            Ensure.NotNull(processes, "processes");
            Ensure.NotNull(applicationVersion, "applicationVersion");
            Ensure.NotNull(countingImporter, "countingImporter");
            Ensure.NotNull(statisticsFactory, "statisticsFactory");
            this.mainApplicationLoader = mainApplicationLoader;
            this.autoStartup = autoStartup;
            this.runHotKey = runHotKey;
            this.settingsService = settingsService;
            this.settings = settings;
            this.settingsFactory = settingsFactory;
            this.navigator = navigator;
            this.logProvider = logProvider;
            this.searchDiagnostics = searchDiagnostics;
            this.executorFactory = executorFactory;
            this.processes = processes;
            this.applicationVersion = applicationVersion;
            this.countingImporter = countingImporter;
            this.statisticsFactory = statisticsFactory;
        }

        public async Task<ConfigurationViewModel> Create()
        {
            ConfigurationViewModel viewModel = new ConfigurationViewModel(
                new SaveConfigurationCommandFactory(this, settingsService, settings), 
                settingsFactory, 
                this, 
                navigator,
                new TroubleshootViewModel(logProvider, searchDiagnostics, executorFactory, processes), 
                new StatisticsWithImportViewModel(statisticsFactory, countingImporter),
                new AboutViewModel(applicationVersion, processes)
            );

            viewModel.ConfigurationPath = Properties.Configuration.Default.Path;

            await MapAsync(settings, viewModel);
            return viewModel;
        }

        public async Task MapAsync(ISettings settings, ConfigurationViewModel viewModel)
        {
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
            viewModel.IsAutoStartup = await autoStartup.IsEnabledAsync();
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
            viewModel.LogLevel = settings.LogLevel;
        }

        public async Task MapAsync(ConfigurationViewModel viewModel, ISettings settings)
        {
            settings.SourceDirectoryPath = viewModel.SourceDirectoryPath;
            settings.PreferedApplicationPath = viewModel.PreferedApplication?.Path;
            settings.FileSearchMode = viewModel.FileSearchMode;
            settings.FileSearchCount = viewModel.FileSearchCount;
            settings.IsFileSearchPatternSaved = viewModel.IsFileSearchPatternSaved;
            settings.IsLastUsedApplicationSavedAsPrefered = viewModel.IsLastUsedApplicationSavedAsPrefered;
            settings.IsDismissedWhenLostFocus = viewModel.IsDismissedWhenLostFocus;
            settings.IsHiddentOnStartup = viewModel.IsHiddentOnStartup;
            settings.IsAutoSelectApplicationVersion = viewModel.IsAutoSelectApplicationVersion;
            settings.AutoSelectApplicationMinimalVersion = viewModel.AutoSelectApplicationMinimalVersion.Model;
            settings.IsFileNameRemovedFromDisplayedPath = viewModel.IsFileNameRemovedFromDisplayedPath;
            settings.IsDisplayedPathTrimmedToLastFolderName = viewModel.IsDisplayedPathTrimmedToLastFolderName;
            settings.IsTrayIcon = viewModel.IsTrayIcon;
            settings.IsStatisticsCounted = viewModel.IsStatisticsCounted;
            settings.IsProjectCountEnabled = viewModel.IsProjectCountEnabled;
            settings.AdditionalApplications = new AdditionalApplicationCollection(viewModel.AdditionalApplications.Select(a => a.Model));

            if (viewModel.IsAutoStartup)
                await autoStartup.EnableAsync();
            else
                await autoStartup.DisableAsync();

            string runKeyValue;
            if (Converts.Try(viewModel.RunKey, out runKeyValue))
                settings.RunKey = runKeyValue;

            if (viewModel.RunKey == null)
                runHotKey.UnBind();
            else
                runHotKey.Bind(viewModel.RunKey.Key, viewModel.RunKey.Modifier);

            settings.PositionMode = viewModel.PositionMode;
            settings.PositionLeft = viewModel.PositionLeft ?? 0;
            settings.PositionTop = viewModel.PositionTop ?? 0;

            settings.HiddenMainApplications = viewModel.MainApplications.Where(a => !a.IsEnabled).Select(a => a.Path).ToArray();

            settings.ThemeMode = viewModel.ThemeMode;
            settings.LogLevel = viewModel.LogLevel;
        }
    }
}
