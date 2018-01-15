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
        private HashSet<string> pinnedFiles;

        public AppPinStateService(ISettingsService settingsService, ISettings settings)
        {
            Ensure.NotNull(settingsService, "settingsService");
            Ensure.NotNull(settings, "settings");
            this.settingsService = settingsService;
            this.settings = settings;

            EventManager.FilePinned += OnPinned;
        }

        public HashSet<string> GetPinnedFiles()
        {
            if (pinnedFiles == null)
            {
                pinnedFiles = new HashSet<string>();

                string rawValue = settings.PinnedFiles;
                if (!String.IsNullOrEmpty(rawValue))
                {
                    foreach (string filePath in rawValue.Split(new char[] { ';' }, StringSplitOptions.RemoveEmptyEntries))
                    {
                        if (File.Exists(filePath))
                            pinnedFiles.Add(filePath);
                    }
                }
            }

            return pinnedFiles;
        }

        private void OnPinned(FileViewModel viewModel)
        {
            HashSet<string> pinnedFiles = GetPinnedFiles();
            if (viewModel.IsPinned)
                pinnedFiles.Add(viewModel.Path);
            else
                pinnedFiles.Remove(viewModel.Path);

            settings.PinnedFiles = String.Join(";", pinnedFiles);
            settingsService.SaveAsync(settings);
        }

        public IEnumerable<string> Enumerate()
        {
            return GetPinnedFiles();
        }

        public bool IsPinned(string path)
        {
            Ensure.NotNullOrEmpty(path, "path");
            return GetPinnedFiles().Contains(path);
        }
    }
}
