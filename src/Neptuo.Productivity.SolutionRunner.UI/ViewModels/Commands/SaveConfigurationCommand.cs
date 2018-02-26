using Neptuo.Productivity.SolutionRunner.Services;
using Neptuo.Productivity.SolutionRunner.Services.Applications;
using Neptuo.Productivity.SolutionRunner.Services.Configuration;
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
        private readonly ISettingsService settingsService;
        private readonly ISettings settings;
        private readonly IRunHotKeyService runHotKey;
        private readonly ShortcutService shortcutService;

        internal SaveConfigurationCommand(ConfigurationViewModel viewModel, ISettingsService settingsService, ISettings settings, IRunHotKeyService runHotKey, ShortcutService shortcutService)
        {
            Ensure.NotNull(viewModel, "viewModel");
            Ensure.NotNull(settingsService, "settingsService");
            Ensure.NotNull(settings, "settings");
            Ensure.NotNull(runHotKey, "runHotKey");
            Ensure.NotNull(shortcutService, "shortcutService");
            this.viewModel = viewModel;
            this.settingsService = settingsService;
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
            Properties.Configuration.Default.Path = viewModel.ConfigurationPath;
            Properties.Configuration.Default.Save();

            settings.SourceDirectoryPath = viewModel.SourceDirectoryPath;
            settings.PreferedApplicationPath = viewModel.PreferedApplication?.Path;
            settings.FileSearchMode = viewModel.FileSearchMode;
            settings.FileSearchCount = viewModel.FileSearchCount;
            settings.IsFileSearchPatternSaved = viewModel.IsFileSearchPatternSaved;
            settings.IsLastUsedApplicationSavedAsPrefered = viewModel.IsLastUsedApplicationSavedAsPrefered;
            settings.IsDismissedWhenLostFocus = viewModel.IsDismissedWhenLostFocus;
            settings.IsHiddentOnStartup = viewModel.IsHiddentOnStartup;
            settings.IsAutoSelectApplicationVersion = viewModel.IsAutoSelectApplicationVersion;
            settings.AutoSelectApplicationMinimalVersion = viewModel.AutoSelectApplicationMinimalVersion.Model;
            settings.IsFileNameRemovedFromDisplayedPath = viewModel.IsFileNameRemovedFromDisplayedPath;
            settings.IsDisplayedPathTrimmedToLastFolderName = viewModel.IsDisplayedPathTrimmedToLastFolderName;
            settings.IsTrayIcon = viewModel.IsTrayIcon;
            settings.IsStatisticsCounted = viewModel.IsStatisticsCounted;
            settings.IsProjectCountEnabled = viewModel.IsProjectCountEnabled;
            settings.AdditionalApplications = new AdditionalApplicationCollection(viewModel.AdditionalApplications.Select(a => a.Model));

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

            settings.PositionMode = viewModel.PositionMode;
            settings.PositionLeft = viewModel.PositionLeft ?? 0;
            settings.PositionTop = viewModel.PositionTop ?? 0;

            settings.HiddenMainApplications = viewModel.MainApplications.Where(a => !a.IsEnabled).Select(a => a.Path).ToArray();

            settings.ThemeMode = viewModel.ThemeMode;

            settingsService.SaveAsync(settings);
            EventManager.RaiseConfigurationSaved(viewModel);
        }
    }
}
