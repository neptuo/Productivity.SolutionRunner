﻿using Neptuo.Observables.Commands;
using Neptuo.Productivity.SolutionRunner.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Neptuo.Productivity.SolutionRunner.ViewModels.Commands
{
    public class EditAdditionalCommandCommand : Command<AdditionalApplicationListViewModel>
    {
        private readonly AdditionalApplicationEditViewModel viewModel;
        private readonly INavigator navigator;

        public EditAdditionalCommandCommand(AdditionalApplicationEditViewModel viewModel, INavigator navigator)
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
                navigator.OpenAdditionalCommandEdit(parameter.Model, m =>
                {
                    if (m != null)
                        parameter.UpdateModel(m);
                });
            }
        }
    }
}
