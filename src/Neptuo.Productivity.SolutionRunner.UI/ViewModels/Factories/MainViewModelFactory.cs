using Neptuo;
using Neptuo.Activators;
using Neptuo.Productivity.SolutionRunner.Properties;
using Neptuo.Productivity.SolutionRunner.Services;
using Neptuo.Productivity.SolutionRunner.Services.Applications;
using Neptuo.Productivity.SolutionRunner.Services.Searching;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Neptuo.Productivity.SolutionRunner.ViewModels.Factories
{
    public class MainViewModelFactory : IFactory<MainViewModel>
    {
        private readonly IPinStateService pinStateService;
        private readonly Settings settings;
        private readonly VsVersionLoader vsLoader;
        private readonly Func<HashSet<string>> pinnedFilesGetter;
        private readonly PropertyChangedEventHandler propertyChangedHandler;

        internal MainViewModelFactory(IPinStateService pinStateService, Settings settings, VsVersionLoader vsLoader, Func<HashSet<string>> pinnedFilesGetter, PropertyChangedEventHandler propertyChangedHandler)
        {
            Ensure.NotNull(pinStateService, "pinStateService");
            Ensure.NotNull(settings, "settings");
            Ensure.NotNull(vsLoader, "vsLoader");
            Ensure.NotNull(pinnedFilesGetter, "pinnedFilesGetter");
            this.pinStateService = pinStateService;
            this.settings = settings;
            this.vsLoader = vsLoader;
            this.pinnedFilesGetter = pinnedFilesGetter;
            this.propertyChangedHandler = propertyChangedHandler;
        }

        public MainViewModel Create()
        {
            MainViewModel viewModel = new MainViewModel(
                new PinnedForEmptyPatternFileSearchService(
                    //new DelayedFileSearchService(
                    //    Dispatcher,
                    //    CreateFileSearchService()
                    //),
                    //this
                    CreateFileSearchService(),
                    pinStateService
                ),
                Settings.Default.GetFileSearchMode,
                Settings.Default.GetFileSearchCount
            );

            if (propertyChangedHandler != null)
                viewModel.PropertyChanged += propertyChangedHandler;

            ApplicationFilteredCollection applications = new ApplicationFilteredCollection(Settings.Default, viewModel);
            vsLoader.Add(applications);

            AdditionalApplicationLoader additionalLoader = new AdditionalApplicationLoader();
            additionalLoader.Add(viewModel);

            IFileCollection files = viewModel;
            foreach (string filePath in pinnedFilesGetter())
                files.Add(Path.GetFileNameWithoutExtension(filePath), filePath, true);

            return viewModel;
        }

        private string directoryPath;
        private FileSystemWatcherSearchService fileSearchService;

        private IFileSearchService CreateFileSearchService()
        {
            if (fileSearchService == null || directoryPath != Settings.Default.SourceDirectoryPath)
            {
                directoryPath = Settings.Default.SourceDirectoryPath;
                fileSearchService = new FileSystemWatcherSearchService(directoryPath, pinStateService);
            }

            return fileSearchService;
        }
    }
}
