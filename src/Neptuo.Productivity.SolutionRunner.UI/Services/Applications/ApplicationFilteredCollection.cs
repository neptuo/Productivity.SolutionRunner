using Neptuo.Productivity.SolutionRunner.Services.Configuration;
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
        private readonly ISettings settings;
        private readonly IApplicationCollection applications;

        public ApplicationFilteredCollection(ISettings settings, IApplicationCollection applications)
        {
            Ensure.NotNull(settings, "settings");
            Ensure.NotNull(applications, "applications");
            this.settings = settings;
            this.applications = applications;
        }

        public IApplicationBuilder Add(string name, string path, string arguments, bool isAdministratorRequired, ImageSource icon, Key hotKey, bool isMain)
        {
            if (isMain && settings.HiddenMainApplications.Contains(path))
                return new EmptyAplicationBuilder();

            return applications.Add(name, path, arguments, isAdministratorRequired, icon, hotKey, isMain);
        }

        public IApplicationBuilder Add(string name, Version version, string path, string arguments, bool isAdministratorRequired, ImageSource icon, Key hotKey, bool isMain)
        {
            if (isMain && settings.HiddenMainApplications.Contains(path))
                return new EmptyAplicationBuilder();

            return applications.Add(name, version, path, arguments, isAdministratorRequired, icon, hotKey, isMain);
        }
    }
}
