﻿using Neptuo.FileSystems;
using Neptuo.FileSystems.Features;
using Neptuo.FileSystems.Features.Searching;
using Neptuo.Productivity.SolutionRunner.ViewModels;
using Neptuo.Threading.Tasks;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Neptuo.Productivity.SolutionRunner.Services.Searching
{
    public class LocalFileSearchService : IFileSearchService
    {
        private readonly IFilePathSearch filePathSearch;
        private readonly IPinStateService pinStateService;

        public LocalFileSearchService(string directoryPath, IPinStateService pinStateService)
        {
            Ensure.NotNull(pinStateService, "pinStateService");
            this.filePathSearch = new LocalSearchProvider(directoryPath);
            this.pinStateService = pinStateService;
        }

        public Task InitializeAsync()
        {
            return Async.CompletedTask;
        }

        public Task SearchAsync(string searchPattern, FileSearchMode mode, int count, IFileCollection files, CancellationToken cancellationToken)
        {
            IEnumerable<IFile> matchedFiles = filePathSearch.FindFiles(TextSearch.CreateContained(searchPattern), TextSearch.CreateMatched("sln"));
            files.Clear();

            IEnumerable<FileViewModel> viewModels = matchedFiles
                .Select(f => new FileViewModel(f.Name, f.WithAbsolutePath().AbsolutePath, pinStateService.IsPinned(f.WithAbsolutePath().AbsolutePath)))
                .OrderBy(vm => !vm.IsPinned)
                .ThenBy(vm => vm.Name)
                .Take(count);

            foreach (FileViewModel viewModel in viewModels)
                files.Add(viewModel.Name, viewModel.Path, viewModel.IsPinned);

            return Task.FromResult(true);
        }
    }
}
