using Neptuo.Productivity.SolutionRunner.Services.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Neptuo.Productivity.SolutionRunner.Views.DesignData
{
    public class DesignLogProvider : ILogService
    {
        public string FindFileContent(string fileName)
        {
            return String.Empty;
        }

        public IEnumerable<LogModel> GetFileNames()
        {
            yield return new LogModel(
                "ErrorLog",
                new List<string>()
                {
                    "ErrorLog_2017-12-24.log",
                    "ErrorLog_2018-02-01.log",
                    "ErrorLog_2018-03-04.log",
                    "ErrorLog_2018-03-06.log"
                }
            );
        }
    }
}
