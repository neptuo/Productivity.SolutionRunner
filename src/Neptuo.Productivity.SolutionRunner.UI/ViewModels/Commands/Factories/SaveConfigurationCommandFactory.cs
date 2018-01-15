using Neptuo.Activators;
using Neptuo.Productivity.SolutionRunner.Services;
using Neptuo.Productivity.SolutionRunner.Services.Configuration;
using Neptuo.Productivity.SolutionRunner.Services.StartupShortcuts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Neptuo.Productivity.SolutionRunner.ViewModels.Commands.Factories
{
    public class SaveConfigurationCommandFactory : IFactory<SaveConfigurationCommand, ConfigurationViewModel>
    {
        private readonly ISettingsService settingsService;
        private readonly ISettings settings;
        private readonly DefaultRunHotKeyService runHotKey;
        private readonly ShortcutService shortcutService;

        internal SaveConfigurationCommandFactory(ISettingsService settingsService, ISettings settings, DefaultRunHotKeyService runHotKey, ShortcutService shortcutService)
        {
            Ensure.NotNull(settingsService, "settingsService");
            Ensure.NotNull(settings, "settings");
            Ensure.NotNull(runHotKey, "runHotKey");
            Ensure.NotNull(shortcutService, "shortcutService");
            this.settingsService = settingsService;
            this.settings = settings;
            this.runHotKey = runHotKey;
            this.shortcutService = shortcutService;
        }

        public SaveConfigurationCommand Create(ConfigurationViewModel viewModel)
        {
            return new SaveConfigurationCommand(viewModel, settingsService, settings, runHotKey, shortcutService);
        }
    }
}
