using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Neptuo.Productivity.SolutionRunner.Services.Statistics
{
    /// <summary>
    /// Provides methods for appending statistics.
    /// </summary>
    public interface ICountingAppender
    {
        /// <summary>
        /// Appends application usage.
        /// </summary>
        /// <param name="path">A path to the application being started.</param>
        void Application(string path);

        /// <summary>
        /// Appends opened file in application.
        /// </summary>
        /// <param name="applicationPath">A path to the application being started.</param>
        /// <param name="filePath">A path to the file being opened.</param>
        void File(string applicationPath, string filePath);

        /// <summary>
        /// Appends opened file in application.
        /// </summary>
        /// <param name="applicationPath">A path to the application being started.</param>
        /// <param name="argumentsTemplate">A template of arguments passed to the application.</param>
        /// <param name="filePath">A path to the file being opened.</param>
        void File(string applicationPath, string argumentsTemplate, string filePath);
    }
}
