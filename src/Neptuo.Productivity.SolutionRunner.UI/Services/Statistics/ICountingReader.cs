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
        IEnumerable<Month> Months();

        IEnumerable<ApplicationCountModel> Applications(Month monthFrom, Month monthTo);
        IEnumerable<FileCountModel> Files(Month monthFrom, Month monthTo);
    }
}
