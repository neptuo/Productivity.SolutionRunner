using Neptuo.Productivity.SolutionRunner.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace Neptuo.Productivity.SolutionRunner.ViewModels.Commands
{
    public class PinCommand : CommandBase
    {
        private readonly FileViewModel viewModel;

        public PinCommand(FileViewModel viewModel)
        {
            Ensure.NotNull(viewModel, "viewModel");
            this.viewModel = viewModel;
        }

        protected override bool CanExecute()
        {
            return true;
        }

        protected override void Execute()
        {
            viewModel.IsPinned = true;
        }
    }
}
