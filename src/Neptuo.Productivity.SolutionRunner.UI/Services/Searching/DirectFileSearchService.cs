using Neptuo.FileSystems;
using Neptuo.Threading.Tasks;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Neptuo.Productivity.SolutionRunner.Services.Searching
{
    public class DirectFileSearchService : IFileSearchService
    {
        private readonly IPinStateService pinStateService;
        private readonly string directoryPath;

        public DirectFileSearchService(string directoryPath, IPinStateService pinStateService)
        {
            Ensure.Condition.DirectoryExists(directoryPath, "directoryPath");
            Ensure.NotNull(pinStateService, "pinStateService");
            this.directoryPath = directoryPath;
            this.pinStateService = pinStateService;
        }

        public Task InitializeAsync()
        {
            return Async.CompletedTask;
        }

        public Task SearchAsync(string searchPattern, FileSearchMode mode, int count, IFileCollection files, CancellationToken cancellationToken)
        {
            files.Clear();

            switch (mode)
            {
                case FileSearchMode.StartsWith:
                    searchPattern = String.Format("{0}*.sln", searchPattern);
                    break;
                case FileSearchMode.Contains:
                    searchPattern = String.Format("*{0}*.sln", searchPattern);
                    break;
                default:
                    throw Ensure.Exception.NotSupportedSearchMode(mode);
            }

            IEnumerable<string> filePaths = Directory
                .EnumerateFiles(directoryPath, searchPattern, SearchOption.AllDirectories)
                .OrderBy(f => !pinStateService.IsPinned(f))
                .ThenBy(f => Path.GetFileNameWithoutExtension(f))
                .Take(count);
                //.EnumerateFiles(directoryPath, "*.sln", SearchOption.AllDirectories)

            foreach (string filePath in filePaths)
                files.Add(Path.GetFileNameWithoutExtension(filePath), filePath, pinStateService.IsPinned(filePath));

            return Task.FromResult(true);
        }
    }
}
