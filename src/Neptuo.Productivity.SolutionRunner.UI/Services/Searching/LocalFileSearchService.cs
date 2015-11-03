using Neptuo.FileSystems;
using Neptuo.FileSystems.Features;
using Neptuo.FileSystems.Features.Searching;
using Neptuo.Productivity.SolutionRunner.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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

        public Task SearchAsync(string searchPattern, FileSearchMode mode, IFileCollection files)
        {
            IEnumerable<IFile> matchedFiles = filePathSearch.FindFiles(TextSearch.CreateContained(searchPattern), TextSearch.CreateMatched("sln"));
            files.Clear();

            IEnumerable<FileViewModel> viewModels = matchedFiles
                    .Select(f => new FileViewModel(f.Name, f.WithAbsolutePath().AbsolutePath, pinStateService.IsPinned(f.WithAbsolutePath().AbsolutePath)))
                    .OrderBy(vm => !vm.IsPinned)
                    .ThenBy(vm => vm.Name)
                    .Take(20);

            foreach (FileViewModel viewModel in viewModels)
                files.Add(viewModel.Name, viewModel.Path, viewModel.IsPinned);

            return Task.FromResult(true);
        }
    }
}
