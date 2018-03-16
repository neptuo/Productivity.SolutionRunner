using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Timers;

namespace Neptuo.Productivity.SolutionRunner.Services.Logging
{
    public class BatchExecutor<T> : DisposableBase
    {
        private readonly Action<IEnumerable<T>> worker;
        private readonly Timer timer;
        private readonly List<T> items = new List<T>();
        private readonly object itemsLock = new object();

        public BatchExecutor(Action<IEnumerable<T>> worker, TimeSpan threshold)
        {
            Ensure.NotNull(worker, "worker");
            this.worker = worker;

            if (threshold > TimeSpan.Zero)
            {
                timer = new Timer(threshold.TotalMilliseconds);
                timer.Elapsed += OnTimer;
                timer.Start();
            }
        }

        private void OnTimer(object sender, ElapsedEventArgs e)
            => Flush();

        public void Add(T item)
        {
            if (timer == null)
            {
                worker(new T[] { item });
                return;
            }

            lock (itemsLock)
            {
                items.Add(item);
            }
        }

        public void Flush()
        {
            lock (itemsLock)
            {
                worker(items);
                items.Clear();
            }
        }

        protected override void DisposeManagedResources()
        {
            base.DisposeManagedResources();

            Flush();

            if (timer != null)
                timer.Dispose();
        }
    }
}
