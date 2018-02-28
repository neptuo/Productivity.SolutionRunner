using System;
using System.Collections.Generic;
using System.IO;
using System.IO.IsolatedStorage;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Neptuo.Productivity.SolutionRunner.Services.Searching
{
    public class FileCache
    {
        public const string FileName = "FileCache.dat";

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

        public void Add(string path)
        {
            Append(path);
        }

        public IEnumerable<string> Enumerate()
        {
            IsolatedStorageFile storage = GetStorage();
            if (storage.FileExists(FileName))
            {
                using (IsolatedStorageFileStream stream = new IsolatedStorageFileStream(FileName, FileMode.Open, storage))
                using (StreamReader reader = new StreamReader(stream))
                {
                    string line;
                    while ((line = reader.ReadLine()) != null)
                    {
                        if (File.Exists(line))
                            yield return line;
                    }
                }
            }
        }
    }
}
