using Neptuo;
using Neptuo.Collections.Specialized;
using Neptuo.Converters;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Neptuo.Productivity.SolutionRunner.Services.Configuration
{
    public class DefaultKeyValueCollection : IKeyValueCollection
    {
        public IEnumerable<string> Keys => settings.Properties.OfType<SettingsProperty>().Select(p => p.Name);

        private readonly IConverterRepository converters;
        private readonly SettingsBase settings;

        public DefaultKeyValueCollection(SettingsBase settings)
            : this(Converts.Repository, settings)
        { }

        public DefaultKeyValueCollection(IConverterRepository converters, SettingsBase settings)
        {
            Ensure.NotNull(converters, "converters");
            Ensure.NotNull(settings, "settings");
            this.converters = converters;
            this.settings = settings;
        }

        public IKeyValueCollection Add(string key, object value)
        {
            settings[key] = value;
            return this;
        }

        public bool TryGet<T>(string key, out T value)
        {
            object rawValue = settings[key];
            if (rawValue == null)
            {
                value = default(T);
                return false;
            }

            if (converters.TryConvert(rawValue, out value))
                return true;

            return false;
        }

        public void Save()
        {
            settings.Save();
        }
    }
}
