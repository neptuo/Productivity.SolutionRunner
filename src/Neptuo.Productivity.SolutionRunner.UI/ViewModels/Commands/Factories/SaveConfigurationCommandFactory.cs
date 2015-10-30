using Neptuo.Activators;
using Neptuo.Productivity.SolutionRunner.Services;
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

        public SaveConfigurationCommandFactory(DefaultRunHotKeyService runHotKey)
        {
            Ensure.NotNull(runHotKey, "runHotKey");
            this.runHotKey = runHotKey;
        }

        public SaveConfigurationCommand Create(ConfigurationViewModel viewModel)
        {
            return new SaveConfigurationCommand(viewModel, runHotKey);
        }
    }
}
