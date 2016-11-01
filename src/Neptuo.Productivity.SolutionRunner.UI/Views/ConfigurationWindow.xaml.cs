using Neptuo.Productivity.SolutionRunner.Services;
using Neptuo.Productivity.SolutionRunner.ViewModels;
using Neptuo.Productivity.SolutionRunner.Views.Controls;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using EventManager = Neptuo.Productivity.SolutionRunner.ViewModels.EventManager;
using Neptuo.Productivity.SolutionRunner.Services.Applications;

namespace Neptuo.Productivity.SolutionRunner.Views
{
    /// <summary>
    /// Interaction logic for ConfigurationWindow.xaml
    /// </summary>
    public partial class ConfigurationWindow : Window
    {
        private readonly INavigator navigator;
        private bool isSaveRequired;

        public ConfigurationViewModel ViewModel
        {
            get { return (ConfigurationViewModel)DataContext; }
            set { DataContext = value; }
        }

        public ConfigurationWindow(ConfigurationViewModel viewModel, INavigator navigator, bool isSaveRequired)
        {
            Ensure.NotNull(viewModel, "viewModel");
            Ensure.NotNull(navigator, "navigator");
            ViewModel = viewModel;
            this.navigator = navigator;
            this.isSaveRequired = isSaveRequired;

            InitializeComponent();
            EventManager.ConfigurationSaved += OnConfigurationSaved;
        }

        private void OnConfigurationSaved(ConfigurationViewModel viewModel)
        {
            EventManager.ConfigurationSaved -= OnConfigurationSaved;
            isSaveRequired = false;
            Close();
        }

        protected override void OnActivated(EventArgs e)
        {
            base.OnActivated(e);
            //tbxSourceDirectory.Focus();
            dbrSourceDirectory.Focus();
        }

        private void OnPreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Escape)
            {
                Close();
                e.Handled = true;
            }
        }

        private void OnPreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            if (WindowDrag.TryMove(e))
                DragMove();
        }

        private void btnCancel_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void OnClosing(object sender, CancelEventArgs e)
        {
            if (isSaveRequired)
            {
                string message = "Source root directory is not set. Terminate the application?";
                string caption = "Configuration";
                e.Cancel = MessageBox.Show(this, message, caption, MessageBoxButton.YesNo) == MessageBoxResult.No;
                return;
            }

            navigator.OpenMain();
        }

        private void btnAbout_Click(object sender, RoutedEventArgs e)
        {
            Process.Start("http://www.neptuo.com/project/desktop/solutionrunner");
        }

        //private AdditionalApplicationModel editedApplicationModel;

        //private void btnAddApplication_Click(object sender, RoutedEventArgs e)
        //{
        //    editedApplicationModel = null;
        //    navigator.OpenAdditionalApplicationEdit(null, OnApplicationSaved);
        //}

        //private void OnApplicationSaved(AdditionalApplicationModel model)
        //{
        //    if (model != null)
        //    {
        //        if (editedApplicationModel != null && editedApplicationModel != model)
        //        {
        //            // TODO: Check for duplicities.
        //            // TODO: Update...
        //        }
        //        else
        //        {
        //            // TODO: Check for duplicities.
        //            ViewModel.AdditionalApplications.Add(new AdditionalApplicationListViewModel(model));
        //        }
        //    }
        //}
    }
}
