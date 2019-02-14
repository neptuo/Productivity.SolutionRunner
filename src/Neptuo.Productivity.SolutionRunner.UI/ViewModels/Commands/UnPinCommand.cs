using Neptuo.Observables.Commands;
using Neptuo.Productivity.SolutionRunner.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Neptuo.Productivity.SolutionRunner.ViewModels.Commands
{
    public class UnPinCommand : Command
    {
        private readonly FileViewModel viewModel;

        public UnPinCommand(FileViewModel viewModel)
        {
            Ensure.NotNull(viewModel, "viewModel");
            this.viewModel = viewModel;
        }

        public override bool CanExecute() 
            => true;

        public override void Execute() 
            => viewModel.IsPinned = false;
    }
}
