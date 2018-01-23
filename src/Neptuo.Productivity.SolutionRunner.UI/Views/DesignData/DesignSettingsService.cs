using Neptuo.Collections.Specialized;
using Neptuo.Productivity.SolutionRunner.Properties;
using Neptuo.Productivity.SolutionRunner.Services.Configuration;
using Neptuo.Threading.Tasks;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Neptuo.Productivity.SolutionRunner.Views.DesignData
{
    internal class DesignSettingsService : ISettingsService
    {
        public Task<ISettings> LoadAsync()
        {
            ISettings settings = new Settings();
            return Task.FromResult(settings);
        }

        public Task<IKeyValueCollection> LoadRawAsync()
        {
            return Task.FromResult<IKeyValueCollection>(new KeyValueCollection());
        }

        public Task SaveAsync(ISettings settings)
        {
            return Async.CompletedTask;
        }

        public Task SaveRawAsync(IKeyValueCollection settings)
        {
            return Async.CompletedTask;
        }
    }
}
