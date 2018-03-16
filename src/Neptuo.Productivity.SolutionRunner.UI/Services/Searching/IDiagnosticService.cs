using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Neptuo.Productivity.SolutionRunner.Services.Searching
{
    /// <summary>
    /// A searching problem solving service.
    /// </summary>
    public interface IDiagnosticService
    {
        /// <summary>
        /// Whether the diagnostics are available.
        /// </summary>
        bool IsAvailable { get; }

        /// <summary>
        /// Enumerates currently available files.
        /// </summary>
        /// <returns>Enumeration currently available files.</returns>
        IEnumerable<string> EnumerateFiles();
    }
}
