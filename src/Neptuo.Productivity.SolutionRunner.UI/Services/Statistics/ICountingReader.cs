using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Neptuo.Productivity.SolutionRunner.Services.Statistics
{
    /// <summary>
    /// An interface for loading counted data.
    /// </summary>
    public interface ICountingReader
    {
        IEnumerable<ApplicationCountModel> Applications();
        IEnumerable<FileCountModel> Files();
    }
}
