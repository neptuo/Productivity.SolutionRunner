using Neptuo.Logging;
using Neptuo.Logging.Serialization.Formatters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace Neptuo.Productivity.SolutionRunner.Services.Logging
{
    public class ErrorLogSerializer : FileLogSerializer
    {
        public const string RootName = "Error";

        public ErrorLogSerializer(ILogFormatter formatter)
            : base(formatter)
        { }

        protected override string GetRootName(string scopeName)
            => RootName;

        public override bool IsEnabled(string scopeName, LogLevel level)
            => level == LogLevel.Error || level == LogLevel.Fatal;
    }
}
