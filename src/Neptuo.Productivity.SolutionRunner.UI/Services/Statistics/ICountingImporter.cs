using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Neptuo.Productivity.SolutionRunner.Services.Statistics
{
    public interface ICountingImporter
    {
        Task ImportAsync(Stream data);
        Task ExportAsync(Stream data);
    }
}
