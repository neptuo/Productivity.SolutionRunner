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
        public ErrorLogSerializer(ILogFormatter formatter)
            : base(formatter)
        { }

        protected override string GetRootName(string scopeName)
        {
            return "Error";
        }

        public override bool IsEnabled(string scopeName, LogLevel level)
        {
            return level == LogLevel.Error || level == LogLevel.Fatal;
        }
    }
}
