using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Neptuo.Productivity.SolutionRunner.Services.Logging
{
    public class IsolatedLogService : ILogService
    {
        public IEnumerable<LogModel> GetFileNames()
        {
            Dictionary<string, List<string>> result = new Dictionary<string, List<string>>();
            foreach (string fileName in SequenceIsolatedFile.EnumerateNames("*.log"))
            {
                string[] parts = fileName.Split(new char[] { '_' }, StringSplitOptions.RemoveEmptyEntries);
                string name = parts[0];
                if (!result.TryGetValue(name, out List<string> items))
                    result[name] = items = new List<string>();

                items.Add(fileName);
            }

            return result.Select(i => new LogModel(i.Key, i.Value));
        }

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
