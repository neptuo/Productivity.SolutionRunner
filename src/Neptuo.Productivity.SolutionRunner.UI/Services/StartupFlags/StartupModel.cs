using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Neptuo.Productivity.SolutionRunner.Services.StartupFlags
{
    public class StartupModel
    {
        public bool IsStartup { get; set; }
        public bool IsHidden { get; set; }

        public StartupModel(bool isHidden)
        {
            IsHidden = isHidden;
        }
    }
}
