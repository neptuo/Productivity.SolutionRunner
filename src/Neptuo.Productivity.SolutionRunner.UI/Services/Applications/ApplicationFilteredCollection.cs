using Neptuo;
using Neptuo.Productivity.SolutionRunner.Properties;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using System.Windows.Media;

namespace Neptuo.Productivity.SolutionRunner.Services.Applications
{
    /// <summary>
    /// An implementation of <see cref="IApplicationCollection"/> that filters applications by <see cref="Settings.HiddenMainApplications"/>.
    /// </summary>
    internal class ApplicationFilteredCollection : IApplicationCollection
    {
        private readonly Settings settings;
        private readonly IApplicationCollection applications;

        public ApplicationFilteredCollection(Settings settings, IApplicationCollection applications)
        {
            Ensure.NotNull(settings, "settings");
            Ensure.NotNull(applications, "applications");
            this.settings = settings;
            this.applications = applications;
        }

        public IApplicationCollection Add(string name, string path, string arguments, ImageSource icon, Key hotKey, bool isMain)
        {
            if (isMain && settings.GetHiddenMainApplications().Contains(path))
                return this;

            applications.Add(name, path, arguments, icon, hotKey, isMain);
            return this;
        }

        public IApplicationCollection Add(string name, Version version, string path, string arguments, ImageSource icon, Key hotKey, bool isMain)
        {
            if (isMain && settings.GetHiddenMainApplications().Contains(path))
                return this;

            applications.Add(name, version, path, arguments, icon, hotKey, isMain);
            return this;
        }
    }
}
