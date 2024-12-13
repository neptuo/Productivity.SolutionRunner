using Neptuo.Collections.Specialized;
using Neptuo.Formatters;
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
    public class JsonSettings : ISettings, ICompositeModel
    {
        public string FileSearchPattern { get; set; }
        public bool IsAutoSelectApplicationVersion { get; set; }
        public bool IsDismissedWhenLostFocus { get; set; }
        public bool IsDisplayedPathTrimmedToLastFolderName { get; set; }
        public bool IsFileNameRemovedFromDisplayedPath { get; set; }
        public bool IsFileSearchPatternSaved { get; set; }
        public bool IsHiddentOnStartup { get; set; }
        public bool IsLastUsedApplicationSavedAsPrefered { get; set; }
        public bool IsProjectCountEnabled { get; set; }
        public bool IsStatisticsCounted { get; set; }
        public bool IsTrayIcon { get; set; }
        public IReadOnlyList<string> PinnedFiles { get; set; }
        public double PositionTop { get; set; }
        public double PositionLeft { get; set; }
        public PositionMode PositionMode { get; set; }
        public string PreferedApplicationPath { get; set; }
        public string RunKey { get; set; }
        public string SourceDirectoryPath { get; set; }
        public Theme ThemeMode { get; set; }
        public AdditionalApplicationCollection AdditionalApplications { get; set; }
        public IReadOnlyList<string> HiddenMainApplications { get; set; }
        public int FileSearchCount { get; set; }
        public FileSearchMode FileSearchMode { get; set; }
        public Version AutoSelectApplicationMinimalVersion { get; set; }

        public LogLevel LogLevel { get; set; } = LogLevel.Error;

        public void Load(ICompositeStorage storage)
        {
            int configurationVersion = storage.Get("ConfigurationVersion", 1);

            FileSearchPattern = storage.Get<string>("FileSearchPattern");
            IsAutoSelectApplicationVersion = storage.Get<bool>("IsAutoSelectApplicationVersion");
            IsDismissedWhenLostFocus = storage.Get<bool>("IsDismissedWhenLostFocus");
            IsDisplayedPathTrimmedToLastFolderName = storage.Get<bool>("IsDisplayedPathTrimmedToLastFolderName");
            IsFileNameRemovedFromDisplayedPath = storage.Get<bool>("IsFileNameRemovedFromDisplayedPath");
            IsFileSearchPatternSaved = storage.Get<bool>("IsFileSearchPatternSaved");
            IsHiddentOnStartup = storage.Get<bool>("IsHiddentOnStartup");
            IsLastUsedApplicationSavedAsPrefered = storage.Get<bool>("IsLastUsedApplicationSavedAsPrefered");
            IsProjectCountEnabled = storage.Get<bool>("IsProjectCountEnabled");
            IsStatisticsCounted = storage.Get<bool>("IsStatisticsCounted");
            IsTrayIcon = storage.Get<bool>("IsTrayIcon");
            PinnedFiles = storage.Get<IReadOnlyList<string>>("PinnedFiles");
            PositionTop = storage.Get<int>("PositionTop");
            PositionLeft = storage.Get<int>("PositionLeft");
            PositionMode = storage.Get<PositionMode>("PositionMode");
            PreferedApplicationPath = storage.Get<string>("PreferedApplicationPath");
            RunKey = storage.Get<string>("RunKey");
            SourceDirectoryPath = storage.Get<string>("SourceDirectoryPath");
            ThemeMode = storage.Get<Theme>("ThemeMode");

            AdditionalApplications = new AdditionalApplicationCollection();
            if (storage.TryGet("AdditionalApplications", out var additionalApplications))
                AdditionalApplications.Load(additionalApplications);

            HiddenMainApplications = storage.Get<IReadOnlyList<string>>("HiddenMainApplications");
            FileSearchCount = storage.Get<int>("FileSearchCount");
            FileSearchMode = storage.Get<FileSearchMode>("FileSearchMode");
            AutoSelectApplicationMinimalVersion = storage.Get<Version>("AutoSelectApplicationMinimalVersion");
            LogLevel = storage.Get("LogLevel", LogLevel.Error);
        }

        public void Save(ICompositeStorage storage)
        {
            storage.Add("ConfigurationVersion", 1);

            storage.Add("FileSearchPattern", FileSearchPattern);
            storage.Add("IsAutoSelectApplicationVersion", IsAutoSelectApplicationVersion);
            storage.Add("IsDismissedWhenLostFocus", IsDismissedWhenLostFocus);
            storage.Add("IsDisplayedPathTrimmedToLastFolderName", IsDisplayedPathTrimmedToLastFolderName);
            storage.Add("IsFileNameRemovedFromDisplayedPath", IsFileNameRemovedFromDisplayedPath);
            storage.Add("IsFileSearchPatternSaved", IsFileSearchPatternSaved);
            storage.Add("IsHiddentOnStartup", IsHiddentOnStartup);
            storage.Add("IsLastUsedApplicationSavedAsPrefered", IsLastUsedApplicationSavedAsPrefered);
            storage.Add("IsProjectCountEnabled", IsProjectCountEnabled);
            storage.Add("IsStatisticsCounted", IsStatisticsCounted);
            storage.Add("IsTrayIcon", IsTrayIcon);
            storage.Add("PinnedFiles", PinnedFiles);
            storage.Add("PositionTop", PositionTop);
            storage.Add("PositionLeft", PositionLeft);
            storage.Add("PositionMode", PositionMode);
            storage.Add("PreferedApplicationPath", PreferedApplicationPath);
            storage.Add("RunKey", RunKey);
            storage.Add("SourceDirectoryPath", SourceDirectoryPath);
            storage.Add("ThemeMode", ThemeMode);
            AdditionalApplications.Save(storage.Add("AdditionalApplications"));
            storage.Add("HiddenMainApplications", HiddenMainApplications);
            storage.Add("FileSearchCount", FileSearchCount);
            storage.Add("FileSearchMode", FileSearchMode);
            storage.Add("AutoSelectApplicationMinimalVersion", AutoSelectApplicationMinimalVersion);
            storage.Add("LogLevel", LogLevel);
        }
    }
}
