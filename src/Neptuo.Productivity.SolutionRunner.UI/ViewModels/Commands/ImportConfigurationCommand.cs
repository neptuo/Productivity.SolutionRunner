using Neptuo.Observables.Commands;
using Neptuo.Productivity.SolutionRunner.Services.Configuration;
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
    public class ImportConfigurationCommand : AsyncCommand
    {
        private readonly ConfigurationViewModel viewModel;
        private readonly ISettingsFactory settingsFactory;
        private readonly IConfigurationViewModelMapper mapper;

        public ImportConfigurationCommand(ConfigurationViewModel viewModel, ISettingsFactory settingsFactory, IConfigurationViewModelMapper mapper)
        {
            Ensure.NotNull(viewModel, "viewModel");
            Ensure.NotNull(settingsFactory, "settingFactory");
            Ensure.NotNull(mapper, "mapper");
            this.viewModel = viewModel;
            this.settingsFactory = settingsFactory;
            this.mapper = mapper;
        }

        protected override bool CanExecuteOverride()
            => true;

        protected async override Task ExecuteAsync(CancellationToken cancellationToken)
        {
            var dialog = new OpenFileDialog
            {
                AddExtension = true,
                CheckFileExists = true
            };

            string path = viewModel.ConfigurationPath;
            if (String.IsNullOrEmpty(path))
            {
                dialog.InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.ProgramFilesX86);
            }
            else
            {
                dialog.FileName = Path.GetFileNameWithoutExtension(path);
                dialog.DefaultExt = Path.GetExtension(path);
                dialog.InitialDirectory = Path.GetDirectoryName(path);
                dialog.Filter = "Configuration File (*.json)|*.json";
            }

            if (dialog.ShowDialog() == DialogResult.OK)
            {
                path = dialog.FileName;

                ISettingsService target = settingsFactory.CreateForFile(path);
                ISettings targetSettings = await target.LoadAsync();

                await mapper.MapAsync(targetSettings, viewModel);
            }
        }
    }
}
