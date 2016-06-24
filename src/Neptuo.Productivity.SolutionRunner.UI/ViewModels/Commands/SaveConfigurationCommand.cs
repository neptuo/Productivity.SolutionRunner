using Neptuo.Productivity.SolutionRunner.Properties;
using Neptuo.Productivity.SolutionRunner.Services;
using Neptuo.Productivity.SolutionRunner.Services.Searching;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Neptuo.Productivity.SolutionRunner.ViewModels.Commands
{
    public class SaveConfigurationCommand : CommandBase
    {
        private readonly ConfigurationViewModel viewModel;
        private readonly IRunHotKeyService runHotKey;

        public SaveConfigurationCommand(ConfigurationViewModel viewModel, IRunHotKeyService runHotKey)
        {
            Ensure.NotNull(viewModel, "viewModel");
            Ensure.NotNull(runHotKey, "runHotKey");
            this.viewModel = viewModel;
            this.runHotKey = runHotKey;
        }

        protected override bool CanExecute()
        {
            return !String.IsNullOrEmpty(viewModel.SourceDirectoryPath) && Directory.Exists(viewModel.SourceDirectoryPath) && viewModel.FileSearchCount > 0;
        }

        protected override void Execute()
        {
            Settings.Default.SourceDirectoryPath = viewModel.SourceDirectoryPath;
            Settings.Default.PreferedApplicationPath = viewModel.PreferedApplicationPath;
            Settings.Default.FileSearchMode = Converts.To<FileSearchMode, string>(viewModel.FileSearchMode);
            Settings.Default.FileSearchCount = viewModel.FileSearchCount;
            Settings.Default.IsFileSearchPatternSaved = viewModel.IsFileSearchPatternSaved;
            Settings.Default.IsLastUsedApplicationSavedAsPrefered = viewModel.IsLastUsedApplicationSavedAsPrefered;
            Settings.Default.IsDismissedWhenLostFocus = viewModel.IsDismissedWhenLostFocus;

            string runKeyValue;
            if (Converts.Try(viewModel.RunKey, out runKeyValue))
                Settings.Default.RunKey = runKeyValue;

            if (viewModel.RunKey == null)
                runHotKey.UnBind();
            else
                runHotKey.Bind(viewModel.RunKey.Key, viewModel.RunKey.Modifier);

            Settings.Default.Save();
            EventManager.RaiseConfigurationSaved(viewModel);
        }
    }
}
