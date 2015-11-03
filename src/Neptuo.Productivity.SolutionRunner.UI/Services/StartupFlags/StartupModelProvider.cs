using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Neptuo.Productivity.SolutionRunner.Services.StartupFlags
{
    public class StartupModelProvider
    {
        public StartupModel Get(string[] args)
        {
            return new StartupModel(args.Contains("--hidden"));
        }
    }
}
