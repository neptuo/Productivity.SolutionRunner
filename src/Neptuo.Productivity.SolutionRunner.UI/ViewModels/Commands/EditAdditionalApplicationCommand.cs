using Neptuo.Observables.Commands;
using Neptuo.Productivity.SolutionRunner.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Neptuo.Productivity.SolutionRunner.ViewModels.Commands
{
    public class EditAdditionalApplicationCommand : Command<AdditionalApplicationListViewModel>
    {
        private readonly ConfigurationViewModel viewModel;
        private readonly INavigator navigator;

        public EditAdditionalApplicationCommand(ConfigurationViewModel viewModel, INavigator navigator)
        {
            Ensure.NotNull(viewModel, "viewModel");
            Ensure.NotNull(navigator, "navigator");
            this.viewModel = viewModel;
            this.navigator = navigator;
        }

        public override bool CanExecute(AdditionalApplicationListViewModel parameter) 
            => true;

        public override void Execute(AdditionalApplicationListViewModel parameter)
        {
            if (parameter != null)
            {
                navigator.OpenAdditionalApplicationEdit(parameter.Model, m =>
                {
                    if (m != null)
                        parameter.UpdateModel(m);
                });
            }
        }
    }
}
