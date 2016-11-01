using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Neptuo.Productivity.SolutionRunner.Services.Execution
{
    public interface IApplication
    {
        string Path { get; }
        string Arguments { get; }
    }
}
