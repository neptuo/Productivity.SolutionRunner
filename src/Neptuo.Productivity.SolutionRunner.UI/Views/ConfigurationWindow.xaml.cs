using Neptuo.Productivity.SolutionRunner.Services;
using Neptuo.Productivity.SolutionRunner.Services.Logging;
using Neptuo.Productivity.SolutionRunner.ViewModels;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using EventManager = Neptuo.Productivity.SolutionRunner.ViewModels.EventManager;

namespace Neptuo.Productivity.SolutionRunner.Views
{
    public partial class ConfigurationWindow : Window
    {
        private readonly INavigator navigator;

        public bool IsSaveRequired { get; private set; }

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
            IsSaveRequired = isSaveRequired;
            this.navigator = navigator;

            InitializeComponent();
            EventManager.ConfigurationSaved += OnConfigurationSaved;

            if (isSaveRequired)
                btnCancel.Content = "Exit";
        }

        private void OnConfigurationSaved(ConfigurationViewModel viewModel)
        {
            EventManager.ConfigurationSaved -= OnConfigurationSaved;
            IsSaveRequired = false;
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

        private void btnCancel_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void OnClosing(object sender, CancelEventArgs e)
        {
            if (IsSaveRequired)
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

        private void btnViewStatistics_Click(object sender, RoutedEventArgs e)
        {
            navigator.OpenStatistics();
        }

        private void OnLogReload() 
            => ViewModel.Troubleshooting.ReloadLogs();
    }
}
