using Neptuo.Productivity.SolutionRunner.Services;
using Neptuo.Productivity.SolutionRunner.Services.Configuration;
using Neptuo.Productivity.SolutionRunner.ViewModels;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Neptuo.Productivity.SolutionRunner
{
    internal class AppPinStateService : IPinStateService
    {
        private readonly ISettingsService settingsService;
        private readonly ISettings settings;

        public AppPinStateService(ISettingsService settingsService, ISettings settings)
        {
            Ensure.NotNull(settingsService, "settingsService");
            Ensure.NotNull(settings, "settings");
            this.settingsService = settingsService;
            this.settings = settings;

            EventManager.FilePinned += OnPinned;
        }

        private void OnPinned(FileViewModel viewModel)
        {
            bool isChanged = false;
            HashSet<string> pinnedFiles = new HashSet<string>(settings.PinnedFiles);
            if (viewModel.IsPinned)
                isChanged = pinnedFiles.Add(viewModel.Path);
            else
                isChanged = pinnedFiles.Remove(viewModel.Path);

            if (isChanged)
            {
                settings.PinnedFiles = pinnedFiles.ToList();
                settingsService.SaveAsync(settings);
            }
        }

        public IEnumerable<string> Enumerate() => settings.PinnedFiles;

        public bool IsPinned(string path)
        {
            Ensure.NotNullOrEmpty(path, "path");
            return Enumerate().Contains(path);
        }
    }
}
