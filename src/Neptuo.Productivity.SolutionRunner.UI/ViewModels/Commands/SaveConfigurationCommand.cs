using Neptuo.Observables.Commands;
using Neptuo.Productivity.SolutionRunner.Services.Configuration;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Windows.ApplicationModel;
using Windows.UI.Popups;

namespace Neptuo.Productivity.SolutionRunner.ViewModels.Commands
{
    public class SaveConfigurationCommand : AsyncCommand
    {
        private readonly ConfigurationViewModel viewModel;
        private readonly IConfigurationViewModelMapper mapper;
        private readonly ISettingsService settingsService;
        private readonly ISettings settings;

        internal SaveConfigurationCommand(ConfigurationViewModel viewModel, IConfigurationViewModelMapper mapper, ISettingsService settingsService, ISettings settings)
        {
            Ensure.NotNull(viewModel, "viewModel");
            Ensure.NotNull(mapper, "mapper");
            Ensure.NotNull(settingsService, "settingsService");
            Ensure.NotNull(settings, "settings");
            this.viewModel = viewModel;
            this.mapper = mapper;
            this.settingsService = settingsService;
            this.settings = settings;
        }

        protected override bool CanExecuteOverride()
        {
            if (String.IsNullOrEmpty(viewModel.SourceDirectoryPath))
                return false;

            if (!Directory.Exists(viewModel.SourceDirectoryPath))
                return false;

            if (viewModel.FileSearchCount <= 0)
                return false;

            return true;
        }

        protected override async Task ExecuteAsync(CancellationToken cancellationToken)
        {
            Properties.Configuration.Default.Path = viewModel.ConfigurationPath;
            Properties.Configuration.Default.Save();

            mapper.MapAsync(viewModel, settings);

            await settingsService.SaveAsync(settings);
            EventManager.RaiseConfigurationSaved(viewModel);
        }

        public new void RaiseCanExecuteChanged()
            => base.RaiseCanExecuteChanged();
    }
}
