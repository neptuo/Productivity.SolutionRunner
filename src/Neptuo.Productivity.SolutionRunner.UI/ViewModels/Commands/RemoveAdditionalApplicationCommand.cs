using Neptuo.Observables.Commands;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Neptuo.Productivity.SolutionRunner.ViewModels.Commands
{
    public class RemoveAdditionalApplicationCommand : Command<AdditionalApplicationListViewModel>
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

        public override bool CanExecute(AdditionalApplicationListViewModel parameter) 
            => true;

        public override void Execute(AdditionalApplicationListViewModel parameter)
        {
            if (parameter != null)
                container.Remove(parameter);
        }
    }
}
