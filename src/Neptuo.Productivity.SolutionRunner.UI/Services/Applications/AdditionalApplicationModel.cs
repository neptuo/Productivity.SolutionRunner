using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Neptuo.Productivity.SolutionRunner.Services.Applications
{
    public class AdditionalApplicationModel
    {
        public string Name { get; private set; }
        public string Path { get; private set; }
        public string Arguments { get; private set; }

        public AdditionalApplicationModel(string name, string path, string arguments)
        {
            Name = name;
            Path = path;
            Arguments = arguments;
        }
    }
}
