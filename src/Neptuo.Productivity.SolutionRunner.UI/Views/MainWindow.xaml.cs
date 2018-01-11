using Neptuo;
using Neptuo.Collections.Specialized;
using Neptuo.Linq.Expressions;
using Neptuo.Productivity.SolutionRunner.Properties;
using Neptuo.Productivity.SolutionRunner.Services;
using Neptuo.Productivity.SolutionRunner.Services.Execution;
using Neptuo.Productivity.SolutionRunner.Services.Positions;
using Neptuo.Productivity.SolutionRunner.ViewModels;
using Neptuo.Productivity.SolutionRunner.Views.Controls;
using Neptuo.Text.Tokens;
using Neptuo.Windows.Threading;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using EventManager = Neptuo.Productivity.SolutionRunner.ViewModels.EventManager;
using AccessKeyPressedEventArgs = Neptuo.Productivity.SolutionRunner.Views.Controls.AccessKeyPressedEventArgs;

namespace Neptuo.Productivity.SolutionRunner.Views
{
    public partial class MainWindow : Window, IPositionTarget
    {
        private readonly INavigator navigator;
        private readonly Settings settings;
        private readonly ProcessService processService;
        private readonly bool isClosedAfterStartingProcess;

        private readonly ThrottlingHelper fileThrottler;
        private readonly ThrottlingHelper applicationThrottler;

        public DispatcherHelper DispatcherHelper { get; private set; }

        public MainViewModel ViewModel
        {
            get { return (MainViewModel)DataContext; }
            set
            {
                DataContext = value;
                if (value != null)
                    InitializeViewModel(value);
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
                    if (value)
                        lvwFiles.SelectionChanged += OnAutoSelectApplicationVersionSelectionChanged;
                    else
                        lvwFiles.SelectionChanged -= OnAutoSelectApplicationVersionSelectionChanged;
                }

                if (value)
                    TryAutoSelectApplicationVersion();
            }
        }

        #region FileWidth

        public const double FileWidthDefaultValue = 476d;

        public double FileWidth
        {
            get { return (double)GetValue(FileWidthProperty); }
            set { SetValue(FileWidthProperty, value); }
        }

        public static readonly DependencyProperty FileWidthProperty =
            DependencyProperty.Register("FileWidth", typeof(double), typeof(MainWindow), new PropertyMetadata(FileWidthDefaultValue));

        #endregion

        internal MainWindow(INavigator navigator, Settings settings, ProcessService processService, bool isClosedAfterStartingProcess)
        {
            Ensure.NotNull(navigator, "navigator");
            Ensure.NotNull(settings, "settings");
            Ensure.NotNull(processService, "processService");
            this.navigator = navigator;
            this.settings = settings;
            this.processService = processService;
            this.isClosedAfterStartingProcess = isClosedAfterStartingProcess;

            InitializeComponent();
            AccessKey.AddPressedHandler(this, OnAccessKeyPressed);
            AccessKey.AddPressingHandler(this, OnAccessKeyPressing);
            EventManager.FilePinned += OnFilePinned;
            DispatcherHelper = new DispatcherHelper(Dispatcher);

            fileThrottler = new ThrottlingHelper(DispatcherHelper, () => lvwFiles.SelectedIndex = 0, 0);
            applicationThrottler = new ThrottlingHelper(DispatcherHelper, () => lvwApplications.SelectedIndex = 0, 0);
        }

        private void RunSolution(ApplicationViewModel application, FileViewModel file)
        {
            if (application != null)
            {
                if (settings.IsLastUsedApplicationSavedAsPrefered)
                {
                    if (application.Path != settings.PreferedApplicationPath)
                    {
                        settings.PreferedApplicationPath = application.Path;
                        settings.Save();
                    }
                }
                else
                {
                    TrySelectPreferedApplication();
                }

                processService.Run(application, file);

                if (isClosedAfterStartingProcess)
                    Close();
            }
        }

        private void OnFilePinned(FileViewModel viewModel)
        {
            if (ViewModel != null)
            {
                // Update sorting.
                ICollectionView filesView = CollectionViewSource.GetDefaultView(ViewModel.Files);
                if (filesView != null)
                    filesView.Refresh();
            }
        }

        private void InitializeViewModel(MainViewModel viewModel)
        {
            ICollectionView filesView = CollectionViewSource.GetDefaultView(viewModel.Files);
            if (filesView != null)
            {
                filesView.SortDescriptions.Add(new SortDescription(nameof(FileViewModel.IsPinned), ListSortDirection.Descending));
                filesView.SortDescriptions.Add(new SortDescription(nameof(FileViewModel.Name), ListSortDirection.Ascending));
                filesView.SortDescriptions.Add(new SortDescription(nameof(FileViewModel.Path), ListSortDirection.Ascending));
                filesView.CollectionChanged += OnFilesViewCollectionChanged;
                filesView.Refresh();

                lvwApplications.SelectedIndex = 0;
            }

            ICollectionView applicationsView = CollectionViewSource.GetDefaultView(viewModel.Applications);
            if (applicationsView != null)
            {
                applicationsView.SortDescriptions.Add(new SortDescription(nameof(ApplicationViewModel.IsMain), ListSortDirection.Descending));
                applicationsView.SortDescriptions.Add(new SortDescription(nameof(ApplicationViewModel.Name), ListSortDirection.Ascending));
                applicationsView.SortDescriptions.Add(new SortDescription(nameof(ApplicationViewModel.Path), ListSortDirection.Ascending));
                applicationsView.CollectionChanged += OnApplicationsViewCollectionChanged;
                filesView.Refresh();

                lvwApplications.SelectedIndex = 0;
            }

            InitializeWidth();
            viewModel.InitializeAsync().ContinueWith(t =>
            {
                if (t.IsFaulted)
                    DispatcherHelper.Run(() => App.ShowExceptionDialog(t.Exception));
                else
                    DispatcherHelper.Run(FocusTextBox);
            });
        }

        private void InitializeWidth()
        {
            lvwApplications.SizeChanged += (e, sender) => UpdateFileWidth();

            FileWidth = FileWidthDefaultValue;
            Task.Factory.StartNew(() =>
            {
                Thread.Sleep(10);
                DispatcherHelper.Run(UpdateFileWidth);
            });
        }

        private void UpdateFileWidth()
        {
            FileWidth = Math.Max(lvwApplications.ActualWidth, FileWidthDefaultValue) - 20;
        }

        private void OnFilesViewCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e.Action == NotifyCollectionChangedAction.Add)
                fileThrottler.Run();
        }

        private void OnApplicationsViewCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e.Action == NotifyCollectionChangedAction.Add)
                applicationThrottler.Run();
        }

        protected override void OnActivated(EventArgs e)
        {
            base.OnActivated(e);
            ClearAccessKeyActiveFlags();
            FocusTextBox();
        }

        private void FocusTextBox()
        {
            tbxSearch.Focus();
            tbxSearch.CaretIndex = tbxSearch.Text.Length;
        }

        private void btnConfiguration_Click(object sender, RoutedEventArgs e)
        {
            navigator.OpenConfiguration();
            Close();
        }

        private void OnPreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (ViewModel == null || ViewModel.IsLoading)
                return;

            if (e.Key == Key.F1)
            {
                btnConfiguration_Click(sender, new RoutedEventArgs(e.RoutedEvent, sender));
                e.Handled = true;
            }

            if (lvwFiles.Items.Count > 0)
            {
                if (e.Key == Key.Down)
                {
                    lvwFiles.SelectedIndex = (lvwFiles.SelectedIndex + 1) % lvwFiles.Items.Count;
                    e.Handled = true;
                }
                else if (e.Key == Key.Up)
                {
                    int newIndex = lvwFiles.SelectedIndex - 1;
                    if (newIndex < 0)
                        newIndex = lvwFiles.Items.Count - 1;

                    lvwFiles.SelectedIndex = newIndex;
                    e.Handled = true;
                }
                else if (e.Key == Key.PageUp && !IsModifierKeyPressed())
                {
                    lvwFiles.SelectedIndex = 0;
                    e.Handled = true;
                }
                else if (e.Key == Key.PageDown && !IsModifierKeyPressed())
                {
                    lvwFiles.SelectedIndex = lvwFiles.Items.Count - 1;
                    e.Handled = true;
                }
                else if (e.Key == Key.S && Keyboard.IsKeyDown(Key.LeftCtrl))
                {
                    if (lvwFiles.SelectedItem is FileViewModel file)
                    {
                        if (!file.IsPinned && file.PinCommand.CanExecute(null))
                            file.PinCommand.Execute(null);
                        else if (file.IsPinned && file.UnPinCommand.CanExecute(null))
                            file.UnPinCommand.Execute(null);
                    }
                }
            }

            if (lvwApplications.Items.Count > 0)
            {
                if (e.Key == Key.Tab && Keyboard.IsKeyDown(Key.LeftCtrl))
                {
                    if (Keyboard.IsKeyDown(Key.LeftShift))
                    {
                        int newIndex = lvwApplications.SelectedIndex - 1;
                        if (newIndex < 0)
                            newIndex = lvwApplications.Items.Count - 1;

                        lvwApplications.SelectedIndex = newIndex;
                        e.Handled = true;
                    }
                    else
                    {
                        lvwApplications.SelectedIndex = (lvwApplications.SelectedIndex + 1) % lvwApplications.Items.Count;
                        e.Handled = true;
                    }
                }
            }

            if (e.Key == Key.Enter)
            {
                // On ENTER, open sln in selected application.
                ApplicationViewModel application = lvwApplications.SelectedItem as ApplicationViewModel;
                FileViewModel file = lvwFiles.SelectedItem as FileViewModel;

                if (application != null && file != null)
                    RunSolution(application, file);
            }
            else if (e.Key == Key.Escape)
            {
                // On ESCAPE, close app or clear search box.
                if (String.IsNullOrEmpty(ViewModel.SearchPattern))
                    Close();
                else
                    ViewModel.SearchPattern = null;

                e.Handled = true;
            }

            if (e.Key == Key.C && Keyboard.IsKeyDown(Key.LeftCtrl) && tbxSearch.SelectionLength == 0)
            {
                string text = null;
                if (Keyboard.IsKeyDown(Key.LeftShift))
                {
                    if (lvwApplications.SelectedItem is ApplicationViewModel application)
                        text = application.Path;
                }
                else
                {
                    if (lvwFiles.SelectedItem is FileViewModel file)
                        text = file.Path;
                }

                if (text != null)
                {
                    Clipboard.SetText(text);
                    e.Handled = true;
                }
            }

            // Lastly, if non of the hot keys was pressed. Try to focus search box.
            if (!e.Handled && !tbxSearch.IsFocused)
            {
                tbxSearch.Focus();
            }
        }

        private bool IsModifierKeyPressed()
        {
            if (Keyboard.IsKeyDown(Key.LeftAlt) || Keyboard.IsKeyDown(Key.RightAlt))
                return true;

            if (Keyboard.IsKeyDown(Key.LeftCtrl) || Keyboard.IsKeyDown(Key.RightCtrl))
                return true;

            if (Keyboard.IsKeyDown(Key.LeftShift) || Keyboard.IsKeyDown(Key.RightShift))
                return true;

            return false;
        }

        private void OnAccessKeyPressed(object sender, AccessKeyPressedEventArgs e)
        {
            IApplication application = ViewModel.Applications.Find(e.Keys);
            if (application != null)
            {
                processService.Run(application, lvwFiles.SelectedItem as FileViewModel);

                if (isClosedAfterStartingProcess)
                    Close();
            }

            ClearAccessKeyActiveFlags();
        }

        private void ClearAccessKeyActiveFlags()
        {
            foreach (ApplicationViewModel application in ViewModel.Applications)
            {
                application.IsHotKeyActive = false;
                foreach (ApplicationCommandViewModel command in application.Commands)
                    command.IsHotKeyActive = false;
            }
        }

        private void OnAccessKeyPressing(object sender, AccessKeyPressingEventArgs e)
        {
            List<Key> keys = new List<Key>(e.PreviousKeys);
            keys.Add(e.LastKey);

            IApplication model = ViewModel.Applications.Find(keys);
            ApplicationViewModel application = model as ApplicationViewModel;
            if (application != null)
            {
                application.IsHotKeyActive = true;
                return;
            }

            ApplicationCommandViewModel command = model as ApplicationCommandViewModel;
            if (command != null)
            {
                command.IsHotKeyActive = true;
                return;
            }

            e.IsCancelled = true;
        }

        private void OnPreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            if (WindowDrag.TryMove(e))
                DragMove();
        }

        public void TrySelectPreferedApplication()
        {
            if (!String.IsNullOrEmpty(settings.PreferedApplicationPath))
            {
                int index = 0;
                ICollectionView applicationsView = CollectionViewSource.GetDefaultView(ViewModel.Applications);
                if (applicationsView != null)
                {
                    foreach (ApplicationViewModel application in applicationsView)
                    {
                        if (application.Path == settings.PreferedApplicationPath)
                        {
                            lvwApplications.SelectedIndex = index;
                            break;
                        }

                        index++;
                    }
                }
            }
        }

        private void cocApplication_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (lvwApplications.SelectedItem is ApplicationViewModel application)
                RunSolution(application, null);
        }

        private void cocFile_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (lvwApplications.SelectedItem is ApplicationViewModel application && lvwFiles.SelectedItem is FileViewModel file)
                RunSolution(application, file);
        }

        protected override void OnClosed(EventArgs e)
        {
            base.OnClosed(e);
            AccessKey.RemovePressedHandler(this, OnAccessKeyPressed);
            AccessKey.RemovePressingHandler(this, OnAccessKeyPressing);
            EventManager.FilePinned -= OnFilePinned;

            if (ViewModel != null)
                ViewModel.Dispose();
        }

        private void OnAutoSelectApplicationVersionSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            TryAutoSelectApplicationVersion();
        }

        public void TryAutoSelectApplicationVersion()
        {
            if (lvwFiles.SelectedItem is FileViewModel file && file.Version != null)
            {
                Version toSelect = file.Version;
                Version minimal = settings.GetAutoSelectApplicationMinimalVersion();
                if (minimal != null && minimal > file.Version)
                    toSelect = minimal;

                ApplicationViewModel application = ViewModel.Applications
                    .FirstOrDefault(a => a.Version != null && a.Version.Major == toSelect.Major);

                if (application != null && lvwApplications.SelectedItem != application)
                    lvwApplications.SelectedItem = application;
            }
        }
    }
}
