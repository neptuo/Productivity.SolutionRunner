using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Neptuo.Productivity.SolutionRunner.Services
{
    public interface IPinStateService
    {
        IEnumerable<string> GetList();
        bool IsPinned(string path);
    }
}
