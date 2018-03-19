using System;
using System.Collections.Generic;
using System.IO;
using System.IO.IsolatedStorage;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Neptuo.Productivity.SolutionRunner.Services
{
    public class SequenceIsolatedFile
    {
        private readonly string fileName;
        private readonly object fileLock;

        public SequenceIsolatedFile(string fileName)
        {
            Ensure.NotNullOrEmpty(fileName, "fileName");
            this.fileName = fileName;
            this.fileLock = FileLockProvider.Get(fileName);
        }

        public bool IsEmpty
        {
            get
            {
                lock (fileLock)
                {
                    IsolatedStorageFile storage = GetStorage();
                    if (storage.FileExists(fileName))
                    {
                        using (IsolatedStorageFileStream stream = new IsolatedStorageFileStream(fileName, FileMode.Open, storage))
                            return stream.Length == 0;
                    }

                    return true;
                }
            }
        }

#if DEBUG
        public static IsolatedStorageFile GetStorage() => IsolatedStorageFile.GetUserStoreForAssembly();
#else
        public static IsolatedStorageFile GetStorage() => IsolatedStorageFile.GetUserStoreForApplication();
#endif

        public void Append(params string[] lines)
            => Append((IEnumerable<string>)lines);

        public void Append(IEnumerable<string> lines)
        {
            lock (fileLock)
            {

                IsolatedStorageFile storage = GetStorage();
                using (IsolatedStorageFileStream stream = new IsolatedStorageFileStream(fileName, FileMode.Append, storage))
                using (StreamWriter writer = new StreamWriter(stream))
                {
                    foreach (string line in lines)
                        writer.WriteLine(line);
                }
            }
        }

        public IEnumerable<string> Enumerate()
        {
            lock (fileLock)
            {
                IsolatedStorageFile storage = GetStorage();
                if (storage.FileExists(fileName))
                {
                    using (IsolatedStorageFileStream stream = new IsolatedStorageFileStream(fileName, FileMode.Open, storage))
                    using (StreamReader reader = new StreamReader(stream))
                    {
                        string line;
                        while ((line = reader.ReadLine()) != null)
                            yield return line;
                    }
                }
            }
        }

        public void Clear()
        {
            lock (fileLock)
            {
                IsolatedStorageFile storage = GetStorage();
                if (storage.FileExists(fileName))
                    storage.DeleteFile(fileName);
            }
        }

        public static IEnumerable<string> EnumerateNames(string pattern)
            => GetStorage().GetFileNames(pattern);

        public static bool Exists(string fileName)
            => GetStorage().FileExists(fileName);
    }
}
