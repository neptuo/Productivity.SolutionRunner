using Neptuo.Activators;
using Neptuo.Productivity.SolutionRunner.Services;
using Neptuo.Productivity.SolutionRunner.Services.Applications;
using Neptuo.Productivity.SolutionRunner.Services.Configuration;
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
        private readonly ISettings settings;
        private readonly IApplicationLoader mainApplicationLoader;
        private readonly Func<HashSet<string>> pinnedFilesGetter;
        private readonly PropertyChangedEventHandler propertyChangedHandler;

        internal MainViewModelFactory(IPinStateService pinStateService, ISettings settings, IApplicationLoader mainApplicationLoader, Func<HashSet<string>> pinnedFilesGetter, PropertyChangedEventHandler propertyChangedHandler)
        {
            Ensure.NotNull(pinStateService, "pinStateService");
            Ensure.NotNull(settings, "settings");
            Ensure.NotNull(mainApplicationLoader, "mainApplicationLoader");
            Ensure.NotNull(pinnedFilesGetter, "pinnedFilesGetter");
            this.pinStateService = pinStateService;
            this.settings = settings;
            this.mainApplicationLoader = mainApplicationLoader;
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
                () => settings.FileSearchMode,
                () => settings.FileSearchCount
            );

            if (propertyChangedHandler != null)
                viewModel.PropertyChanged += propertyChangedHandler;

            ApplicationFilteredCollection applications = new ApplicationFilteredCollection(settings, viewModel);
            mainApplicationLoader.Add(applications);

            AdditionalApplicationLoader additionalLoader = new AdditionalApplicationLoader(settings);
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
            if (fileSearchService == null || directoryPath != settings.SourceDirectoryPath)
            {
                directoryPath = settings.SourceDirectoryPath;
                fileSearchService = new FileSystemWatcherSearchService(directoryPath, pinStateService);
            }

            return fileSearchService;
        }

        public void ClearService()
        {
            fileSearchService = null;
        }
    }
}
