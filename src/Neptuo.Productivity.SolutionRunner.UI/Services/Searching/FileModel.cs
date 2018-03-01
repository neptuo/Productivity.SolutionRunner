using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Neptuo.Productivity.SolutionRunner.Services.Searching
{
    public class FileModel : PatternMatcherFactory.IFileModel
    {
        public string Name { get; private set; }
        public string NameWithExtension { get; private set; }

        private string path;
        public string Path
        {
            get { return path; }
            set
            {
                path = value;
                Name = System.IO.Path.GetFileNameWithoutExtension(path);
                NameWithExtension = System.IO.Path.GetFileName(path);
            }
        }

        internal void Append(object path)
        {
            throw new NotImplementedException();
        }

        public FileModel(string path)
        {
            Path = path;
        }

        public override int GetHashCode()
        {
            return 23 ^ NameWithExtension.GetHashCode() ^ Path.GetHashCode();
        }

        public override bool Equals(object obj)
        {
            FileModel other = obj as FileModel;
            if (other == null)
                return false;

            if (Path.ToLowerInvariant() != other.Path.ToLowerInvariant())
                return false;

            return true;
        }
    }
}
