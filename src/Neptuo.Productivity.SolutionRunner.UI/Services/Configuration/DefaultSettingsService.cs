using Neptuo.Collections.Specialized;
using Neptuo.Productivity.SolutionRunner.Properties;
using Neptuo.Threading.Tasks;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Neptuo.Productivity.SolutionRunner.Services.Configuration
{
    public class DefaultSettingsService : ISettingsService
    {
        public Task<ISettings> LoadAsync()
        {
            ISettings settings = Settings.Default;
            return Task.FromResult(settings);
        }

        public Task<IKeyValueCollection> LoadRawAsync()
        {
            IKeyValueCollection settings = new DefaultKeyValueCollection(Settings.Default);
            return Task.FromResult(settings);
        }

        public Task SaveAsync(ISettings settings)
        {
            Settings target = (Settings)settings;
            target.Save();

            return Async.CompletedTask;
        }
    }
}
