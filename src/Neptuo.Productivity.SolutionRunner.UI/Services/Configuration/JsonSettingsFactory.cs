using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Neptuo.Productivity.SolutionRunner.Services.Configuration
{
    public class JsonSettingsFactory : ISettingsFactory
    {
        public ISettingsService CreateForFile(string filePath)
        {
            return new JsonSettingsService(() => filePath);
        }
    }
}
