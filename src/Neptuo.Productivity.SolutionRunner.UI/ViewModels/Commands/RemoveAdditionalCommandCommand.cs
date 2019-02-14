using Neptuo.Observables.Commands;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Neptuo.Productivity.SolutionRunner.ViewModels.Commands
{
    public class RemoveAdditionalCommandCommand : Command<AdditionalApplicationListViewModel>
    {
        private readonly AdditionalApplicationEditViewModel viewModel;

        public RemoveAdditionalCommandCommand(AdditionalApplicationEditViewModel viewModel)
        {
            Ensure.NotNull(viewModel, "viewModel");
            this.viewModel = viewModel;
        }

        public override bool CanExecute(AdditionalApplicationListViewModel parameter) 
            => true;

        public override void Execute(AdditionalApplicationListViewModel parameter)
        {
            if (parameter != null)
                viewModel.Commands.Remove(parameter);
        }
    }
}
