using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Neptuo.Productivity.SolutionRunner.ViewModels.Commands
{
    public class RemoveAdditionalCommandCommand : CommandBase<AdditionalApplicationListViewModel>
    {
        private readonly AdditionalApplicationEditViewModel viewModel;

        public RemoveAdditionalCommandCommand(AdditionalApplicationEditViewModel viewModel)
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
            if (parameter != null)
                viewModel.Commands.Remove(parameter);
        }
    }
}
