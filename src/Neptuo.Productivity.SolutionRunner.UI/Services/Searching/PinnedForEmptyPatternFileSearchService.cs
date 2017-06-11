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
    public class PinnedForEmptyPatternFileSearchService : DisposableBase, IFileSearchService
    {
        private readonly IPinStateService pinStateService;
        private readonly IFileSearchService innerService;
        private CancellationTokenSource lastCancellation;

        public PinnedForEmptyPatternFileSearchService(IFileSearchService innerService, IPinStateService pinStateService)
        {
            Ensure.NotNull(innerService, "innerService");
            Ensure.NotNull(pinStateService, "pinStateService");
            this.innerService = innerService;
            this.pinStateService = pinStateService;
        }

        public Task InitializeAsync()
        {
            return innerService.InitializeAsync();
        }

        public Task SearchAsync(string searchPattern, FileSearchMode mode, int count, IFileCollection files, CancellationToken cancellationToken)
        {
            if (String.IsNullOrEmpty(searchPattern))
            {
                files.Clear();
                foreach (string filePath in pinStateService.Enumerate())
                    files.Add(Path.GetFileNameWithoutExtension(filePath), filePath, true);

                if (lastCancellation != null)
                {
                    lastCancellation.Cancel();
                    lastCancellation = null;
                }

                return Task.FromResult(true);
            }

            if (cancellationToken.IsCancellationRequested)
                return Async.CompletedTask;

            lastCancellation = new CancellationTokenSource();
            cancellationToken.Register(() => lastCancellation.Cancel());

            Task result = innerService.SearchAsync(searchPattern, mode, count, files, lastCancellation.Token);
            return result;
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
