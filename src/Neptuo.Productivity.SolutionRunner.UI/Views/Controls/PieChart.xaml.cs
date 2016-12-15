using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Markup;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Neptuo.Productivity.SolutionRunner.Views.Controls
{
    /// <summary>
    /// Interaction logic for PieChart.xaml
    /// </summary>
    public partial class PieChart : ItemsControl
    {
        public double Thickness
        {
            get { return (double)GetValue(ThicknessProperty); }
            set { SetValue(ThicknessProperty, value); }
        }

        public static readonly DependencyProperty ThicknessProperty = DependencyProperty.Register(
            "Thickness",
            typeof(double),
            typeof(PieChart),
            new PropertyMetadata(10d, OnThicknessChanged)
        );

        private static void OnThicknessChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            PieChart pieChart = (PieChart)d;
            pieChart.Update();
        }

        public string ValuePath
        {
            get { return (string)GetValue(ValuePathProperty); }
            set { SetValue(ValuePathProperty, value); }
        }

        public static readonly DependencyProperty ValuePathProperty = DependencyProperty.Register(
            "ValuePath",
            typeof(string),
            typeof(PieChart),
            new PropertyMetadata(null)
        );

        public string ForegroundPath
        {
            get { return (string)GetValue(ForegroundPathProperty); }
            set { SetValue(ForegroundPathProperty, value); }
        }

        public static readonly DependencyProperty ForegroundPathProperty = DependencyProperty.Register(
            "ForegroundPath",
            typeof(string),
            typeof(PieChart),
            new PropertyMetadata(null)
        );

        public string LabelPath
        {
            get { return (string)GetValue(LabelPathProperty); }
            set { SetValue(LabelPathProperty, value); }
        }

        public static readonly DependencyProperty LabelPathProperty = DependencyProperty.Register(
            "LabelPath", 
            typeof(string), 
            typeof(PieChart), 
            new PropertyMetadata(null)
        );

        public IValueConverter LabelConverter
        {
            get { return (IValueConverter)GetValue(LabelConverterProperty); }
            set { SetValue(LabelConverterProperty, value); }
        }

        public static readonly DependencyProperty LabelConverterProperty = DependencyProperty.Register(
            "LabelConverter", 
            typeof(IValueConverter), 
            typeof(PieChart), 
            new PropertyMetadata(null)
        );

        public double LabelOffset
        {
            get { return (double)GetValue(LabelOffsetProperty); }
            set { SetValue(LabelOffsetProperty, value); }
        }

        public static readonly DependencyProperty LabelOffsetProperty = DependencyProperty.Register(
            "LabelOffset",
            typeof(double),
            typeof(PieChart),
            new PropertyMetadata(10d, OnLabelOffsetChanged)
        );

        private static void OnLabelOffsetChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            PieChart chart = (PieChart)d;
            chart.Update();
        }

        public PieChart()
        {
            InitializeComponent();
        }

        protected override void OnItemsChanged(NotifyCollectionChangedEventArgs e)
        {
            base.OnItemsChanged(e);
            Update();
        }

        protected override DependencyObject GetContainerForItemOverride()
        {
            PieChartItem item = new PieChartItem();

            if (!String.IsNullOrEmpty(ValuePath))
                item.SetBinding(PieChartItem.ValueProperty, new Binding(ValuePath));

            if (!String.IsNullOrEmpty(ForegroundPath))
                item.SetBinding(PieChartItem.ForegroundProperty, new Binding(ForegroundPath));

            if (!String.IsNullOrEmpty(LabelPath))
                item.SetBinding(PieChartItem.LabelProperty, new Binding(LabelPath) { Converter = LabelConverter });

            return item;
        }

        protected override bool IsItemItsOwnContainerOverride(object item)
        {
            return item is PieChartItem;
        }

        protected override Size ArrangeOverride(Size arrangeBounds)
        {
            Size result = base.ArrangeOverride(arrangeBounds);
            Update();
            return result;
        }
        
        protected override void PrepareContainerForItemOverride(DependencyObject element, object item)
        {
            PieChartItem chartItem = element as PieChartItem;
            if (chartItem != null)
                chartItem.LabelTemplate = ItemTemplate;
        }

        internal void Update()
        {
            double sum = 0;
            foreach (PieChartItem item in EnumerateItems())
            {
                double percentage = GetPercentage(item);
                item.Update(sum, percentage, this);
                sum += percentage;
            }
        }

        private double GetPercentage(PieChartItem item)
        {
            double sum = EnumerateItems().Sum(i => i.Value);
            double value = item.Value;

            return (value / sum) * 100;
        }

        private IEnumerable<PieChartItem> EnumerateItems()
        {
            for (int i = 0; i < Items.Count; i++)
            {
                PieChartItem item = Items[i] as PieChartItem;
                if (item != null)
                    yield return item;
                else
                    yield return (PieChartItem)ItemContainerGenerator.ContainerFromIndex(i);
            }
        }

        private void OnSizeChanged(object sender, SizeChangedEventArgs e)
        {
            Update();
        }
    }
}
