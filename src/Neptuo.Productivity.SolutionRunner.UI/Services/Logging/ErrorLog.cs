using Neptuo.Logging;
using Neptuo.Logging.Serialization;
using Neptuo.Logging.Serialization.Formatters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace Neptuo.Productivity.SolutionRunner.Services.Logging
{
    public class ErrorLog : ILogSerializer, ILogProvider
    {
        public const string FileNameFormat = "ErrorLog_{0:yyyy-MM}.log";

        private readonly ILogFormatter formatter;

        public ErrorLog(ILogFormatter formatter)
        {
            Ensure.NotNull(formatter, "formatter");
            this.formatter = formatter;
        }

        private void Append(string line)
        {
            SequenceIsolatedFile file = new SequenceIsolatedFile(String.Format(FileNameFormat, DateTime.Now));
            file.Append(line);
        }

        public void Append(string scopeName, LogLevel level, object model)
        {
            string message = formatter.Format(scopeName, level, model);
            Append(message);
        }

        public bool IsEnabled(string scopeName, LogLevel level)
        {
            return level == LogLevel.Error || level == LogLevel.Fatal;
        }

        public IEnumerable<string> GetFileNames() => SequenceIsolatedFile.EnumerateNames("*.log");

        public string FindFileContent(string fileName)
        {
            Ensure.NotNullOrEmpty(fileName, "fileName");
            if (fileName.EndsWith(".log") && SequenceIsolatedFile.Exists(fileName))
            {
                SequenceIsolatedFile file = new SequenceIsolatedFile(fileName);
                return String.Join(Environment.NewLine, file.Enumerate());
            }

            return null;
        }
    }
}
