using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Neptuo.Productivity.SolutionRunner.Services.Logging
{
    public class LogModel
    {
        public string Name { get; }
        public IReadOnlyCollection<string> FileNames { get; }

        public LogModel(string name, IReadOnlyCollection<string> fileNames)
        {
            Ensure.NotNullOrEmpty(name, "name");
            Ensure.NotNull(fileNames, "fileNames");
            Name = name;
            FileNames = fileNames;
        }
    }
}
