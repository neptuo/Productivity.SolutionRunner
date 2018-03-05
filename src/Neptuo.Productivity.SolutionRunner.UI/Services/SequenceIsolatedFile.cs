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
        private string fileName;

        public SequenceIsolatedFile(string fileName)
        {
            Ensure.NotNullOrEmpty(fileName, "fileName");
            this.fileName = fileName;
        }

        public bool IsEmpty
        {
            get
            {
                IsolatedStorageFile storage = GetStorage();
                if (storage.FileExists(fileName))
                {
                    using (IsolatedStorageFileStream stream = new IsolatedStorageFileStream(fileName, FileMode.Open, storage))
                        return stream.Length > 0;
                }

                return false;
            }
        }

#if DEBUG
        private IsolatedStorageFile GetStorage() => IsolatedStorageFile.GetUserStoreForAssembly();
#else
        private IsolatedStorageFile GetStorage() => IsolatedStorageFile.GetUserStoreForApplication();
#endif

        public void Append(params string[] lines)
            => Append((IEnumerable<string>)lines);

        public void Append(IEnumerable<string> lines)
        {
            IsolatedStorageFile storage = GetStorage();
            using (IsolatedStorageFileStream stream = new IsolatedStorageFileStream(fileName, FileMode.Append, storage))
            using (StreamWriter writer = new StreamWriter(stream))
            {
                foreach (string line in lines)
                    writer.WriteLine(line);
            }
        }

        public IEnumerable<string> Enumerate()
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

        public void Clear()
        {
            IsolatedStorageFile storage = GetStorage();
            if (storage.FileExists(fileName))
                storage.DeleteFile(fileName);
        }
    }
}
