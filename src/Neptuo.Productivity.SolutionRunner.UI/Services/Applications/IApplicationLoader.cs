using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Neptuo.Productivity.SolutionRunner.Services.Applications
{
    public interface IApplicationLoader
    {
        void Add(IApplicationCollection applications);
    }
}
