using Neptuo;
using Neptuo.Activators;
using Neptuo.Productivity.SolutionRunner.Services;
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
        private readonly DefaultRunHotKeyService runHotKey;
        private readonly ShortcutService shortcutService;

        public SaveConfigurationCommandFactory(DefaultRunHotKeyService runHotKey, ShortcutService shortcutService)
        {
            Ensure.NotNull(runHotKey, "runHotKey");
            Ensure.NotNull(shortcutService, "shortcutService");
            this.runHotKey = runHotKey;
            this.shortcutService = shortcutService;
        }

        public SaveConfigurationCommand Create(ConfigurationViewModel viewModel)
        {
            return new SaveConfigurationCommand(viewModel, runHotKey, shortcutService);
        }
    }
}
