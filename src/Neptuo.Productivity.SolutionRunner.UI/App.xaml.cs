using Neptuo;
using Neptuo.Activators;
using Neptuo.Converters;
using Neptuo.Exceptions;
using Neptuo.Exceptions.Handlers;
using Neptuo.Formatters.Converters;
using Neptuo.Logging;
using Neptuo.Logging.Serialization.Converters;
using Neptuo.Logging.Serialization.Formatters;
using Neptuo.Observables.Collections;
using Neptuo.Productivity.SolutionRunner.Properties;
using Neptuo.Productivity.SolutionRunner.Services;
using Neptuo.Productivity.SolutionRunner.Services.Applications;
using Neptuo.Productivity.SolutionRunner.Services.Colors;
using Neptuo.Productivity.SolutionRunner.Services.Converters;
using Neptuo.Productivity.SolutionRunner.Services.Exceptions;
using Neptuo.Productivity.SolutionRunner.Services.Execution;
using Neptuo.Productivity.SolutionRunner.Services.Logging;
using Neptuo.Productivity.SolutionRunner.Services.Positions;
using Neptuo.Productivity.SolutionRunner.Services.Searching;
using Neptuo.Productivity.SolutionRunner.Services.StartupFlags;
using Neptuo.Productivity.SolutionRunner.Services.StartupShortcuts;
using Neptuo.Productivity.SolutionRunner.Services.Statistics;
using Neptuo.Productivity.SolutionRunner.Services.Themes;
using Neptuo.Productivity.SolutionRunner.ViewModels;
using Neptuo.Productivity.SolutionRunner.ViewModels.Commands.Factories;
using Neptuo.Productivity.SolutionRunner.ViewModels.Factories;
using Neptuo.Productivity.SolutionRunner.Views;
using Neptuo.Productivity.SolutionRunner.Views.Controls;
using Neptuo.Windows.Threading;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Threading;
using EventManager = Neptuo.Productivity.SolutionRunner.ViewModels.EventManager;
using NotifyIcon = System.Windows.Forms.NotifyIcon;

namespace Neptuo.Productivity.SolutionRunner
{
    public partial class App : Application, INavigator, INavigatorState, IPinStateService
    {
        private StartupModel startup;
        private IApplicationLoader mainApplicationLoader;
        private DefaultRunHotKeyService runHotKey;
        private SwitchableContingService countingService;
        private IPositionProvider positionProvider;
        private ILog log;
        private ErrorLog errorLog;

        private IExceptionHandler exceptionHandler;

        private IFactory<ConfigurationViewModel> configurationFactory;
        private MainViewModelFactory mainFactory;

        // THIS must be synchronized with Click-Once deployment settings.
        private readonly ShortcutService shortcutService = new ShortcutService(
            companyName: "Neptuo",
            suiteName: "Productivity",
            productName: "Productivity.SolutionRunner"
        );

        protected DispatcherHelper DispatcherHelper { get; private set; }

        private void PrepareStartup(StartupEventArgs e)
        {
            DispatcherHelper = new DispatcherHelper(Dispatcher);

            StartupModelProvider provider = new StartupModelProvider();
            startup = provider.Get(e.Args);
            startup.IsStartup = true;

            if (!startup.IsHidden)
                startup.IsHidden = Settings.Default.IsHiddentOnStartup;
        }

        protected override void OnStartup(StartupEventArgs e)
        {
            ReloadThemeResources();

            TaskScheduler.UnobservedTaskException += OnTaskSchedulerUnobservedException;

            //#if DEBUG
            //            Settings.Default.Reset();
            //#endif

            PrepareStartup(e);
            base.OnStartup(e);

            BuildErrorHandler();

            Converts.Repository
                .AddJsonPrimitivesSearchHandler()
                .AddJsonObjectSearchHandler()
                .AddJsonEnumSearchHandler()
                .AddEnumSearchHandler(false)
                .AddToStringSearchHandler()
                .Add(new AdditionalApplicationCollectionConverter())
                .Add(new KeyViewModelConverter())
                .Add(new ExceptionModelConverter());

            EventManager.FilePinned += OnFilePinned;
            EventManager.ConfigurationSaved += OnConfigurationSaved;
            EventManager.ProcessStarted += OnProcessStarted;

            mainApplicationLoader = new ApplicationLoaderCollection()
                .Add(new VsVersionLoader())
                .Add(new Vs2017VersionLoader())
                .Add(new VsCodeLoader());

            // Bind global hotkey.
            runHotKey = new DefaultRunHotKeyService(this, this);
            KeyViewModel runKey;
            if (Converts.Try(Settings.Default.RunKey, out runKey) && runKey != null)
            {
                try
                {
                    runHotKey.Bind(runKey.Key, runKey.Modifier);
                }
                catch (Win32Exception)
                {
                    runHotKey.UnBind();
                    Settings.Default.RunKey = null;
                    Settings.Default.Save();
                }
            }

            if (!runHotKey.IsSet)
                startup.IsHidden = false;

            configurationFactory = new ConfigurationViewModelFactory(
                mainApplicationLoader,
                shortcutService,
                runHotKey,
                Settings.Default,
                this
            );

            mainFactory = new MainViewModelFactory(
                this,
                Settings.Default,
                mainApplicationLoader,
                GetPinnedFiles,
                OnMainViewModelPropertyChanged
            );

            SettingsExtension.Settings = Settings.Default;

            positionProvider = new PositionService(Settings.Default);
            InitializeCounting();

            if (Settings.Default.IsTrayIcon)
                TryCreateTrayIcon();

            // Open window.
            if (String.IsNullOrEmpty(Settings.Default.SourceDirectoryPath))
                OpenConfiguration();
            else
                OpenMain();

            startup.IsStartup = false;
        }

        private void BuildErrorHandler()
        {
            errorLog = new ErrorLog(new DefaultLogFormatter());
            ILogFactory logFactory = new DefaultLogFactory()
                .AddSerializer(errorLog)
#if DEBUG
                .AddConsole()
#endif
            ;

            log = logFactory.Scope("Root");

            ExceptionHandlerBuilder builder = new ExceptionHandlerBuilder();
            builder
                .Filter<UnauthorizedAccessException>()
                .Handler(new UnauthorizedAccessExceptionHandler(Settings.Default, this, () => { mainWindow?.Close(); mainFactory.ClearService(); }));

            builder
                .Filter(e => !(e is UnauthorizedAccessException))
                .Handler(new LogExceptionHandler(log))
                .Handler(new MessageBoxExceptionHandler(this));

            exceptionHandler = builder;
        }

        private void OnProcessStarted(IApplication application, IFile file)
        {
            TrySaveLastSearchPattern();
        }

        private void TrySaveLastSearchPattern()
        {
            if (Settings.Default.IsFileSearchPatternSaved)
            {
                Settings.Default.FileSearchPattern = mainWindow.ViewModel.SearchPattern;
                Settings.Default.Save();
            }
        }

        private void InitializeCounting()
        {
            CountingService inner = new CountingService();
            countingService = new SwitchableContingService(Settings.Default, inner, inner);
        }

        private NotifyIcon trayIcon;

        private void OnConfigurationSaved(ConfigurationViewModel viewModel)
        {
            if (viewModel.IsTrayIcon)
                TryCreateTrayIcon();
            else
                TryDestroyTrayIcon();

            ReloadThemeResources();
        }

        private void ReloadThemeResources()
        {
            Uri uri = null;
            switch (Settings.Default.ThemeMode)
            {
                case ThemeMode.Dark:
                    uri = new Uri("/Views/Themes/Dark.xaml", UriKind.Relative);
                    break;
                case ThemeMode.Light:
                    uri = new Uri("/Views/Themes/Light.xaml", UriKind.Relative);
                    break;
                default:
                    throw Ensure.Exception.NotSupported(Settings.Default.ThemeMode);
            }

            if (Resources.MergedDictionaries[0].Source != uri)
                Resources.MergedDictionaries[0].Source = uri;
        }

        private bool TryCreateTrayIcon()
        {
            if (trayIcon == null)
            {
                trayIcon = new NotifyIcon();
                trayIcon.Icon = Icon.ExtractAssociatedIcon(Process.GetCurrentProcess().MainModule.FileName);
                trayIcon.Text = "SolutionRunner";
                trayIcon.MouseClick += OnTrayIconClick;
                trayIcon.Visible = true;

                trayIcon.ContextMenu = new System.Windows.Forms.ContextMenu();
                trayIcon.ContextMenu.MenuItems.Add("Open", (sender, e) => { OpenMain(); configurationWindow?.Close(); statisticsWindow?.Close(); });
                trayIcon.ContextMenu.MenuItems.Add("Configuration", (sender, e) => { OpenConfiguration(); mainWindow?.Close(); statisticsWindow?.Close(); });
                trayIcon.ContextMenu.MenuItems.Add("Statistics", (sender, e) => OpenStatistics());
                trayIcon.ContextMenu.MenuItems.Add("Exit", (sender, e) => Shutdown());
                return true;
            }

            return false;
        }

        private bool TryDestroyTrayIcon()
        {
            if (trayIcon != null)
            {
                trayIcon.MouseClick -= OnTrayIconClick;
                trayIcon.Dispose();
                trayIcon = null;
                return true;
            }

            return false;
        }

        private bool TryUpdateTrayIcon()
        {
            if (trayIcon != null && mainWindow != null)
            {
                string resourceName = null;
                if (mainWindow.ViewModel.IsLoading)
                    resourceName = "Neptuo.Productivity.SolutionRunner.Resources.Loading.ico";
                else
                    resourceName = "Neptuo.Productivity.SolutionRunner.Resources.SolutionRunner.ico";

                trayIcon.Icon = new Icon(Assembly.GetExecutingAssembly().GetManifestResourceStream(resourceName));
                return true;
            }

            return false;
        }

        private void OnTrayIconClick(object sender, System.Windows.Forms.MouseEventArgs e)
        {
            if (e.Button != System.Windows.Forms.MouseButtons.Right)
                Activate();
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
            TryDestroyTrayIcon();
            base.OnExit(e);
        }

        #region IPinStateService

        private HashSet<string> pinnedFiles;

        private HashSet<string> GetPinnedFiles()
        {
            if (pinnedFiles == null)
            {
                pinnedFiles = new HashSet<string>();

                string rawValue = Settings.Default.PinnedFiles;
                if (!String.IsNullOrEmpty(rawValue))
                {
                    foreach (string filePath in rawValue.Split(new char[] { ';' }, StringSplitOptions.RemoveEmptyEntries))
                    {
                        if (File.Exists(filePath))
                            pinnedFiles.Add(filePath);
                    }
                }
            }

            return pinnedFiles;
        }

        private void OnFilePinned(FileViewModel viewModel)
        {
            HashSet<string> pinnedFiles = GetPinnedFiles();
            if (viewModel.IsPinned)
                pinnedFiles.Add(viewModel.Path);
            else
                pinnedFiles.Remove(viewModel.Path);

            Settings.Default.PinnedFiles = String.Join(";", pinnedFiles);
            Settings.Default.Save();
        }

        public IEnumerable<string> Enumerate()
        {
            return GetPinnedFiles();
        }

        public bool IsPinned(string path)
        {
            Ensure.NotNullOrEmpty(path, "path");
            return GetPinnedFiles().Contains(path);
        }

        #endregion

        #region INavigator & INavigatorState

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

                configurationWindow = new ConfigurationWindow(configurationFactory.Create(), this, errorLog, String.IsNullOrEmpty(Settings.Default.SourceDirectoryPath));
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
                mainWindow = new MainWindow(this, Settings.Default, new ProcessService(countingService), runHotKey.IsSet);
                mainWindow.Closing += OnMainWindowClosing;
                mainWindow.Closed += OnMainWindowClosed;
            }

            if (mainWindow.ViewModel == null || isMainWindowViewModelReloadRequired)
            {
                isMainWindowViewModelReloadRequired = false;

                mainWindow.ShowInTaskbar = !runHotKey.IsSet;
                mainWindow.IsAutoSelectApplicationVersion = Settings.Default.IsAutoSelectApplicationVersion;

                mainWindow.ViewModel = mainFactory.Create();
                mainWindow.TrySelectPreferedApplication();
            }

            mainWindow.IsAutoSelectApplicationVersion = Settings.Default.IsAutoSelectApplicationVersion;
            ResetLastSearchPattern();
            mainWindow.Deactivated += OnMainWindowDeactivated;

            TryUpdateTrayIcon();

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
                TryUpdateTrayIcon();
        }

        private void ResetLastSearchPattern()
        {
            if (Settings.Default.IsFileSearchPatternSaved)
                mainWindow.ViewModel.SearchPattern = Settings.Default.FileSearchPattern;
            else if (!String.IsNullOrEmpty(mainWindow.ViewModel.SearchPattern))
                mainWindow.ViewModel.SearchPattern = String.Empty;
        }

        private void OnMainWindowDeactivated(object sender, EventArgs e)
        {
            if (Settings.Default.IsDismissedWhenLostFocus && mainWindow != null)
            {
                DispatcherHelper.Run(() =>
                {
                    if (Settings.Default.IsDismissedWhenLostFocus && mainWindow != null && !mainWindow.IsActive)
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
