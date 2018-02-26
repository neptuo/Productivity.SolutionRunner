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
    /// <summary>
    /// Interaction logic for DirectoryBrowser.xaml
    /// </summary>
    public partial class FileBrowser : UserControl
    {
        public string Path
        {
            get { return (string)GetValue(PathProperty); }
            set { SetValue(PathProperty, value); }
        }

        public static readonly DependencyProperty PathProperty = DependencyProperty.Register(
            "Path", 
            typeof(string), 
            typeof(FileBrowser)
        );


        public string Filter
        {
            get { return (string)GetValue(FilterProperty); }
            set { SetValue(FilterProperty, value); }
        }

        public static readonly DependencyProperty FilterProperty = DependencyProperty.Register(
            "Filter", 
            typeof(string), 
            typeof(FileBrowser), 
            new PropertyMetadata("Executables (*.exe)|*.exe|Command List (*.cmd)|*.cmd|Batch (*.bat)|*.bat")
        );


        public bool IsExistingOnly
        {
            get { return (bool)GetValue(IsExistingOnlyProperty); }
            set { SetValue(IsExistingOnlyProperty, value); }
        }

        public static readonly DependencyProperty IsExistingOnlyProperty = DependencyProperty.Register(
            "IsExistingOnly", 
            typeof(bool), 
            typeof(FileBrowser), 
            new PropertyMetadata(true)
        );


        public FileBrowser()
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
            OpenFileDialog dialog = new OpenFileDialog();
            dialog.AddExtension = true;
            dialog.CheckFileExists = IsExistingOnly;

            string path = Path;
            if (String.IsNullOrEmpty(path))
            {
                dialog.InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.ProgramFilesX86);
            }
            else
            {
                dialog.FileName = System.IO.Path.GetFileNameWithoutExtension(Path);
                dialog.DefaultExt = System.IO.Path.GetExtension(Path);
                dialog.InitialDirectory = System.IO.Path.GetDirectoryName(Path);
                dialog.Filter = Filter;
            }

            if (dialog.ShowDialog() == DialogResult.OK)
                Path = dialog.FileName;

            tbxPath.Focus();
        }
    }
}
