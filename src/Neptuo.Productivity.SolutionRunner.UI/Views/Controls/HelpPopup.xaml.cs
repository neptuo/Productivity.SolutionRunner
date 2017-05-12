using System;
using System.Collections.Generic;
using System.ComponentModel;
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
    [ContentProperty("Body")]
    public partial class HelpPopup : Popup
    {
        private Window window;

        public object Body
        {
            get { return (object)GetValue(BodyProperty); }
            set { SetValue(BodyProperty, value); }
        }

        public static readonly DependencyProperty BodyProperty = DependencyProperty.Register(
            "Body",
            typeof(object),
            typeof(HelpPopup),
            new PropertyMetadata(null)
        );

        public HelpPopup()
        {
            DependencyPropertyDescriptor placementTarget = DependencyPropertyDescriptor.FromProperty(PlacementTargetProperty, typeof(Popup));
            placementTarget.AddValueChanged(this, OnPlacementTargetChanged);

            InitializeComponent();
        }

        private void OnPlacementTargetChanged(object sender, EventArgs e)
        {
            if (window != null)
                window.LocationChanged -= OnWindowLocationChanged;

            window = Window.GetWindow(PlacementTarget);
            if (window != null)
                window.LocationChanged += OnWindowLocationChanged;
        }
        
        private void OnWindowLocationChanged(object sender, EventArgs e)
        {
            double offset = HorizontalOffset;
            HorizontalOffset = offset + 1;
            HorizontalOffset = offset;
        }
    }
}
