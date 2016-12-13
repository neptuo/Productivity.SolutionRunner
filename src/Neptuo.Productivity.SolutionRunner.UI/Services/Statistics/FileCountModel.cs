using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Neptuo.Productivity.SolutionRunner.Services.Statistics
{
    public class FileCountModel
    {
        public string Path { get; private set; }
        public int Count { get; private set; }

        public FileCountModel(string path, int count)
        {
            Ensure.NotNullOrEmpty(path, "path");
            Path = path;
            Count = count;
        }
    }
}
