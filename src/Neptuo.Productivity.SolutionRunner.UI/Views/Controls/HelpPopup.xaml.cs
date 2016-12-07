using System;
using System.Collections.Generic;
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
    /// Interaction logic for HelpPopup.xaml
    /// </summary>
    [ContentProperty("Body")]
    public partial class HelpPopup : Popup
    {
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
            InitializeComponent();
        }
    }
}
