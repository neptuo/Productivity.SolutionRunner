using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Neptuo.Productivity.SolutionRunner.ViewModels
{
    public class VersionViewModel
    {
        public string Name { get; private set; }
        public Version Model { get; private set; }

        public VersionViewModel(Version version)
            : this(version.ToString(), version)
        {

        }

        private VersionViewModel(string name, Version version)
        {
            Name = name;
            Model = version;
        }

        public static VersionViewModel Empty()
        {
            return new VersionViewModel("---", null);
        }
    }
}
