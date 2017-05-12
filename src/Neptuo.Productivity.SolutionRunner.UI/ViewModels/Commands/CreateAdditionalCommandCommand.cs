﻿using Neptuo.Productivity.SolutionRunner.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Neptuo.Productivity.SolutionRunner.ViewModels.Commands
{
    public class CreateAdditionalCommandCommand : CommandBase
    {
        private readonly AdditionalApplicationEditViewModel viewModel;
        private readonly INavigator navigator;

        public CreateAdditionalCommandCommand(AdditionalApplicationEditViewModel viewModel, INavigator navigator)
        {
            Ensure.NotNull(viewModel, "viewModel");
            Ensure.NotNull(navigator, "navigator");
            this.viewModel = viewModel;
            this.navigator = navigator;
        }

        protected override bool CanExecute()
        {
            return true;
        }

        protected override void Execute()
        {
            navigator.OpenAdditionalCommandEdit(null, m => viewModel.Commands.Add(new AdditionalApplicationListViewModel(m)));
        }
    }
}
