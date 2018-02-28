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

        public bool IsCacheUsed { get; set; }

        public FileStorage Add(FileModel file, bool isCacheIncluded = true)
        {
            storage.Add(file);

            if (isCacheIncluded)
                cache.Add(file.Path);

            return this;
        }

        public FileStorage AddRange(IEnumerable<FileModel> files, bool isCacheIncluded = true)
        {
            foreach (FileModel file in files)
                Add(file, isCacheIncluded);

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
                    cacheStorage = new HashSet<FileModel>(cache.Enumerate().Select(p => new FileModel(p)));

                return cacheStorage.GetEnumerator();
            }

            return storage.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
    }
}
