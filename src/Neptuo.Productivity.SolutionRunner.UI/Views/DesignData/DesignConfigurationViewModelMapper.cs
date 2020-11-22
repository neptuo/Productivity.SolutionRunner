using Neptuo.Productivity.SolutionRunner.Services.Configuration;
using Neptuo.Productivity.SolutionRunner.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Neptuo.Productivity.SolutionRunner.Views.DesignData
{
    public class DesignConfigurationViewModelMapper : IConfigurationViewModelMapper
    {
        public Task MapAsync(ISettings settings, ConfigurationViewModel viewModel)
            => Task.FromResult(true);

        public Task MapAsync(ConfigurationViewModel viewModel, ISettings settings)
            => Task.FromResult(true);
    }
}
