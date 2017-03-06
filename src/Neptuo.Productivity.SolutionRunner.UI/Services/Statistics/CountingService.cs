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
    public class CountingService : ICountingAppender, ICountingReader
    {
        public const string FileName = "Statistics.dat";

        private IsolatedStorageFile GetStorage()
        {
            return IsolatedStorageFile.GetUserStoreForApplication();
        }

        private void Append(string line)
        {
            IsolatedStorageFile storage = GetStorage();
            using (IsolatedStorageFileStream stream = new IsolatedStorageFileStream(FileName, FileMode.Append, storage))
            using (StreamWriter writer = new StreamWriter(stream))
                writer.WriteLine(line);
        }

        private string GetDateTimeNow()
        {
            return DateTime.Now.ToString("yyyy-MM-dd");
        }

        public void Application(string path)
        {
            Append(String.Concat(GetDateTimeNow(), ";", path));
        }

        public void File(string applicationPath, string filePath)
        {
            Append(String.Concat(GetDateTimeNow(), ";", applicationPath, ";", filePath));
        }

        public void File(string applicationPath, string argumentsTemplate, string filePath)
        {
            Append(String.Concat(GetDateTimeNow(), ";", applicationPath, ";", argumentsTemplate, ";", filePath));
        }

        private IEnumerable<string[]> ReadLines()
        {
            IsolatedStorageFile storage = GetStorage();
            if (storage.FileExists(FileName))
            {
                using (IsolatedStorageFileStream stream = new IsolatedStorageFileStream(FileName, FileMode.Open, storage))
                using (StreamReader reader = new StreamReader(stream))
                {
                    string line = null;
                    while ((line = reader.ReadLine()) != null)
                    {
                        string[] parts = line.Split(new char[] { ';' }, StringSplitOptions.RemoveEmptyEntries);
                        if (parts.Length == 0)
                            continue;

                        yield return parts;
                    }
                }
            }
        }

        public IEnumerable<Month> Months()
        {
            HashSet<Month> result = new HashSet<Month>();

            foreach (string[] parts in ReadLines())
            {
                DateTime date;
                if (DateTime.TryParse(parts[0], out date))
                    result.Add(date);
            }

            return result;
        }

        public IEnumerable<ApplicationCountModel> Applications(Month monthFrom, Month monthTo)
        {
            Dictionary<string, int> result = new Dictionary<string, int>();

            foreach (string[] parts in ReadLines())
            {
                DateTime date;
                if (DateTime.TryParse(parts[0], out date))
                {
                    Month month = date;
                    if (monthFrom > month || month > monthTo)
                        continue;
                }

                string applicationPath = parts[1];
                int value;
                if (result.TryGetValue(applicationPath, out value))
                    result[applicationPath] = ++value;
                else
                    result[applicationPath] = 1;
            }

            return result.Select(item => new ApplicationCountModel(item.Key, item.Value));
        }

        public IEnumerable<FileCountModel> Files(Month monthFrom, Month monthTo)
        {
            Dictionary<string, int> result = new Dictionary<string, int>();

            foreach (string[] parts in ReadLines())
            {
                if (parts.Length > 2)
                {
                    DateTime date;
                    if (DateTime.TryParse(parts[0], out date))
                    {
                        Month month = date;
                        if (monthFrom > month || month > monthTo)
                            continue;
                    }

                    string filePath = parts[2];
                    if (parts.Length > 3)
                        filePath = parts[3];

                    int value;
                    if (result.TryGetValue(filePath, out value))
                        result[filePath] = ++value;
                    else
                        result[filePath] = 1;
                }
            }

            return result.Select(item => new FileCountModel(item.Key, item.Value));
        }
    }
}
