using System;
using System.Collections.Generic;
using System.IO;
using System.IO.IsolatedStorage;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Neptuo.Productivity.SolutionRunner.Services.Statistics
{
    /// <summary>
    /// A service for counting numbers.
    /// </summary>
    public class CountingService : ICountingAppender
    {
        public const string FileName = "Statistics.dat";

        private void Append(string line)
        {
            IsolatedStorageFile storage = IsolatedStorageFile.GetUserStoreForApplication();
            using (IsolatedStorageFileStream stream = new IsolatedStorageFileStream(FileName, FileMode.Append, storage))
            using (StreamWriter writer = new StreamWriter(stream))
                writer.WriteLine(line);
        }

        public void Application(string path)
        {
            Append(path);
        }

        public void File(string applicationPath, string filePath)
        {
            Append(String.Concat(applicationPath, ";", filePath));
        }

        public void File(string applicationPath, string argumentsTemplate, string filePath)
        {
            Append(String.Concat(applicationPath, ";", argumentsTemplate, ";", filePath));
        }
    }
}
