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
        string AdditionalApplications { get; set; }
        string AutoSelectApplicationMinimalVersion { get; set; }
        int FileSearchCount { get; set; }
        string FileSearchMode { get; set; }
        string FileSearchPattern { get; set; }
        string HiddenMainApplications { get; set; }
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
        string PinnedFiles { get; set; }
        double PositionLeft { get; set; }
        PositionMode PositionMode { get; set; }
        double PositionTop { get; set; }
        string PreferedApplicationPath { get; set; }
        string RunKey { get; set; }
        string SourceDirectoryPath { get; set; }
        ThemeMode ThemeMode { get; set; }

        Version GetAutoSelectApplicationMinimalVersion();
        int GetFileSearchCount();
        FileSearchMode GetFileSearchMode();
        string[] GetHiddenMainApplications();
        void SetAutoSelectApplicationMinimalVersion(Version version);
    }
}
