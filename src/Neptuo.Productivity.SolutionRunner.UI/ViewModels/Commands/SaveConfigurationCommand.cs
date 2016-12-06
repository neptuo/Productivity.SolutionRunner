using Neptuo;
using Neptuo.Activators;
using Neptuo.Formatters;
using Neptuo.Productivity.SolutionRunner.Properties;
using Neptuo.Productivity.SolutionRunner.Services;
using Neptuo.Productivity.SolutionRunner.Services.Applications;
using Neptuo.Productivity.SolutionRunner.Services.Searching;
using Neptuo.Productivity.SolutionRunner.Services.StartupShortcuts;
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
        private readonly Settings settings;
        private readonly IRunHotKeyService runHotKey;
        private readonly ShortcutService shortcutService;

        internal SaveConfigurationCommand(ConfigurationViewModel viewModel, Settings settings, IRunHotKeyService runHotKey, ShortcutService shortcutService)
        {
            Ensure.NotNull(viewModel, "viewModel");
            Ensure.NotNull(settings, "settings");
            Ensure.NotNull(runHotKey, "runHotKey");
            Ensure.NotNull(shortcutService, "shortcutService");
            this.viewModel = viewModel;
            this.settings = settings;
            this.runHotKey = runHotKey;
            this.shortcutService = shortcutService;
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
            settings.SourceDirectoryPath = viewModel.SourceDirectoryPath;
            settings.PreferedApplicationPath = viewModel.PreferedApplicationPath;
            settings.FileSearchMode = Converts.To<FileSearchMode, string>(viewModel.FileSearchMode);
            settings.FileSearchCount = viewModel.FileSearchCount;
            settings.IsFileSearchPatternSaved = viewModel.IsFileSearchPatternSaved;
            settings.IsLastUsedApplicationSavedAsPrefered = viewModel.IsLastUsedApplicationSavedAsPrefered;
            settings.IsDismissedWhenLostFocus = viewModel.IsDismissedWhenLostFocus;
            settings.IsHiddentOnStartup = viewModel.IsHiddentOnStartup;
            settings.IsAutoSelectApplicationVersion = viewModel.IsAutoSelectApplicationVersion;
            settings.IsFileNameRemovedFromDisplayedPath = viewModel.IsFileNameRemovedFromDisplayedPath;
            settings.IsDisplayedPathTrimmedToLastFolderName = viewModel.IsDisplayedPathTrimmedToLastFolderName;
            settings.AdditionalApplications = Converts
                .To<AdditionalApplicationCollection, string>(new AdditionalApplicationCollection(viewModel.AdditionalApplications.Select(a => a.Model)));

            if (viewModel.IsAutoStartup)
                shortcutService.Create(Environment.SpecialFolder.Startup);
            else
                shortcutService.Delete(Environment.SpecialFolder.Startup);

            string runKeyValue;
            if (Converts.Try(viewModel.RunKey, out runKeyValue))
                settings.RunKey = runKeyValue;

            if (viewModel.RunKey == null)
                runHotKey.UnBind();
            else
                runHotKey.Bind(viewModel.RunKey.Key, viewModel.RunKey.Modifier);

            settings.Save();
            EventManager.RaiseConfigurationSaved(viewModel);
        }
    }
}
