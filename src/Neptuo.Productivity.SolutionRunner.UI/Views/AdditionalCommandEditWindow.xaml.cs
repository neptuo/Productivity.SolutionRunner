using Neptuo.Productivity.SolutionRunner.ViewModels;
using System;
using System.Collections.Generic;
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

namespace Neptuo.Productivity.SolutionRunner.Views
{
    public partial class AdditionalCommandEditWindow : Window
    {
        public AdditionalCommandEditViewModel ViewModel
        {
            get { return (AdditionalCommandEditViewModel)DataContext; }
            set { DataContext = value; }
        }

        public AdditionalCommandEditWindow(AdditionalCommandEditViewModel viewModel)
        {
            InitializeComponent();
            ViewModel = viewModel;
        }

        public AdditionalCommandEditWindow()
        {
            InitializeComponent();
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

        private void ApplicationEdit_DisplayNameChanged(object sender, RoutedEventArgs e)
        {
            ViewModel.IsNameChanged = true;
            ApplicationEdit.DisplayNameChanged -= ApplicationEdit_DisplayNameChanged;
        }
    }
}
