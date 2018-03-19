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

        private readonly SequenceIsolatedFile storage = new SequenceIsolatedFile(FileName);
        private HashSet<FileModel> files;

        public bool IsEmpty => storage.IsEmpty;

        public void Add(FileModel file)
        {
            if (files == null || files.Add(file))
                storage.Append(file.Path);
        }

        public void AddRange(IEnumerable<FileModel> files)
        {
            if (this.files != null)
            {
                storage.Append(files.Where(f => this.files.Add(f)).Select(f => f.Path));
            }
            else if (storage.IsEmpty)
            {
                this.files = new HashSet<FileModel>();
                foreach (FileModel file in files)
                    this.files.Add(file);

                storage.Append(files.Select(f => f.Path));
            }
        }

        public IEnumerable<FileModel> Enumerate()
        {
            if (files == null)
            {
                files = new HashSet<FileModel>();

                foreach (string path in storage.Enumerate())
                {
                    if (File.Exists(path))
                        files.Add(new FileModel(path));
                }

                storage.Clear();
                storage.Append(files.Select(f => f.Path));
            }

            return files;
        }

        public static void Clear() 
            => new SequenceIsolatedFile(FileName).Clear();
    }
}
