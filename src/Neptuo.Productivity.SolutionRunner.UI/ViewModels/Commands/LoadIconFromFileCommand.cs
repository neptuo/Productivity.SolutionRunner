using Neptuo;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Neptuo.Productivity.SolutionRunner.ViewModels.Commands
{
    public class LoadIconFromFileCommand : CommandBase
    {
        private readonly Action<string> setter;

        public LoadIconFromFileCommand(Action<string> setter)
        {
            Ensure.NotNull(setter, "setter");
            this.setter = setter;
        }

        protected override bool CanExecute() => true;

        protected override void Execute()
        {
            OpenFileDialog dialog = new OpenFileDialog();
            dialog.AddExtension = true;
            dialog.CheckFileExists = true;
            dialog.InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.ProgramFilesX86);

            if (dialog.ShowDialog() == DialogResult.OK)
            {
                string path = dialog.FileName;
                Icon icon = Icon.ExtractAssociatedIcon(path);
                using (var imageStream = new MemoryStream())
                {
                    icon.Save(imageStream);
                    imageStream.Position = 0;

                    string data = Convert.ToBase64String(imageStream.ToArray());
                    setter(data);
                }
            }
        }
    }
}
