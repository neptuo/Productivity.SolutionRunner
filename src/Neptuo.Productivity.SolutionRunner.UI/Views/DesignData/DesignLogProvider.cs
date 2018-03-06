using Neptuo.Productivity.SolutionRunner.Services.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Neptuo.Productivity.SolutionRunner.Views.DesignData
{
    public class DesignLogProvider : ILogProvider
    {
        public string FindFileContent(string fileName)
        {
            return String.Empty;
        }

        public IEnumerable<string> GetFileNames()
        {
            yield return "ErrorLog_2017-12-24.log";
            yield return "ErrorLog_2018-02-01.log";
            yield return "ErrorLog_2018-03-04.log";
            yield return "ErrorLog_2018-03-06.log";
        }
    }
}
