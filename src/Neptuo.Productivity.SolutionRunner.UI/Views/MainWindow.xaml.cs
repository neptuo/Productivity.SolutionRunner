using Neptuo.Linq.Expressions;
using Neptuo.Productivity.SolutionRunner.Services;
using Neptuo.Productivity.SolutionRunner.ViewModels;
using Neptuo.Productivity.SolutionRunner.Views.Controls;
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

        public MainWindow(INavigator navigator)
        {
            Ensure.NotNull(navigator, "navigator");
            this.navigator = navigator;

            InitializeComponent();
            EventManager.FilePinned += OnFilePinned;
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
                applicationsView.SortDescriptions.Add(new SortDescription(ApplicationNamePropertyName, ListSortDirection.Ascending));
                applicationsView.CollectionChanged += OnApplicationsViewCollectionChanged;
                filesView.Refresh();

                lvwApplications.SelectedIndex = 0;
            }
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
                else if(e.Key == Key.Home)
                {
                    lvwFiles.SelectedIndex = 0;
                    e.Handled = true;
                }
                else if(e.Key == Key.End)
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
                    if(Keyboard.IsKeyDown(Key.LeftShift))
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
                if (application != null)
                {
                    FileViewModel file = lvwFiles.SelectedItem as FileViewModel;
                    if (file != null)
                    {
                        Process.Start(new ProcessStartInfo(application.Path, file.Path));
                        Close();
                    }
                }
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

        private void OnPreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            if(WindowDrag.TryMove(e))
                DragMove();
        }

        protected override void OnClosed(EventArgs e)
        {
            base.OnClosed(e);

            if (ViewModel != null)
                ViewModel.Dispose();
        }
    }
}
