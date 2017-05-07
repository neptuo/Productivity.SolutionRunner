using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Neptuo.Productivity.SolutionRunner.ViewModels.Commands
{
    public class RemoveAdditionalApplicationCommand : CommandBase<AdditionalApplicationListViewModel>
    {
        public interface IContainer
        {
            void Remove(AdditionalApplicationListViewModel viewModel);
        }

        private readonly IContainer container;

        public RemoveAdditionalApplicationCommand(IContainer container)
        {
            Ensure.NotNull(container, "container");
            this.container = container;
        }

        protected override bool CanExecute(AdditionalApplicationListViewModel parameter)
        {
            return true;
        }

        protected override void Execute(AdditionalApplicationListViewModel parameter)
        {
            if (parameter != null)
                container.Remove(parameter);
        }
    }
}
