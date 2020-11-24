using Neptuo;
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
    public class ImportStatisticsCommand : AsyncCommand
    {
        private readonly StatisticsWithImportViewModel viewModel;
        private readonly ICountingImporter importer;

        public ImportStatisticsCommand(StatisticsWithImportViewModel viewModel, ICountingImporter importer)
        {
            Ensure.NotNull(viewModel, "viewModel");
            Ensure.NotNull(importer, "importer");
            this.viewModel = viewModel;
            this.importer = importer;
        }

        protected override bool CanExecuteOverride()
            => true;

        protected async override Task ExecuteAsync(CancellationToken cancellationToken)
        {
            var dialog = new OpenFileDialog
            {
                AddExtension = true,
                CheckFileExists = true,
                CheckPathExists = true
            };

            dialog.InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile);
            dialog.FileName = "Statistics";
            dialog.DefaultExt = ".dat";
            dialog.Filter = "Solution Runner Statistics file (*.dat)|*.dat";

            if (dialog.ShowDialog() == DialogResult.OK)
            {
                string filePath = dialog.FileName;
                if (File.Exists(filePath))
                {
                    using (FileStream data = new FileStream(filePath, FileMode.Open, FileAccess.Read))
                        await importer.ImportAsync(data);
                }
            }

            viewModel.Reload.Execute(null);
        }
    }
}
