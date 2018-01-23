using Neptuo.Collections.Specialized;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Neptuo.Productivity.SolutionRunner.Services.Configuration
{
    public interface ISettingsService
    {
        Task<ISettings> LoadAsync();
        Task<IKeyValueCollection> LoadRawAsync();
        Task SaveAsync(ISettings settings);
        Task SaveRawAsync(IKeyValueCollection settings);
    }
}
