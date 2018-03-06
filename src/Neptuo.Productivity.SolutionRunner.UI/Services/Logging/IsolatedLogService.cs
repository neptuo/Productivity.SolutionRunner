using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Neptuo.Productivity.SolutionRunner.Services.Logging
{
    public class IsolatedLogService : ILogService
    {
        // TODO: Group by root ScopeName

        public IEnumerable<string> GetFileNames() => SequenceIsolatedFile.EnumerateNames("*.log");

        public string FindFileContent(string fileName)
        {
            Ensure.NotNullOrEmpty(fileName, "fileName");
            if (fileName.EndsWith(".log") && SequenceIsolatedFile.Exists(fileName))
            {
                SequenceIsolatedFile file = new SequenceIsolatedFile(fileName);
                return String.Join(Environment.NewLine, file.Enumerate());
            }

            return null;
        }
    }
}
