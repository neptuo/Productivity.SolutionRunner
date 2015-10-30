using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Neptuo.Productivity.SolutionRunner.Services.Searching
{
    public class PinnedForEmptyPatternFileSearchService : DisposableBase, IFileSearchService
    {
        private readonly IPinStateService pinStateService;
        private readonly IFileSearchService innerService;

        public PinnedForEmptyPatternFileSearchService(IFileSearchService innerService, IPinStateService pinStateService)
        {
            Ensure.NotNull(innerService, "innerService");
            Ensure.NotNull(pinStateService, "pinStateService");
            this.innerService = innerService;
            this.pinStateService = pinStateService;
        }

        public Task SearchAsync(string searchPattern, IFileCollection files)
        {
            if (String.IsNullOrEmpty(searchPattern))
            {
                files.Clear();
                foreach (string filePath in pinStateService.GetList())
                    files.Add(Path.GetFileNameWithoutExtension(filePath), filePath, true);

                return Task.FromResult(true);
            }

            return innerService.SearchAsync(searchPattern, files);
        }

        protected override void DisposeManagedResources()
        {
            base.DisposeManagedResources();

            IDisposable disposable = innerService as IDisposable;
            if (disposable != null)
                disposable.Dispose();
        }
    }
}
