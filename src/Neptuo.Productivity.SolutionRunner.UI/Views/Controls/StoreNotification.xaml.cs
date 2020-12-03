using Neptuo.Productivity.SolutionRunner.Properties;
using System;
using System.Collections.Generic;
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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Neptuo.Productivity.SolutionRunner.Views.Controls
{
    public partial class StoreNotification : UserControl
    {
        public double ContentWidth
        {
            get { return (double)GetValue(ContentWidthProperty); }
            set { SetValue(ContentWidthProperty, value); }
        }

        public static readonly DependencyProperty ContentWidthProperty = DependencyProperty.Register(
            "ContentWidth", 
            typeof(double), 
            typeof(StoreNotification), 
            new PropertyMetadata(0d, OnContentWidthChanged)
        );

        private static void OnContentWidthChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            StoreNotification view = (StoreNotification)d;
            view.dpnContent.Width = view.ContentWidth;
        }


        public bool IsCloseable
        {
            get { return (bool)GetValue(IsCloseableProperty); }
            set { SetValue(IsCloseableProperty, value); }
        }

        public static readonly DependencyProperty IsCloseableProperty = DependencyProperty.Register(
            "IsCloseable", 
            typeof(bool), 
            typeof(StoreNotification), 
            new PropertyMetadata(true, OnIsCloseableChanged)
        );

        private static void OnIsCloseableChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            StoreNotification view = (StoreNotification)d;
            view.btnCloseMoveToStore.Visibility = view.IsCloseable
                ? Visibility.Visible
                : Visibility.Collapsed;

            view.EnsureStoreNotification();
        }

        public StoreNotification()
        {
            InitializeComponent();
            EnsureStoreNotification();
        }

        private void btnMoveToStore_Click(object sender, RoutedEventArgs e) 
            => Process.Start("https://www.neptuo.com/blog/2020/12/productivity-solution-runner-v2");

        private void btnCloseMoveToStore_Click(object sender, RoutedEventArgs e)
        {
            Configuration.Default.IsStoreNofiticationVisible = false;
            Configuration.Default.Save();
            EnsureStoreNotification();
        }

        private void EnsureStoreNotification() => grdStoreNotification.Visibility = !IsCloseable || Configuration.Default.IsStoreNofiticationVisible
            ? Visibility.Visible
            : Visibility.Collapsed;
    }
}
