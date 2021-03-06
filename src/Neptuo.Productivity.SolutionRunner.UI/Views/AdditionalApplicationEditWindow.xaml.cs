﻿using Neptuo.Productivity.SolutionRunner.ViewModels;
using Neptuo.Productivity.SolutionRunner.Views.Controls;
using System;
using System.Collections.Generic;
using System.ComponentModel;
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
    public partial class AdditionalApplicationEditWindow : Window
    {
        public AdditionalApplicationEditViewModel ViewModel
        {
            get { return (AdditionalApplicationEditViewModel)DataContext; }
            set { DataContext = value; }
        }

        public AdditionalApplicationEditWindow(AdditionalApplicationEditViewModel viewModel)
        {
            InitializeComponent();
            ViewModel = viewModel;
        }

        protected override void OnActivated(EventArgs e)
        {
            base.OnActivated(e);
            ApplicationEdit.Focus();
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
