using Neptuo;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Neptuo.Productivity.SolutionRunner.Services.Applications
{
    public class VersionFormatter
    {
        public static string Format(Version version)
        {
            Ensure.NotNull(version, "version");
            return String.Format(
                "{0}.{1}",
                version.Major,
                version.Minor
            );
        }

        public static string Format(FileVersionInfo version)
        {
            Ensure.NotNull(version, "version");
            return String.Format(
                "{0}.{1}",
                version.FileMajorPart,
                version.FileMinorPart
            );
        }
    }
}
