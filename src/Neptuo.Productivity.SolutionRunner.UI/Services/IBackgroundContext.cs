using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Neptuo.Productivity.SolutionRunner.Services
{
    /// <summary>
    /// A context for background worker process.
    /// </summary>
    public interface IBackgroundContext
    {
        /// <summary>
        /// Notifies that background work has started.
        /// When returned object is disposed, work is done.
        /// </summary>
        IDisposable Start();
    }
}
