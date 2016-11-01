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

        public Task InitializeAsync()
        {
            return innerService.InitializeAsync();
        }

        public Task SearchAsync(string searchPattern, FileSearchMode mode, int count, IFileCollection files, CancellationToken cancellationToken)
        {
            TaskCompletionSource<bool> result = new TaskCompletionSource<bool>();
            Task.Factory.StartNew(
                DelayedSearchHandler,
                new Context()
                {
                    RequestIndex = ++currentRequestIndex,
                    CompletionSource = result,
                    SearchPattern = searchPattern,
                    Mode = mode,
                    Count = count,
                    Files = files,
                    CancellationToken = cancellationToken
                },
                cancellationToken
            );

            return result.Task;
        }

        /// <summary>
        /// Executed in own thread for delayed searching.
        /// </summary>
        /// <param name="parameter">The context of type <see cref="Context"/>.</param>
        private void DelayedSearchHandler(object parameter)
        {
            Thread.Sleep(delay);
            Context context = (Context)parameter;

            if (currentRequestIndex == context.RequestIndex)
            {
                dispatcher.Run(() =>
                {
                    if (context.CancellationToken.IsCancellationRequested)
                        return;

                    if (currentRequestIndex == context.RequestIndex)
                    {
                        currentRequestIndex = 0;
                        innerService
                            .SearchAsync(context.SearchPattern, context.Mode, context.Count, context.Files, context.CancellationToken)
                            .ContinueWith(t => context.CompletionSource.SetResult(true));
                    }
                    else
                    {
                        context.CompletionSource.SetCanceled();
                    }
                });
            }
            else
            {
                context.CompletionSource.SetCanceled();
            }
        }

        protected override void DisposeManagedResources()
        {
            base.DisposeManagedResources();

            IDisposable disposable = innerService as IDisposable;
            if (disposable != null)
                disposable.Dispose();
        }

        /// <summary>
        /// A context of thread searching hit.
        /// </summary>
        private class Context
        {
            public int RequestIndex { get; set; }
            public TaskCompletionSource<bool> CompletionSource { get; set; }
            public string SearchPattern { get; set; }
            public FileSearchMode Mode { get; set; }
            public int Count { get; set; }
            public IFileCollection Files { get; set; }
            public CancellationToken CancellationToken { get; set; }
        }
    }
}
