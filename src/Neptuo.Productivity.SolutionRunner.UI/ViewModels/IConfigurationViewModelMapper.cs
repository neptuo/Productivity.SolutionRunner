using Neptuo.Productivity.SolutionRunner.Services.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Neptuo.Productivity.SolutionRunner.ViewModels
{
    public interface IConfigurationViewModelMapper
    {
        Task MapAsync(ISettings settings, ConfigurationViewModel viewModel);
        Task MapAsync(ConfigurationViewModel viewModel, ISettings settings);
    }
}
