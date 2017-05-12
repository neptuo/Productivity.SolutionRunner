using Neptuo.Productivity.SolutionRunner.Services.Applications;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace Neptuo.Productivity.SolutionRunner.ViewModels.Commands
{
    public class SaveApplicationCommand : CommandBase
    {
        private readonly IApplicationViewModel viewModel;
        private readonly AdditionalApplicationModel sourceModel;
        private readonly Action<AdditionalApplicationModel> onSaved;

        public SaveApplicationCommand(IApplicationViewModel viewModel, AdditionalApplicationModel sourceModel, Action<AdditionalApplicationModel> onSaved)
        {
            Ensure.NotNull(viewModel, "viewModel");
            Ensure.NotNull(onSaved, "onSaved");
            this.viewModel = viewModel;
            this.sourceModel = sourceModel;
            this.onSaved = onSaved;
        }

        protected override bool CanExecute()
        {
            return !String.IsNullOrEmpty(viewModel.Path) && File.Exists(viewModel.Path);
        }

        protected override void Execute()
        {
            AdditionalApplicationModel targetModel = viewModel.ToModel();
            if (sourceModel == null || !sourceModel.Equals(targetModel))
                onSaved(targetModel);
            else
                onSaved(null);
        }
    }
}
