using Neptuo.Windows.Threading;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Timers;
using System.Windows.Threading;

namespace Neptuo.Productivity.SolutionRunner.Services.Searching
{
    public class DelayedFileSearchService : DisposableBase, IFileSearchService
    {
        private readonly int delay = 500;
        private readonly DispatcherHelper dispatcher;
        private readonly IFileSearchService innerService;
        private int currentRequestIndex = 0;

        public DelayedFileSearchService(Dispatcher dispatcher, IFileSearchService innerService)
        {
            Ensure.NotNull(dispatcher, "dispatcher");
            Ensure.NotNull(innerService, "innerService");
            this.dispatcher = new DispatcherHelper(dispatcher);
            this.innerService = innerService;
        }

        public Task SearchAsync(string searchPattern, FileSearchMode mode, int count, IFileCollection files)
        {
            return Task.Factory.StartNew(
                (requestIndex) =>
                {
                    Thread.Sleep(delay);

                    if (currentRequestIndex == (int)requestIndex)
                    {
                        dispatcher.Run(() =>
                        {
                            if (currentRequestIndex == (int)requestIndex)
                            {
                                currentRequestIndex = 0;
                                innerService.SearchAsync(searchPattern, mode, count, files);
                            }
                        });
                    }
                },
                ++currentRequestIndex
            );
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
