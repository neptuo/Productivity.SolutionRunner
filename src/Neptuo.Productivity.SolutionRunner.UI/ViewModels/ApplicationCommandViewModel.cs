using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace Neptuo.Productivity.SolutionRunner.ViewModels
{
    public class ApplicationCommandViewModel
    {
        public string Name { get; private set; }
        public string Path { get; private set; }
        public string Arguments { get; private set; }
        public Key HotKey { get; private set; }

        public ApplicationCommandViewModel(string name, string path, string arguments, Key hotKey)
        {
            Ensure.NotNullOrEmpty(name, "name");
            Ensure.NotNull(path, "path");
            Name = name;
            Path = path;
            Arguments = arguments;
            HotKey = hotKey;
        }
    }
}
