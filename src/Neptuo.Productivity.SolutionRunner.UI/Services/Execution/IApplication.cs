using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Neptuo.Productivity.SolutionRunner.Services.Execution
{
    /// <summary>
    /// A runnable application description.
    /// </summary>
    public interface IApplication
    {
        /// <summary>
        /// Gets a path to the execution.
        /// </summary>
        string Path { get; }

        /// <summary>
        /// Gets an optional arguments for starting the application WITHOUT file.
        /// </summary>
        string EmptyArguments { get; }

        /// <summary>
        /// Gets an optional arguments for starting the application WITH file.
        /// </summary>
        string FileArguments { get; }

        /// <summary>
        /// Gets a <c>true</c> if administrator priviledge is required; <c>false</c> otherwise.
        /// </summary>
        bool IsAdministratorRequired { get; }
    }
}
