using Neptuo.Logging;
using Neptuo.Logging.Serialization;
using Neptuo.Logging.Serialization.Formatters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Neptuo.Productivity.SolutionRunner.Services.Logging
{
    public class FileLogSerializer : DisposableBase, ILogSerializer
    {
        public const string FileNameFormat = "{0}_{1:yyyy-MM}.log";

        private readonly ILogFormatter formatter;
        private readonly Func<LogLevel> levelThreshold;
        private readonly BatchExecutor<(string, string)> executor;

        public FileLogSerializer(ILogFormatter formatter, Func<LogLevel> levelThreshold, FileLogBatchFactory executorFactory)
        {
            Ensure.NotNull(formatter, "formatter");
            Ensure.NotNull(levelThreshold, "levelThreshold");
            this.formatter = formatter;
            this.levelThreshold = levelThreshold;
            this.executor = executorFactory.Create(OnWriteToFile);
        }

        public void Append(string scopeName, LogLevel level, object model)
        {
            if (IsEnabled(scopeName, level))
            {
                Ensure.NotNull(scopeName, "scopeName");
                string rootName = GetRootName(scopeName);
                string message = formatter.Format(scopeName, level, model);

                executor.Add((rootName, message));
            }
        }

        protected virtual string GetRootName(string scopeName)
            => scopeName.Split(new char[] { '.' }, StringSplitOptions.RemoveEmptyEntries).First();

        public virtual bool IsEnabled(string scopeName, LogLevel level)
        {
            // These are in ErrorLog.
            if (scopeName == ".Root" && (level == LogLevel.Error || level == LogLevel.Fatal))
                return false;

            // Logging settings.
            return level >= levelThreshold();
        }

        private static void OnWriteToFile(IEnumerable<(string rootName, string message)> items)
        {
            foreach (var group in items.GroupBy(i => i.rootName))
            {
                SequenceIsolatedFile file = new SequenceIsolatedFile(String.Format(FileNameFormat, group.Key, DateTime.Now));
                file.Append(group.Select(i => i.message));
            }
        }

        protected override void DisposeManagedResources()
        {
            base.DisposeManagedResources();
            executor.Dispose();
        }
    }
}
