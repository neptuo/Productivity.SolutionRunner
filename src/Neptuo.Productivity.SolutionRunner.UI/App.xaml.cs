using Neptuo.Activators;
using Neptuo.Converters;
using Neptuo.Exceptions;
using Neptuo.Exceptions.Handlers;
using Neptuo.Formatters.Converters;
using Neptuo.Logging;
using Neptuo.Logging.Serialization.Converters;
using Neptuo.Logging.Serialization.Formatters;
using Neptuo.Productivity.SolutionRunner.Properties;
using Neptuo.Productivity.SolutionRunner.Services;
using Neptuo.Productivity.SolutionRunner.Services.Applications;
using Neptuo.Productivity.SolutionRunner.Services.Colors;
using Neptuo.Productivity.SolutionRunner.Services.Configuration;
using Neptuo.Productivity.SolutionRunner.Services.Converters;
using Neptuo.Productivity.SolutionRunner.Services.Exceptions;
using Neptuo.Productivity.SolutionRunner.Services.Execution;
using Neptuo.Productivity.SolutionRunner.Services.Logging;
using Neptuo.Productivity.SolutionRunner.Services.Positions;
using Neptuo.Productivity.SolutionRunner.Services.StartupFlags;
using Neptuo.Productivity.SolutionRunner.Services.StartupShortcuts;
using Neptuo.Productivity.SolutionRunner.Services.Statistics;
using Neptuo.Productivity.SolutionRunner.Services.Themes;
using Neptuo.Productivity.SolutionRunner.ViewModels;
using Neptuo.Productivity.SolutionRunner.ViewModels.Factories;
using Neptuo.Productivity.SolutionRunner.Views;
using Neptuo.Productivity.SolutionRunner.Views.Controls;
using Neptuo.Productivity.SolutionRunner.Views.Converters;
using Neptuo.Windows.Threading;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Deployment.Application;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Threading;
using EventManager = Neptuo.Productivity.SolutionRunner.ViewModels.EventManager;

namespace Neptuo.Productivity.SolutionRunner
{
    public partial class App : Application, INavigator, INavigatorState, IWindowManager
    {
        private ISettingsService settingsService;
        private ISettingsMapper settingsMapper;
        private ISettings settings;
        private StartupModel startup;
        private IApplicationLoader mainApplicationLoader;
        private DefaultRunHotKeyService runHotKey;
        private SwitchableContingService countingService;
        private IPositionProvider positionProvider;
        private ILogFactory logFactory;
        private IsolatedLogService logService;
        private FileLogBatchFactory executorFactory;

        private IExceptionHandler exceptionHandler;

        private IFactory<ConfigurationViewModel> configurationFactory;
        private MainViewModelFactory mainFactory;

        // THIS must be synchronized with Click-Once deployment settings.
        private readonly ShortcutService shortcutService = new ShortcutService(
            companyName: "Neptuo",
            suiteName: "Productivity",
            productName: "Productivity.SolutionRunner"
        );

        private AppTrayIcon trayIcon;
        private AppPinStateService pinStateService;

        protected DispatcherHelper DispatcherHelper { get; private set; }

        private void PrepareStartup(StartupEventArgs e)
        {
            DispatcherHelper = new DispatcherHelper(Dispatcher);

            StartupModelProvider provider = new StartupModelProvider();
            startup = provider.Get(e.Args);
            startup.IsStartup = true;

            if (!startup.IsHidden)
                startup.IsHidden = settings.IsHiddentOnStartup;
        }

        protected async override void OnStartup(StartupEventArgs e)
        {
            InitializeConverters();
            await InitializeSettingsAsync();

            ReloadThemeResources();

            TaskScheduler.UnobservedTaskException += OnTaskSchedulerUnobservedException;

            PrepareStartup(e);
            base.OnStartup(e);

            trayIcon = new AppTrayIcon(this, settings, this, this);
            pinStateService = new AppPinStateService(settingsService, settings);
            positionProvider = new PositionService(settings);

            InitializeErrorHandler();
            InitializeEventManager();

            mainApplicationLoader = new ApplicationLoaderCollection()
                .Add(new VsVersionLoader())
                .Add(new Vs2017VersionLoader())
                .Add(new VsCodeLoader());

            await BindHotKeyAsync();

            InitializeViewModelFactories();

            SettingsExtension.Settings = await settingsService.LoadRawAsync();
            PathConverter.Settings = settings;

            InitializeCounting();

            trayIcon.TryCreate();

            // Open window.
            if (String.IsNullOrEmpty(settings.SourceDirectoryPath))
                OpenConfiguration();
            else
                OpenMain();

            startup.IsStartup = false;
        }

        private async Task InitializeSettingsAsync()
        {
            settingsMapper = new ManualSettingsMapper();

            if (String.IsNullOrEmpty(Configuration.Default.Path) || !File.Exists(Configuration.Default.Path))
            {
                Configuration.Default.Path = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile), "SolutionRunner.json");

#if DEBUG
                if (true)
#else
                if (!ApplicationDeployment.CurrentDeployment.IsFirstRun)
#endif
                {
                    MessageBox.Show(
                        "Unfortunately, we have lost your configuration file (or, if you see this message for the first time, we are going to upgrade from previous version)."
                        + Environment.NewLine
                        + "More information can be found in configuration -> Import and Export.",
                        "SolutionRunner"
                    );
                }

                if (!File.Exists(Configuration.Default.Path))
                {
                    await CopySettingsAsync(
                        settingsMapper,
                        new DefaultSettingsService(),
                        new JsonSettingsService(() => Configuration.Default.Path)
                    );
                }

                Configuration.Default.Save();
            }

            settingsService = new JsonSettingsService(() => Configuration.Default.Path);
            settings = await settingsService.LoadAsync();
        }

        private async Task CopySettingsAsync(ISettingsMapper mapper, ISettingsService source, ISettingsService target)
        {
            var sourceSettings = await source.LoadAsync();
            var targetSettings = await target.LoadAsync();

            mapper.Map(sourceSettings, targetSettings);

            await target.SaveAsync(targetSettings);
        }

        private void InitializeConverters()
        {
            Converts.Repository
                .AddJsonPrimitivesSearchHandler()
                .AddJsonObjectSearchHandler()
                .AddJsonEnumSearchHandler()
                .AddEnumSearchHandler(false)
                .AddToStringSearchHandler()
                .Add(new JsonReadOnlyListConverter())
                .Add(new JsonVersionConverter())
                .Add(new AdditionalApplicationCollectionConverter())
                .Add(new KeyViewModelConverter())
                .Add(new ExceptionModelConverter());
        }

        private void InitializeEventManager()
        {
            EventManager.ConfigurationSaved += OnConfigurationSaved;
            EventManager.ProcessStarted += OnProcessStarted;
        }

        private async Task BindHotKeyAsync()
        {
            runHotKey = new DefaultRunHotKeyService(this, this);
            KeyViewModel runKey;
            if (Converts.Try(settings.RunKey, out runKey) && runKey != null)
            {
                try
                {
                    runHotKey.Bind(runKey.Key, runKey.Modifier);
                }
                catch (Win32Exception)
                {
                    runHotKey.UnBind();
                    settings.RunKey = null;
                    await settingsService.SaveAsync(settings);
                }
            }

            if (!runHotKey.IsSet)
                startup.IsHidden = false;
        }

        private void InitializeErrorHandler()
        {
            executorFactory = new FileLogBatchFactory(TimeSpan.FromSeconds(30));

            logService = new IsolatedLogService();

            logFactory = new DefaultLogFactory()
                .AddSerializer(AddDisposable(new FileLogSerializer(new DefaultLogFormatter(), () => settings.LogLevel, executorFactory)))
                .AddSerializer(AddDisposable(new ErrorLogSerializer(new DefaultLogFormatter(), executorFactory)))
#if DEBUG
                .AddConsole()
#endif
            ;

            ILog rootLog = logFactory.Scope("Root");

            ExceptionHandlerBuilder builder = new ExceptionHandlerBuilder();
            builder
                .Filter<UnauthorizedAccessException>()
                .Handler(new UnauthorizedAccessExceptionHandler(settings, this, () => { mainWindow?.Close(); mainFactory.ClearService(); }));

            builder
                .Filter(e => !(e is UnauthorizedAccessException))
                .Handler(new LogExceptionHandler(rootLog))
                .Handler(new MessageBoxExceptionHandler(this));

            exceptionHandler = builder;
        }

        private void InitializeViewModelFactories()
        {
            mainFactory = AddDisposable(new MainViewModelFactory(
                pinStateService,
                settings,
                mainApplicationLoader,
                logFactory,
                OnMainViewModelPropertyChanged
            ));

            configurationFactory = new ConfigurationViewModelFactory(
                mainApplicationLoader,
                shortcutService,
                runHotKey,
                settingsService,
                settings,
                new JsonSettingsFactory(),
                this,
                logService,
                mainFactory,
                executorFactory
            );
        }

        private void OnProcessStarted(IApplication application, IFile file)
        {
            TrySaveLastSearchPattern();
        }

        private void TrySaveLastSearchPattern()
        {
            if (settings.IsFileSearchPatternSaved)
            {
                settings.FileSearchPattern = mainWindow.ViewModel.SearchPattern;
                settingsService.SaveAsync(settings);
            }
        }

        private void InitializeCounting()
        {
            CountingService inner = new CountingService();
            countingService = new SwitchableContingService(settings, inner, inner);
        }

        private void OnConfigurationSaved(ConfigurationViewModel viewModel)
        {
            if (viewModel.IsTrayIcon)
                trayIcon.TryCreate();
            else
                trayIcon.TryDestroy();

            ReloadThemeResources();
        }

        private void ReloadThemeResources()
        {
            Uri uri = null;
            switch (settings.ThemeMode)
            {
                case ThemeMode.Dark:
                    uri = new Uri("/Views/Themes/Dark.xaml", UriKind.Relative);
                    break;
                case ThemeMode.Light:
                    uri = new Uri("/Views/Themes/Light.xaml", UriKind.Relative);
                    break;
                default:
                    throw Ensure.Exception.NotSupported(settings.ThemeMode);
            }

            if (Resources.MergedDictionaries[0].Source != uri)
                Resources.MergedDictionaries[0].Source = uri;
        }

        public void Activate()
        {
            Window wnd = null;

            if (statisticsWindow != null)
                wnd = statisticsWindow;
            else if (configurationWindow != null)
                OpenConfiguration();
            else if (mainWindow != null)
                OpenMain();

            if (wnd != null)
            {
                if (!wnd.IsVisible)
                    wnd.Show();

                wnd.Activate();
            }
        }

        #region Handling exceptions

        public static void ShowExceptionDialog(Exception e)
        {
            ((App)Current).ShowExceptionDialogInternal(e);
        }

        private void ShowExceptionDialogInternal(Exception e)
        {
            AggregateException aggregate = e as AggregateException;
            if (aggregate != null)
                e = aggregate.InnerException;

            exceptionHandler.Handle(e);
        }

        private void OnDispatcherUnhandledException(object sender, DispatcherUnhandledExceptionEventArgs e)
        {
            ShowExceptionDialog(e.Exception);
            e.Handled = true;
        }

        private void OnTaskSchedulerUnobservedException(object sender, UnobservedTaskExceptionEventArgs e)
        {
            ShowExceptionDialog(e.Exception);
            e.SetObserved();
        }

        #endregion

        protected override void OnExit(ExitEventArgs e)
        {
            runHotKey.Dispose();
            trayIcon.TryDestroy();

            foreach (IDisposable disposable in disposables)
                disposable.Dispose();

            base.OnExit(e);
        }

        #region INavigator & INavigatorState & IAppWindowManager

        MainWindow IWindowManager.Main
        {
            get => mainWindow;
        }

        ConfigurationWindow IWindowManager.Configuration
        {
            get => configurationWindow;
        }

        StatisticsWindow IWindowManager.Statistics
        {
            get => statisticsWindow;
        }

        private MainWindow mainWindow;
        private ConfigurationWindow configurationWindow;
        private bool isMainWindowViewModelReloadRequired = false;

        public bool IsConfigurationOpened
        {
            get { return configurationWindow != null; }
        }

        public bool IsMainOpened
        {
            get { return mainWindow != null; }
        }

        public void OpenConfiguration()
        {
            if (configurationWindow == null)
            {
                isMainWindowViewModelReloadRequired = true;

                configurationWindow = new ConfigurationWindow(configurationFactory.Create(), this, String.IsNullOrEmpty(settings.SourceDirectoryPath));
                configurationWindow.ShowInTaskbar = !runHotKey.IsSet;
                configurationWindow.ResizeMode = !runHotKey.IsSet ? ResizeMode.CanMinimize : ResizeMode.NoResize;
                configurationWindow.Closed += OnConfigurationWindowClosed;
            }

            configurationWindow.Show();
            configurationWindow.Activate();
        }

        private void OnConfigurationWindowClosed(object sender, EventArgs e)
        {
            configurationWindow.Closed -= OnConfigurationWindowClosed;
            configurationWindow = null;

            if (!runHotKey.IsSet && mainWindow == null)
                Shutdown();
        }

        private AdditionalApplicationModel sourceAdditionalApplicationModel;
        private AdditionalApplicationEditWindow additionalApplicationEditWindow;
        private Action<AdditionalApplicationModel> onAdditionalApplicationSaved;

        public void OpenAdditionalApplicationEdit(AdditionalApplicationModel model, Action<AdditionalApplicationModel> onSaved)
        {
            sourceAdditionalApplicationModel = model;
            onAdditionalApplicationSaved = onSaved;

            AdditionalApplicationEditViewModel viewModel = new AdditionalApplicationEditViewModel(this, model, OnAdditionalApplicationSaved);
            additionalApplicationEditWindow = new AdditionalApplicationEditWindow(viewModel);
            additionalApplicationEditWindow.Owner = configurationWindow;
            additionalApplicationEditWindow.ShowDialog();
        }

        private void OnAdditionalApplicationSaved(AdditionalApplicationModel model)
        {
            additionalApplicationEditWindow.Close();

            if (onAdditionalApplicationSaved != null)
                onAdditionalApplicationSaved(model);
        }

        private AdditionalApplicationModel sourceAdditionalCommandModel;
        private AdditionalCommandEditWindow additionalCommandEditWindow;
        private Action<AdditionalApplicationModel> onAdditionalCommandSaved;

        public void OpenAdditionalCommandEdit(AdditionalApplicationModel model, Action<AdditionalApplicationModel> onSaved)
        {
            sourceAdditionalCommandModel = model;
            onAdditionalCommandSaved = onSaved;

            AdditionalCommandEditViewModel viewModel = new AdditionalCommandEditViewModel(this, model, OnAdditionalCommandSaved);
            additionalCommandEditWindow = new AdditionalCommandEditWindow(viewModel);
            additionalCommandEditWindow.Owner = configurationWindow;
            additionalCommandEditWindow.ShowDialog();
        }

        private void OnAdditionalCommandSaved(AdditionalApplicationModel model)
        {
            additionalCommandEditWindow.Close();

            if (onAdditionalCommandSaved != null)
                onAdditionalCommandSaved(model);
        }

        public void OpenMain()
        {
            if (mainWindow == null)
            {
                mainWindow = new MainWindow(this, positionProvider, settingsService, settings, new ProcessService(countingService), runHotKey.IsSet);
                mainWindow.Closing += OnMainWindowClosing;
                mainWindow.Closed += OnMainWindowClosed;
            }

            if (mainWindow.ViewModel == null || isMainWindowViewModelReloadRequired)
            {
                isMainWindowViewModelReloadRequired = false;

                mainWindow.ShowInTaskbar = !runHotKey.IsSet;
                mainWindow.IsAutoSelectApplicationVersion = settings.IsAutoSelectApplicationVersion;

                mainWindow.ViewModel = mainFactory.Create();
                mainWindow.TrySelectPreferedApplication();
            }

            mainWindow.IsAutoSelectApplicationVersion = settings.IsAutoSelectApplicationVersion;
            ResetLastSearchPattern();
            mainWindow.Deactivated += OnMainWindowDeactivated;

            trayIcon.TryUpdate();

            if (!startup.IsStartup || !startup.IsHidden)
            {
                mainWindow.Show();
                positionProvider.Apply(mainWindow);
                mainWindow.Activate();
            }
        }

        private void OnMainViewModelPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(MainViewModel.IsLoading))
                trayIcon.TryUpdate();
        }

        private void ResetLastSearchPattern()
        {
            if (settings.IsFileSearchPatternSaved)
                mainWindow.ViewModel.SearchPattern = settings.FileSearchPattern;
            else if (!String.IsNullOrEmpty(mainWindow.ViewModel.SearchPattern))
                mainWindow.ViewModel.SearchPattern = String.Empty;
        }

        private void OnMainWindowDeactivated(object sender, EventArgs e)
        {
            if (settings.IsDismissedWhenLostFocus && mainWindow != null)
            {
                DispatcherHelper.Run(() =>
                {
                    if (settings.IsDismissedWhenLostFocus && mainWindow != null && !mainWindow.IsActive)
                        mainWindow.Close();
                }, 500);
            }
        }

        private void OnMainWindowClosing(object sender, CancelEventArgs e)
        {
            if (runHotKey.IsSet)
            {
                mainWindow.Hide();
                e.Cancel = true;
            }

            TrySaveLastSearchPattern();
            mainWindow.Deactivated -= OnMainWindowDeactivated;
        }

        private void OnMainWindowClosed(object sender, EventArgs e)
        {
            TrySaveLastSearchPattern();

            if (mainWindow.ViewModel != null)
                mainWindow.ViewModel.PropertyChanged -= OnMainViewModelPropertyChanged;

            mainWindow.Closed -= OnMainWindowClosed;
            mainWindow.Closing -= OnMainWindowClosing;
            mainWindow = null;

            if (!runHotKey.IsSet && configurationWindow == null)
                Shutdown();
        }

        private StatisticsWindow statisticsWindow;

        public void OpenStatistics()
        {
            IFactory<IColorGenerator> colorGeneratorFactory = Factory.Default<RandomColorGenerator>();

            ContainerCollection<ContainerCollection<StatisticsViewModel>> viewModel = new ContainerCollection<ContainerCollection<StatisticsViewModel>>();
            Stack<StatisticsViewModel> appendTo = new Stack<StatisticsViewModel>();

            Container<ContainerCollection<StatisticsViewModel>> allViewModel = new Container<ContainerCollection<StatisticsViewModel>>();
            allViewModel.Title = "All";
            allViewModel.Data = new ContainerCollection<StatisticsViewModel>()
            {
                new Container<StatisticsViewModel>()
                {
                    Title = "All",
                    Data = new StatisticsViewModel(colorGeneratorFactory.Create())
                }
            };
            appendTo.Push(allViewModel.Data[0].Data);
            viewModel.Add(allViewModel);

            foreach (IGrouping<int, Month> year in countingService.Months().OrderByDescending(m => m.Year).GroupBy(m => m.Year))
            {
                Container<ContainerCollection<StatisticsViewModel>> yearViewModel = new Container<ContainerCollection<StatisticsViewModel>>();
                yearViewModel.Title = year.Key.ToString();
                yearViewModel.Data = new ContainerCollection<StatisticsViewModel>();

                yearViewModel.Data.Add(new Container<StatisticsViewModel>()
                {
                    Title = "All",
                    Data = new StatisticsViewModel(colorGeneratorFactory.Create())
                });
                appendTo.Push(yearViewModel.Data[0].Data);

                foreach (Month month in year.OrderBy(m => m.Value))
                {
                    Container<StatisticsViewModel> monthViewModel = new Container<StatisticsViewModel>();
                    monthViewModel.Title = month.ToShortString();
                    monthViewModel.Data = new StatisticsViewModel(colorGeneratorFactory.Create());

                    appendTo.Push(monthViewModel.Data);

                    foreach (ApplicationCountModel application in countingService.Applications(month, month))
                        appendTo.ForEach(vm => vm.AddApplication(application.Path, application.Count));

                    foreach (FileCountModel file in countingService.Files(month, month))
                        appendTo.ForEach(vm => vm.AddFile(file.Path, file.Count));

                    appendTo.Pop();
                    yearViewModel.Data.Add(monthViewModel);
                }

                appendTo.Pop();
                viewModel.Add(yearViewModel);
            }

            statisticsWindow = new StatisticsWindow();
            statisticsWindow.ViewModel = viewModel;
            statisticsWindow.Owner = configurationWindow;
            statisticsWindow.Closed += OnStatisticsWindowClosed;
            statisticsWindow.ShowDialog();
        }

        private void OnStatisticsWindowClosed(object sender, EventArgs e)
        {
            statisticsWindow.Closed -= OnStatisticsWindowClosed;
            statisticsWindow = null;
        }

        #endregion

        private readonly List<IDisposable> disposables = new List<IDisposable>();

        private T AddDisposable<T>(T disposable)
            where T : IDisposable
        {
            if (disposable != null)
                disposables.Add(disposable);

            return disposable;
        }
    }

    public static class ForEachExtensions
    {
        public static void ForEach<T>(this IEnumerable<T> items, Action<T> handler)
        {
            Ensure.NotNull(items, "items");
            foreach (T item in items)
                handler(item);
        }
    }
}
