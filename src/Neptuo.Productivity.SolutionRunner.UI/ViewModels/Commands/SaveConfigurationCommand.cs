using Neptuo.Activators;
using Neptuo.Formatters;
using Neptuo.Productivity.SolutionRunner.Properties;
using Neptuo.Productivity.SolutionRunner.Services;
using Neptuo.Productivity.SolutionRunner.Services.Applications;
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
            if (String.IsNullOrEmpty(viewModel.SourceDirectoryPath))
                return false;

            if (!Directory.Exists(viewModel.SourceDirectoryPath))
                return false;

            if (viewModel.FileSearchCount <= 0)
                return false;

            return true;
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
            Settings.Default.IsHiddentOnStartup = viewModel.IsHiddentOnStartup;
            Settings.Default.IsAutoSelectApplicationVersion = viewModel.IsAutoSelectApplicationVersion;
            Settings.Default.AdditionalApplications = Converts
                .To<AdditionalApplicationCollection, string>(new AdditionalApplicationCollection(viewModel.AdditionalApplications.Select(a => a.Model)));

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
