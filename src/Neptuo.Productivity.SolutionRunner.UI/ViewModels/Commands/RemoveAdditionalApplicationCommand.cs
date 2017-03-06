using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Neptuo.Productivity.SolutionRunner.ViewModels.Commands
{
    public class RemoveAdditionalApplicationCommand : CommandBase<AdditionalApplicationListViewModel>
    {
        private readonly ConfigurationViewModel viewModel;

        public RemoveAdditionalApplicationCommand(ConfigurationViewModel viewModel)
        {
            Ensure.NotNull(viewModel, "viewModel");
            this.viewModel = viewModel;
        }

        protected override bool CanExecute(AdditionalApplicationListViewModel parameter)
        {
            return true;
        }

        protected override void Execute(AdditionalApplicationListViewModel parameter)
        {
            if (viewModel.AdditionalApplications != null && parameter != null)
            {
                viewModel.AdditionalApplications.Remove(parameter);
                if (parameter.Path.Equals(viewModel.PreferedApplication?.Path, StringComparison.InvariantCultureIgnoreCase))
                    viewModel.PreferedApplication = null;
            }
        }
    }
}
