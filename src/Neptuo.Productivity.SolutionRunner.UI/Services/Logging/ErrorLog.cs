using Neptuo.Logging.Serialization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Neptuo.Logging;
using Neptuo;
using Neptuo.Logging.Serialization.Formatters;
using System.IO.IsolatedStorage;
using System.IO;

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

        private bool TryGetStorage(out IsolatedStorageFile file)
        {
            try
            {
                file = IsolatedStorageFile.GetUserStoreForApplication();
                return true;
            }
            catch (Exception)
            {
                file = null;
                return false;
            }
        }

        private void Append(string line)
        {
            if (TryGetStorage(out IsolatedStorageFile storage))
            {
                using (IsolatedStorageFileStream stream = new IsolatedStorageFileStream(String.Format(FileNameFormat, DateTime.Now), FileMode.Append, storage))
                using (StreamWriter writer = new StreamWriter(stream))
                    writer.WriteLine(line);
            }
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

        public IEnumerable<string> GetFileNames()
        {
            if (TryGetStorage(out IsolatedStorageFile storage))
                return storage.GetFileNames("*.log");

            return Enumerable.Empty<string>();
        }

        public string FindFileContent(string fileName)
        {
            Ensure.NotNullOrEmpty(fileName, "fileName");
            if (TryGetStorage(out IsolatedStorageFile storage) && fileName.EndsWith(".log") && storage.FileExists(fileName))
            {
                using (IsolatedStorageFileStream stream = storage.OpenFile(fileName, FileMode.Open))
                using (StreamReader reader = new StreamReader(stream))
                {
                    string content = reader.ReadToEnd();
                    return content;
                }
            }

            return null;
        }
    }
}
