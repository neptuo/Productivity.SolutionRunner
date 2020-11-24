using Neptuo.Observables.Commands;
using Neptuo.Productivity.SolutionRunner.Services.Statistics;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Neptuo.Productivity.SolutionRunner.ViewModels.Commands
{
    public class ExportStatisticsCommand : AsyncCommand
    {
        private readonly ICountingImporter importer;

        public ExportStatisticsCommand(ICountingImporter importer)
        {
            Ensure.NotNull(importer, "importer");
            this.importer = importer;
        }

        protected override bool CanExecuteOverride()
            => true;

        protected async override Task ExecuteAsync(CancellationToken cancellationToken)
        {
            var dialog = new SaveFileDialog
            {
                AddExtension = true
            };

            dialog.InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile);
            dialog.FileName = "Statistics";
            dialog.DefaultExt = ".dat";
            dialog.Filter = "Solution Runner Statistics file (*.dat)|*.dat";

            if (dialog.ShowDialog() == DialogResult.OK)
            {
                string filePath = dialog.FileName;
                using (FileStream data = new FileStream(filePath, FileMode.OpenOrCreate, FileAccess.Write))
                    await importer.ExportAsync(data);
            }
        }
    }
}
