using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Neptuo.Productivity.SolutionRunner.Services.Configuration
{
    public class ManualSettingsMapper : ISettingsMapper
    {
        public void Map(ISettings source, ISettings target)
        {
            target.FileSearchPattern = source.FileSearchPattern;
            target.IsAutoSelectApplicationVersion = source.IsAutoSelectApplicationVersion;
            target.IsDismissedWhenLostFocus = source.IsDismissedWhenLostFocus;
            target.IsDisplayedPathTrimmedToLastFolderName = source.IsDisplayedPathTrimmedToLastFolderName;
            target.IsFileNameRemovedFromDisplayedPath = source.IsFileNameRemovedFromDisplayedPath;
            target.IsFileSearchPatternSaved = source.IsFileSearchPatternSaved;
            target.IsHiddentOnStartup = source.IsHiddentOnStartup;
            target.IsLastUsedApplicationSavedAsPrefered = source.IsLastUsedApplicationSavedAsPrefered;
            target.IsProjectCountEnabled = source.IsProjectCountEnabled;
            target.IsStatisticsCounted = source.IsStatisticsCounted;
            target.IsTrayIcon = source.IsTrayIcon;
            target.PinnedFiles = source.PinnedFiles;
            target.PositionLeft = source.PositionLeft;
            target.PositionMode = source.PositionMode;
            target.PositionTop = source.PositionTop;
            target.PreferedApplicationPath = source.PreferedApplicationPath;
            target.RunKey = source.RunKey;
            target.SourceDirectoryPath = source.SourceDirectoryPath;
            target.ThemeMode = source.ThemeMode;
            target.AdditionalApplications = source.AdditionalApplications;
            target.HiddenMainApplications = source.HiddenMainApplications;
            target.FileSearchCount = source.FileSearchCount;
            target.FileSearchMode = source.FileSearchMode;
            target.AutoSelectApplicationMinimalVersion = source.AutoSelectApplicationMinimalVersion;
        }
    }
}
