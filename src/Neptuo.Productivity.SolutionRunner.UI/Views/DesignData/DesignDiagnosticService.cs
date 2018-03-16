using Neptuo.Productivity.SolutionRunner.Services.Searching;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Neptuo.Productivity.SolutionRunner.Views.DesignData
{
    public class DesignDiagnosticService : IDiagnosticService
    {
        public bool IsAvailable => true;

        public IEnumerable<string> EnumerateFiles()
        {
            yield return @"C:\Development\Project1.sln";
            yield return @"C:\Development\Project2.sln";
            yield return @"C:\Development\Slider.sln";
        }
    }
}
