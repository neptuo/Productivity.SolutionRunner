using Neptuo;
using Neptuo.Activators;
using Neptuo.Productivity.SolutionRunner.Services;
using Neptuo.Productivity.SolutionRunner.Services.Execution;
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
        private readonly ProcessService processes;

        public bool IsSaveRequired { get; private set; }

        public ConfigurationViewModel ViewModel
        {
            get { return (ConfigurationViewModel)DataContext; }
            set { DataContext = value; }
        }

        public ConfigurationWindow(ConfigurationViewModel viewModel, INavigator navigator, ProcessService processes, bool isSaveRequired)
        {
            Ensure.NotNull(viewModel, "viewModel");
            Ensure.NotNull(navigator, "navigator");
            Ensure.NotNull(processes, "processes");
            ViewModel = viewModel;
            IsSaveRequired = isSaveRequired;
            this.navigator = navigator;
            this.processes = processes;

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

        private void OnLogReload()
            => ViewModel.Troubleshooting.ReloadLogs();

        private bool areStatisticsLoaded = false;

        private void tbcMain_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {
            if (tbcMain.SelectedItem != null && tbcMain.SelectedItem == tbiStatistics && !areStatisticsLoaded)
            {
                areStatisticsLoaded = true;
                ViewModel.Statistics.Reload.Execute(null);
            }
        }
    }
}
