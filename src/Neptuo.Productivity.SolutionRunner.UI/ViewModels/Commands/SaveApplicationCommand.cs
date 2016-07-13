using Neptuo.Productivity.SolutionRunner.Services.Applications;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Neptuo.Productivity.SolutionRunner.ViewModels.Commands
{
    public class SaveApplicationCommand : CommandBase
    {
        private readonly AdditionalApplicationEditViewModel viewModel;
        private readonly AdditionalApplicationModel sourceModel;
        private readonly Action<AdditionalApplicationModel> onSaved;

        public SaveApplicationCommand(AdditionalApplicationEditViewModel viewModel, AdditionalApplicationModel sourceModel, Action<AdditionalApplicationModel> onSaved)
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
            AdditionalApplicationModel targetModel = null;
            if (sourceModel == null)
                targetModel = new AdditionalApplicationModel(viewModel.Name, viewModel.Path, viewModel.Arguments);
            else if(viewModel.Path != sourceModel.Path || viewModel.Name != sourceModel.Name || viewModel.Arguments != sourceModel.Arguments)
                targetModel = new AdditionalApplicationModel(viewModel.Name, viewModel.Path, viewModel.Arguments);

            if (targetModel != null)
                onSaved(targetModel);
            else
                onSaved(null);
        }
    }
}
