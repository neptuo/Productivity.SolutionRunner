using System;
using System.Collections;
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

namespace Neptuo.Productivity.SolutionRunner.Views.Controls
{
    /// <summary>
    /// Interaction logic for ListView.xaml
    /// </summary>
    public partial class ListView : UserControl
    {
        public string Label
        {
            get { return (string)GetValue(LabelProperty); }
            set { SetValue(LabelProperty, value); }
        }

        public static readonly DependencyProperty LabelProperty = DependencyProperty.Register(
            "Label", 
            typeof(string), 
            typeof(ListView), 
            new PropertyMetadata(null)
        );


        public string LabelDetail
        {
            get { return (string)GetValue(LabelDetailProperty); }
            set { SetValue(LabelDetailProperty, value); }
        }

        public static readonly DependencyProperty LabelDetailProperty = DependencyProperty.Register(
            "LabelDetail", 
            typeof(string), 
            typeof(ListView), 
            new PropertyMetadata(null)
        );


        public IEnumerable ItemsSource
        {
            get { return (IEnumerable)GetValue(ItemsSourceProperty); }
            set { SetValue(ItemsSourceProperty, value); }
        }

        public static readonly DependencyProperty ItemsSourceProperty = DependencyProperty.Register(
            "ItemsSource", 
            typeof(IEnumerable), 
            typeof(ListView), 
            new PropertyMetadata(null)
        );


        public ICommand CreateCommand
        {
            get { return (ICommand)GetValue(CreateCommandProperty); }
            set { SetValue(CreateCommandProperty, value); }
        }

        public static readonly DependencyProperty CreateCommandProperty = DependencyProperty.Register(
            "CreateCommand", 
            typeof(ICommand), 
            typeof(ListView), 
            new PropertyMetadata(null)
        );


        public string CreateCommandToolTip
        {
            get { return (string)GetValue(CreateCommandToolTipProperty); }
            set { SetValue(CreateCommandToolTipProperty, value); }
        }

        public static readonly DependencyProperty CreateCommandToolTipProperty = DependencyProperty.Register(
            "CreateCommandToolTip", 
            typeof(string), 
            typeof(ListView), 
            new PropertyMetadata(null)
        );


        public DataTemplate ItemTemplate
        {
            get { return (DataTemplate)GetValue(ItemTemplateProperty); }
            set { SetValue(ItemTemplateProperty, value); }
        }

        public static readonly DependencyProperty ItemTemplateProperty = DependencyProperty.Register(
            "ItemTemplate", 
            typeof(DataTemplate), 
            typeof(ListView), 
            new PropertyMetadata(null)
        );


        public object NoData
        {
            get { return GetValue(NoDataProperty); }
            set { SetValue(NoDataProperty, value); }
        }

        public static readonly DependencyProperty NoDataProperty = DependencyProperty.Register(
            "NoData", 
            typeof(object), 
            typeof(ListView), 
            new PropertyMetadata("No data to display. Add some using '+' button...")
        );


        public ListView()
        {
            InitializeComponent();
        }
    }
}
