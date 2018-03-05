using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Neptuo.Productivity.SolutionRunner.Services.Searching
{
    public class FileStorage : IEnumerable<FileModel>
    {
        private readonly HashSet<FileModel> storage = new HashSet<FileModel>();
        private readonly FileCache cache = new FileCache();

        private HashSet<FileModel> cacheStorage;
        private readonly object cacheStorageLock = new object();

        public bool IsCacheUsed { get; set; }

        public bool IsCacheEmpty => cache.IsEmpty;

        public FileStorage Add(FileModel file)
        {
            storage.Add(file);
            cache.Add(file);

            return this;
        }

        public FileStorage AddRange(IEnumerable<FileModel> files)
        {
            foreach (FileModel file in files)
                storage.Add(file);

            cache.AddRange(files);

            return this;
        }

        public FileStorage Remove(FileModel file)
        {
            if (cacheStorage != null)
                cacheStorage.Remove(file);

            storage.Remove(file);

            return this;
        }

        public IEnumerator<FileModel> GetEnumerator()
        {
            if (IsCacheUsed)
            {
                if (cacheStorage == null)
                {
                    lock (cacheStorageLock)
                    {
                        if (cacheStorage == null)
                            cacheStorage = new HashSet<FileModel>(cache.Enumerate());
                    }
                }

                return cacheStorage.GetEnumerator();
            }

            return storage.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
    }
}
