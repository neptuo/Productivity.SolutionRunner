using Neptuo.Activators;
using Neptuo.Productivity.SolutionRunner.Services.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Neptuo.Productivity.SolutionRunner.ViewModels.Commands.Factories
{
    public class SaveConfigurationCommandFactory : IFactory<SaveConfigurationCommand, ConfigurationViewModel>
    {
        private readonly IConfigurationViewModelMapper mapper;
        private readonly ISettingsService settingsService;
        private readonly ISettings settings;

        internal SaveConfigurationCommandFactory(IConfigurationViewModelMapper mapper, ISettingsService settingsService, ISettings settings)
        {
            Ensure.NotNull(mapper, "mapper");
            Ensure.NotNull(settingsService, "settingsService");
            Ensure.NotNull(settings, "settings");
            this.mapper = mapper;
            this.settingsService = settingsService;
            this.settings = settings;
        }

        public SaveConfigurationCommand Create(ConfigurationViewModel viewModel)
        {
            return new SaveConfigurationCommand(viewModel, mapper, settingsService, settings);
        }
    }
}
