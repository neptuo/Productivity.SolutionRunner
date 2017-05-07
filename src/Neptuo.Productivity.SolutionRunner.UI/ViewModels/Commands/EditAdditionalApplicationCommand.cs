using Neptuo.Productivity.SolutionRunner.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Neptuo.Productivity.SolutionRunner.ViewModels.Commands
{
    public class EditAdditionalApplicationCommand : CommandBase<AdditionalApplicationListViewModel>
    {
        public interface IContainer
        {
            void Remove(AdditionalApplicationListViewModel viewModel);
        }

        private readonly IContainer container;
        private readonly INavigator navigator;

        public EditAdditionalApplicationCommand(IContainer container, INavigator navigator)
        {
            Ensure.NotNull(container, "container");
            Ensure.NotNull(navigator, "navigator");
            this.container = container;
            this.navigator = navigator;
        }

        protected override bool CanExecute(AdditionalApplicationListViewModel parameter)
        {
            return true;
        }

        protected override void Execute(AdditionalApplicationListViewModel parameter)
        {
            if (parameter != null)
            {
                navigator.OpenAdditionalApplicationEdit(parameter.Model, m =>
                {
                    if (m == null)
                        container.Remove(parameter);
                    else
                        parameter.UpdateModel(m);
                });
            }
        }
    }
}
