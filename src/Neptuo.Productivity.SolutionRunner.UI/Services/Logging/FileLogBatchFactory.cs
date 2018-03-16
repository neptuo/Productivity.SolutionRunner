using Neptuo.Activators;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Neptuo.Productivity.SolutionRunner.Services.Logging
{
    public class FileLogBatchFactory : IFactory<BatchExecutor<(string, string)>, Action<IEnumerable<(string, string)>>>
    {
        private readonly List<BatchExecutor<(string, string)>> executors = new List<BatchExecutor<(string, string)>>();
        private readonly TimeSpan threshold;

        public FileLogBatchFactory(TimeSpan threshold)
        {
            this.threshold = threshold;
        }

        public BatchExecutor<(string, string)> Create(Action<IEnumerable<(string, string)>> worker)
        {
            BatchExecutor<(string, string)> executor = new BatchExecutor<(string, string)>(worker, threshold);
            executors.Add(executor);
            return executor;
        }

        public void Flush()
        {
            foreach (var executor in executors)
                executor.Flush();
        }
    }
}
