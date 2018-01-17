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
            List<string> pinnedFiles = settings.PinnedFiles.ToList();
            if (viewModel.IsPinned)
                pinnedFiles.Add(viewModel.Path);
            else
                pinnedFiles.Remove(viewModel.Path);

            settings.PinnedFiles = pinnedFiles;
            settingsService.SaveAsync(settings);
        }

        public IEnumerable<string> Enumerate() => settings.PinnedFiles;

        public bool IsPinned(string path)
        {
            Ensure.NotNullOrEmpty(path, "path");
            return Enumerate().Contains(path);
        }
    }
}
