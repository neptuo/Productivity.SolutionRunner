using Neptuo.Activators;
using Neptuo.Logging;
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
    public class MainViewModelFactory : IFactory<MainViewModel>, IDiagnosticService
    {
        private readonly IPinStateService pinStateService;
        private readonly ISettings settings;
        private readonly IApplicationLoader mainApplicationLoader;
        private readonly ILogFactory logFactory;
        private readonly PropertyChangedEventHandler propertyChangedHandler;

        internal MainViewModelFactory(IPinStateService pinStateService, ISettings settings, IApplicationLoader mainApplicationLoader, ILogFactory logFactory, PropertyChangedEventHandler propertyChangedHandler)
        {
            Ensure.NotNull(pinStateService, "pinStateService");
            Ensure.NotNull(settings, "settings");
            Ensure.NotNull(mainApplicationLoader, "mainApplicationLoader");
            Ensure.NotNull(logFactory, "logFactory");
            this.pinStateService = pinStateService;
            this.settings = settings;
            this.mainApplicationLoader = mainApplicationLoader;
            this.logFactory = logFactory;
            this.propertyChangedHandler = propertyChangedHandler;
        }

        public MainViewModel Create()
        {
            UiBackgroundContext backgroundContext = new UiBackgroundContext();

            MainViewModel viewModel = new MainViewModel(
                new PinnedForEmptyPatternFileSearchService(
                    //new DelayedFileSearchService(
                    //    Dispatcher,
                    //    CreateFileSearchService()
                    //),
                    //this
                    CreateFileSearchService(backgroundContext),
                    pinStateService
                ),
                backgroundContext,
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
            foreach (string filePath in pinStateService.Enumerate())
                files.Add(Path.GetFileNameWithoutExtension(filePath), filePath, true);

            return viewModel;
        }

        private string directoryPath;
        private FileSystemWatcherSearchService fileSearchService;

        private IFileSearchService CreateFileSearchService(IBackgroundContext backgroundContext)
        {
            if (fileSearchService == null || directoryPath != settings.SourceDirectoryPath)
            {
                directoryPath = settings.SourceDirectoryPath;
                fileSearchService = new FileSystemWatcherSearchService(directoryPath, pinStateService, backgroundContext, logFactory);
            }

            return fileSearchService;
        }

        public void ClearService()
        {
            fileSearchService = null;
        }

        #region IDiagnosticService

        public bool IsAvailable
        {
            get
            {
                IDiagnosticService target = fileSearchService as IDiagnosticService;
                if (target == null)
                    return false;

                return target.IsAvailable;
            }
        }

        public IEnumerable<string> EnumerateFiles()
        {
            IDiagnosticService target = fileSearchService as IDiagnosticService;
            if (target == null)
                return Enumerable.Empty<string>();

            return target.EnumerateFiles();
        }

        #endregion
    }
}
