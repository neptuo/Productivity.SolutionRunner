using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Neptuo.Productivity.SolutionRunner.Services.Statistics
{
    public class ApplicationCountModel
    {
        public string Path { get; private set; }
        public int Count { get; private set; }

        public ApplicationCountModel(string path, int count)
        {
            Ensure.NotNullOrEmpty(path, "path");
            Path = path;
            Count = count;
        }
    }
}
