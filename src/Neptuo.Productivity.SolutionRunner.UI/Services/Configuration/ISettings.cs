using Neptuo.Logging;
using Neptuo.Productivity.SolutionRunner.Services.Applications;
using Neptuo.Productivity.SolutionRunner.Services.Positions;
using Neptuo.Productivity.SolutionRunner.Services.Searching;
using Neptuo.Productivity.SolutionRunner.Services.Themes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Neptuo.Productivity.SolutionRunner.Services.Configuration
{
    public interface ISettings
    {
        string FileSearchPattern { get; set; }
        bool IsAutoSelectApplicationVersion { get; set; }
        bool IsDismissedWhenLostFocus { get; set; }
        bool IsDisplayedPathTrimmedToLastFolderName { get; set; }
        bool IsFileNameRemovedFromDisplayedPath { get; set; }
        bool IsFileSearchPatternSaved { get; set; }
        bool IsHiddentOnStartup { get; set; }
        bool IsLastUsedApplicationSavedAsPrefered { get; set; }
        bool IsProjectCountEnabled { get; set; }
        bool IsStatisticsCounted { get; set; }
        bool IsTrayIcon { get; set; }
        IReadOnlyList<string> PinnedFiles { get; set; }
        double PositionLeft { get; set; }
        PositionMode PositionMode { get; set; }
        double PositionTop { get; set; }
        string PreferedApplicationPath { get; set; }
        string RunKey { get; set; }
        string SourceDirectoryPath { get; set; }
        Theme ThemeMode { get; set; }

        AdditionalApplicationCollection AdditionalApplications { get; set; }
        IReadOnlyList<string> HiddenMainApplications { get; set; }
        int FileSearchCount { get; set; }
        FileSearchMode FileSearchMode { get; set; }
        Version AutoSelectApplicationMinimalVersion { get; set; }

        LogLevel LogLevel { get; set; }
    }
}
