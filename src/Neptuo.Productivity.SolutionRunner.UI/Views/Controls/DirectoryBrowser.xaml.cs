using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Forms;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using UserControl = System.Windows.Controls.UserControl;

namespace Neptuo.Productivity.SolutionRunner.Views.Controls
{
    public partial class DirectoryBrowser : UserControl
    {
        public string Path
        {
            get { return (string)GetValue(PathProperty); }
            set { SetValue(PathProperty, value); }
        }

        public static readonly DependencyProperty PathProperty = DependencyProperty.Register(
            "Path", 
            typeof(string), 
            typeof(DirectoryBrowser)
        );

        public DirectoryBrowser()
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
            tbxPath.Focus();
        }

        private void btnBrowse_Click(object sender, RoutedEventArgs e)
        {
            FolderBrowserDialog dialog = new FolderBrowserDialog();
            dialog.SelectedPath = Path;

            if (String.IsNullOrEmpty(dialog.SelectedPath))
                dialog.SelectedPath = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile);

            if (dialog.ShowDialog() == DialogResult.OK)
                Path = dialog.SelectedPath;

            tbxPath.Focus();
        }
    }
}
