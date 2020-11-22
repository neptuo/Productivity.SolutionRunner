using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Neptuo.Productivity.SolutionRunner.Services.StartupShortcuts
{
    public interface IAutoStartup
    {
        Task<bool> IsEnabledAsync();
        Task<bool> EnableAsync();
        Task<bool> DisableAsync();
    }
}
