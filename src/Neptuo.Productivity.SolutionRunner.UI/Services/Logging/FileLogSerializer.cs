﻿using Neptuo.Logging;
using Neptuo.Logging.Serialization;
using Neptuo.Logging.Serialization.Formatters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Neptuo.Productivity.SolutionRunner.Services.Logging
{
    public class FileLogSerializer : ILogSerializer
    {
        public const string FileNameFormat = "{0}_{1:yyyy-MM}.log";

        private readonly ILogFormatter formatter;
        private readonly Func<LogLevel> levelThreshold;

        public FileLogSerializer(ILogFormatter formatter, Func<LogLevel> levelThreshold)
        {
            Ensure.NotNull(formatter, "formatter");
            Ensure.NotNull(levelThreshold, "levelThreshold");
            this.formatter = formatter;
            this.levelThreshold = levelThreshold;
        }

        public void Append(string scopeName, LogLevel level, object model)
        {
            if (IsEnabled(scopeName, level))
            {
                Ensure.NotNull(scopeName, "scopeName");
                string rootName = GetRootName(scopeName);
                string message = formatter.Format(scopeName, level, model);
                SequenceIsolatedFile file = new SequenceIsolatedFile(String.Format(FileNameFormat, rootName, DateTime.Now));
                file.Append(message);
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
    }
}
