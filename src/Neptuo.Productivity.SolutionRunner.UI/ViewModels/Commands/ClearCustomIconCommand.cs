using Neptuo;
using Neptuo.Observables.Commands;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Neptuo.Productivity.SolutionRunner.ViewModels.Commands
{
    public class ClearCustomIconCommand : Command
    {
        private readonly AdditionalApplicationEditViewModel viewModel;

        public ClearCustomIconCommand(AdditionalApplicationEditViewModel viewModel)
        {
            Ensure.NotNull(viewModel, "viewModel");
            this.viewModel = viewModel;

            viewModel.PropertyChanged += OnPropertyChanged;
        }

        private void OnPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(AdditionalApplicationEditViewModel.IconData))
                RaiseCanExecuteChanged();
        }

        public override bool CanExecute() 
            => viewModel.IconData != null;

        public override void Execute() 
            => viewModel.IconData = null;
    }
}
