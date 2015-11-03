using Neptuo.Converters;
using Neptuo.FileSystems;
using Neptuo.FileSystems.Features.Searching;
using Neptuo.Productivity.SolutionRunner.Properties;
using Neptuo.Productivity.SolutionRunner.Services;
using Neptuo.Productivity.SolutionRunner.Services.Converters;
using Neptuo.Productivity.SolutionRunner.Services.Searching;
using Neptuo.Productivity.SolutionRunner.Services.StartupFlags;
using Neptuo.Productivity.SolutionRunner.ViewModels;
using Neptuo.Productivity.SolutionRunner.ViewModels.Commands.Factories;
using Neptuo.Productivity.SolutionRunner.Views;
using Neptuo.Windows.HotKeys;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using System.Windows.Threading;
using EventManager = Neptuo.Productivity.SolutionRunner.ViewModels.EventManager;

namespace Neptuo.Productivity.SolutionRunner
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application, INavigator, INavigatorState, IPinStateService
    {
        private StartupModel startup;
        private DefaultRunHotKeyService runHotKey;

        private void PrepareStartup(StartupEventArgs e)
        {
            StartupModelProvider provider = new StartupModelProvider();
            startup = provider.Get(e.Args);
            startup.IsStartup = true;
        }

        protected override void OnStartup(StartupEventArgs e)
        {
            PrepareStartup(e);
            base.OnStartup(e);

            Converts.Repository
                .AddEnumSearchHandler(false)
                .AddToStringSearchHandler()
                .Add<string, KeyViewModel>(new StringToKeyViewModelConverter())
                .Add<KeyViewModel, string>(new KeyViewModelToStringConverter());


            EventManager.FilePinned += OnFilePinned;

            // Bind global hotkey.
            runHotKey = new DefaultRunHotKeyService(this, this);
            KeyViewModel runKey;
            if (Converts.Try(Settings.Default.RunKey, out runKey) && runKey != null)
            {
                try
                {
                    runHotKey.Bind(runKey.Key, runKey.Modifier);
                }
                catch (Win32Exception ex)
                {
                    runHotKey.UnBind();
                    Settings.Default.RunKey = null;
                    Settings.Default.Save();
                }
            }

            if(!runHotKey.IsSet)
                startup.IsHidden = false;

            // Open window.
            if (String.IsNullOrEmpty(Settings.Default.SourceDirectoryPath))
                OpenConfiguration();
            else
                OpenMain();

            startup.IsStartup = false;
        }

        private static FileSearchMode GetUserFileSearchMode()
        {
            FileSearchMode result;
            if (Converts.Try<string, FileSearchMode>(Settings.Default.FileSearchMode, out result))
                return result;

            return FileSearchMode.StartsWith;
        }

        private static int GetUserFileSearchCount()
        {
            return Settings.Default.FileSearchCount;
        }

        #region Handling exceptions

        private void OnDispatcherUnhandledException(object sender, DispatcherUnhandledExceptionEventArgs e)
        {
            StringBuilder message = new StringBuilder();

            string exceptionMessage = e.Exception.ToString();
            if (exceptionMessage.Length > 800)
                exceptionMessage = exceptionMessage.Substring(0, 800);

            message.AppendLine(exceptionMessage);

            MessageBoxResult result = MessageBox.Show(message.ToString(), "Do you want to kill the aplication?", MessageBoxButton.YesNo);
            if (result == MessageBoxResult.Yes)
                Shutdown();

            e.Handled = true;
        }

        #endregion

        protected override void OnExit(ExitEventArgs e)
        {
            runHotKey.Dispose();
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

        public IEnumerable<string> GetList()
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
                ConfigurationViewModel viewModel = new ConfigurationViewModel(new SaveConfigurationCommandFactory(runHotKey));
                viewModel.SourceDirectoryPath = Settings.Default.SourceDirectoryPath;
                viewModel.PreferedApplicationPath = Settings.Default.PreferedApplicationPath;
                viewModel.FileSearchMode = GetUserFileSearchMode();
                viewModel.RunKey = runHotKey.FindKeyViewModel();
                configurationWindow = new ConfigurationWindow(viewModel, this, String.IsNullOrEmpty(Settings.Default.SourceDirectoryPath));
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

        public void OpenMain()
        {
            if (mainWindow == null)
            {
                string directoryPath = Settings.Default.SourceDirectoryPath;

                mainWindow = new MainWindow(this);
                mainWindow.Closing += OnMainWindowClosing;
                mainWindow.Closed += OnMainWindowClosed;

                MainViewModel viewModel = new MainViewModel(
                    new DelayedFileSearchService(
                        Dispatcher,
                        new PinnedForEmptyPatternFileSearchService(
                            //new LocalFileSearchService(directoryPath, this)
                            //new DirectFileSearchService(directoryPath, this),
                            new FileSystemWatcherSearchService(directoryPath, this),
                            this
                        )
                    ),
                    GetUserFileSearchMode
                );

                VsVersionLoader loader = new VsVersionLoader();
                loader.Add(viewModel);

                IFileCollection files = viewModel;
                foreach (string filePath in GetPinnedFiles())
                    files.Add(Path.GetFileNameWithoutExtension(filePath), filePath, true);

                mainWindow.ViewModel = viewModel;

                if (!String.IsNullOrEmpty(Settings.Default.PreferedApplicationPath))
                {
                    int index = 0;
                    foreach (ApplicationViewModel application in viewModel.Applications)
                    {
                        if (application.Path == Settings.Default.PreferedApplicationPath)
                        {
                            mainWindow.lvwApplications.SelectedIndex = index;
                            break;
                        }

                        index++;
                    }
                }
            }

            if (!startup.IsStartup || !startup.IsHidden)
            {
                mainWindow.Show();
                mainWindow.Activate();
            }
        }

        private void OnMainWindowClosing(object sender, CancelEventArgs e)
        {
            if (runHotKey.IsSet)
            {
                mainWindow.Hide();
                e.Cancel = true;
            }
        }

        private void OnMainWindowClosed(object sender, EventArgs e)
        {
            mainWindow.Closed -= OnMainWindowClosed;
            mainWindow.Closing -= OnMainWindowClosed;
            mainWindow = null;

            if (!runHotKey.IsSet && configurationWindow == null)
                Shutdown();
        }

        #endregion
    }
}
