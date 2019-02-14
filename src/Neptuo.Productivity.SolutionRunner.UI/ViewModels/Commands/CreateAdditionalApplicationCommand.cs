using Neptuo.Observables.Commands;
using Neptuo.Productivity.SolutionRunner.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Neptuo.Productivity.SolutionRunner.ViewModels.Commands
{
    public class CreateAdditionalApplicationCommand : Command
    {
        public interface IContainer
        {
            void Add(AdditionalApplicationListViewModel viewModel);
        }

        private readonly IContainer container;
        private readonly INavigator navigator;

        public CreateAdditionalApplicationCommand(IContainer container, INavigator navigator)
        {
            Ensure.NotNull(container, "container");
            Ensure.NotNull(navigator, "navigator");
            this.container = container;
            this.navigator = navigator;
        }

        public override bool CanExecute() 
            => true;

        public override void Execute() 
            => navigator.OpenAdditionalApplicationEdit(null, m => container.Add(new AdditionalApplicationListViewModel(m)));
    }
}
