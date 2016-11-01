using Neptuo.Collections.Specialized;
using Neptuo.Linq.Expressions;
using Neptuo.Productivity.SolutionRunner.Properties;
using Neptuo.Productivity.SolutionRunner.Services;
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

namespace Neptuo.Productivity.SolutionRunner.Views
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private static readonly string FileIsPinnedPropertyName = TypeHelper.PropertyName<FileViewModel, bool>(vm => vm.IsPinned);
        private static readonly string FileNamePropertyName = TypeHelper.PropertyName<FileViewModel, string>(vm => vm.Name);
        private static readonly string ApplicationIsMainPropertyName = TypeHelper.PropertyName<ApplicationViewModel, bool>(vm => vm.IsMain);
        private static readonly string ApplicationNamePropertyName = TypeHelper.PropertyName<ApplicationViewModel, string>(vm => vm.Name);

        private readonly INavigator navigator;

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

        public MainWindow(INavigator navigator)
        {
            Ensure.NotNull(navigator, "navigator");
            this.navigator = navigator;

            InitializeComponent();
            EventManager.FilePinned += OnFilePinned;
        }

        private void RunSolution(ApplicationViewModel application, FileViewModel file)
        {
            if (application != null)
            {
                if (Settings.Default.IsLastUsedApplicationSavedAsPrefered)
                {
                    if (application.Path != Settings.Default.PreferedApplicationPath)
                    {
                        Settings.Default.PreferedApplicationPath = application.Path;
                        Settings.Default.Save();
                    }
                }
                else
                {
                    TrySelectPreferedApplication();
                }

                if (file == null)
                {
                    Process.Start(application.Path);
                }
                else
                {
                    string arguments = file.Path;
                    if (!String.IsNullOrEmpty(application.Arguments))
                    {
                        TokenWriter writer = new TokenWriter(application.Arguments);
                        arguments = writer.Format(new KeyValueCollection()
                            .Add("FilePath", file.Path)
                            .Add("DirectoryPath", System.IO.Path.GetDirectoryName(file.Path))
                        );
                    }

                    Process.Start(new ProcessStartInfo(application.Path, arguments));
                }

                Close();
            }
        }

        private void OnFilePinned(FileViewModel viewModel)
        {
            if (ViewModel != null)
            {
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
                filesView.SortDescriptions.Add(new SortDescription(FileIsPinnedPropertyName, ListSortDirection.Descending));
                filesView.SortDescriptions.Add(new SortDescription(FileNamePropertyName, ListSortDirection.Ascending));
                filesView.CollectionChanged += OnFilesViewCollectionChanged;
                filesView.Refresh();

                lvwApplications.SelectedIndex = 0;
            }

            ICollectionView applicationsView = CollectionViewSource.GetDefaultView(viewModel.Applications);
            if (applicationsView != null)
            {
                applicationsView.SortDescriptions.Add(new SortDescription(ApplicationIsMainPropertyName, ListSortDirection.Descending));
                applicationsView.SortDescriptions.Add(new SortDescription(ApplicationNamePropertyName, ListSortDirection.Ascending));
                applicationsView.CollectionChanged += OnApplicationsViewCollectionChanged;
                filesView.Refresh();

                lvwApplications.SelectedIndex = 0;
            }

            InitializeWidth();
            viewModel.InitializeAsync();
        }

        private void InitializeWidth()
        {
            FileWidth = FileWidthDefaultValue;
            Task.Factory.StartNew(() =>
            {
                Thread.Sleep(10);
                DispatcherHelper.Run(Dispatcher, () => FileWidth = Math.Max(grdMain.ActualWidth, FileWidthDefaultValue) - 20);
            });
        }

        private void OnFilesViewCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e.Action == NotifyCollectionChangedAction.Add)
            {
                Task.Factory.StartNew(() =>
                {
                    Thread.Sleep(10);
                    DispatcherHelper.Run(Dispatcher, () => lvwFiles.SelectedIndex = 0);
                });
            }
        }

        private void OnApplicationsViewCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e.Action == NotifyCollectionChangedAction.Add)
            {
                Task.Factory.StartNew(() =>
                {
                    Thread.Sleep(10);
                    DispatcherHelper.Run(Dispatcher, () => lvwApplications.SelectedIndex = 0);
                });
            }
        }

        protected override void OnActivated(EventArgs e)
        {
            base.OnActivated(e);
            tbxSearch.Focus();
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
                else if (e.Key == Key.Home && !IsModifierKeyPressed())
                {
                    lvwFiles.SelectedIndex = 0;
                    e.Handled = true;
                }
                else if (e.Key == Key.End && !IsModifierKeyPressed())
                {
                    lvwFiles.SelectedIndex = lvwFiles.Items.Count - 1;
                    e.Handled = true;
                }
                else if (e.Key == Key.S && Keyboard.IsKeyDown(Key.LeftCtrl))
                {
                    FileViewModel file = lvwFiles.SelectedItem as FileViewModel;
                    if (file != null)
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
            //else if (e.Key == Key.F1)
            //{
            //    btnHelp.RaiseEvent(new RoutedEventArgs(Button.ClickEvent));
            //}

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

        private void OnPreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            if (WindowDrag.TryMove(e))
                DragMove();
        }

        public void TrySelectPreferedApplication()
        {
            if (!String.IsNullOrEmpty(Settings.Default.PreferedApplicationPath))
            {
                int index = 0;
                ICollectionView applicationsView = CollectionViewSource.GetDefaultView(ViewModel.Applications);
                if (applicationsView != null)
                {
                    foreach (ApplicationViewModel application in applicationsView)
                    {
                        if (application.Path == Settings.Default.PreferedApplicationPath)
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
            ApplicationViewModel application = lvwApplications.SelectedItem as ApplicationViewModel;
            if (application != null)
                RunSolution(application, null);
        }

        private void cocFile_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            ApplicationViewModel application = lvwApplications.SelectedItem as ApplicationViewModel;
            FileViewModel file = lvwFiles.SelectedItem as FileViewModel;

            if (application != null && file != null)
                RunSolution(application, file);
        }

        protected override void OnClosed(EventArgs e)
        {
            base.OnClosed(e);

            if (ViewModel != null)
                ViewModel.Dispose();
        }

        private void OnAutoSelectApplicationVersionSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            TryAutoSelectApplicationVersion();
        }

        public void TryAutoSelectApplicationVersion()
        {
            FileViewModel file = lvwFiles.SelectedItem as FileViewModel;
            if (file != null && file.Version != null)
            {
                ApplicationViewModel application = ViewModel.Applications
                    .FirstOrDefault(a => a.Version != null && a.Version.Major == file.Version.Major);

                if (application != null && lvwApplications.SelectedItem != application)
                    lvwApplications.SelectedItem = application;
            }
        }
    }
}
