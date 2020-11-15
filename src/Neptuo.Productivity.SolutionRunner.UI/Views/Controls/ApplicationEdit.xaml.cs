using Neptuo.Productivity.SolutionRunner.Services.Applications;
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
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.IO;
using System.Drawing;
using System.Security.Policy;
using OpenFileDialog = System.Windows.Forms.OpenFileDialog;
using DialogResult = System.Windows.Forms.DialogResult;

namespace Neptuo.Productivity.SolutionRunner.Views.Controls
{
    public partial class ApplicationEdit : UserControl
    {
        public string DisplayName
        {
            get { return (string)GetValue(DisplayNameProperty); }
            set { SetValue(DisplayNameProperty, value); }
        }

        public static readonly DependencyProperty DisplayNameProperty = DependencyProperty.Register(
            "DisplayName", 
            typeof(string), 
            typeof(ApplicationEdit), 
            new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault)
        );

        public event RoutedEventHandler DisplayNameChanged
        {
            add { AddHandler(DisplayNameChangedEvent, value); }
            remove { RemoveHandler(DisplayNameChangedEvent, value); }
        }

        public static readonly RoutedEvent DisplayNameChangedEvent = EventManager.RegisterRoutedEvent(
            "DisplayNameChanged", 
            RoutingStrategy.Bubble, 
            typeof(RoutedEventHandler), 
            typeof(ApplicationEdit)
        );


        public string FilePath
        {
            get { return (string)GetValue(FilePathProperty); }
            set { SetValue(FilePathProperty, value); }
        }

        public static readonly DependencyProperty FilePathProperty = DependencyProperty.Register(
            "FilePath", 
            typeof(string), 
            typeof(ApplicationEdit), 
            new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault)
        );


        public string FileArguments
        {
            get { return (string)GetValue(FileArgumentsProperty); }
            set { SetValue(FileArgumentsProperty, value); }
        }

        public static readonly DependencyProperty FileArgumentsProperty = DependencyProperty.Register(
            "FileArguments", 
            typeof(string), 
            typeof(ApplicationEdit), 
            new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault)
        );


        public string IconData
        {
            get { return (string)GetValue(IconDataProperty); }
            set { SetValue(IconDataProperty, value); }
        }

        public static readonly DependencyProperty IconDataProperty = DependencyProperty.Register(
            "IconData", 
            typeof(string), 
            typeof(ApplicationEdit), 
            new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault)
        );


        public ApplicationEdit()
        {
            InitializeComponent();
        }

        protected override void OnGotFocus(RoutedEventArgs e)
        {
            base.OnGotFocus(e);
            Focus();
        }

        public new void Focus()
        {
            fbrPath.Focus();
        }

        private void tbxName_KeyUp(object sender, KeyEventArgs e)
        {
            RaiseEvent(new RoutedEventArgs(DisplayNameChangedEvent));
        }
    }
}
