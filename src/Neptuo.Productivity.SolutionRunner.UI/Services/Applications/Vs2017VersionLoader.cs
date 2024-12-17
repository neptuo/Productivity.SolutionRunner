using Microsoft.VisualStudio.Setup.Configuration;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace Neptuo.Productivity.SolutionRunner.Services.Applications
{
    public class Vs2017VersionLoader : IApplicationLoader
    {
        public void Add(IApplicationCollection applications)
        {
            foreach (var (name, installationPath, filePath, version) in Enumerate().OrderBy(vs => vs.Name))
            {
                IApplicationBuilder builder = applications.Add(
                    name,
                    version,
                    filePath,
                    null,
                    null,
                    false,
                    true,
                    IconExtractor.Get(filePath),
                    Key.None,
                    true
                );

                VsVersionLoader.AddAdministratorCommand(builder, filePath);
                VsVersionLoader.AddExperimentalCommand(builder, filePath);
                VsInstallerCommandLoader.AddCommand(builder, installationPath);
            }
        }

        private IEnumerable<VsInstallation> Enumerate()
        {
            var result = new List<VsInstallation>();
            try
            {
                var query = new SetupConfiguration();
                var query2 = (ISetupConfiguration2)query;
                var e = query2.EnumAllInstances();

                var helper = (ISetupHelper)query;

                int fetched;
                var instances = new ISetupInstance[1];
                do
                {
                    e.Next(1, instances, out fetched);
                    if (fetched > 0)
                    {
                        if (TryGetInstance(instances[0], helper, out var instance))
                            result.Add(instance);
                    }
                }
                while (fetched > 0);
            }
            catch (COMException)
            {
                // Noop
            }

            return result;
        }

        private bool TryGetInstance(ISetupInstance instance, ISetupHelper helper, out VsInstallation output)
        {
            string name = "Visual Studio";
            string filePath = null;
            string installationPath = null;
            bool isPreRelease = false;
            Version version = null;

            var instance2 = (ISetupInstance2)instance;
            var state = instance2.GetState();

            if (state.HasFlag(InstanceState.Local | InstanceState.NoErrors))
            {
                Version.TryParse(instance.GetInstallationVersion(), out version);

                if ((state & InstanceState.Local) == InstanceState.Local)
                {
                    installationPath = instance2.GetInstallationPath();
                    filePath = Path.Combine(installationPath, @"Common7\IDE\devenv.exe");
                }

                if (instance is ISetupInstanceCatalog catalog)
                {
                    isPreRelease = catalog.IsPrerelease();

                    var properties = EnumerateProperties(catalog);
                    foreach (var property in properties)
                    {
                        if (property.name == "productDisplayVersion")
                        {
                            if (Version.TryParse(property.value, out Version productVersion))
                                version = productVersion;
                        }
                        else if (property.name == "productName")
                        {
                            name = property.value;
                        }
                    }
                }
            }

            if (name != null && filePath != null && File.Exists(filePath) && version != null)
            {
                name = String.Format(
                    "{0} {1}",
                    name,
                    VersionFormatter.Format(version)
                );

                if (isPreRelease)
                    name += " Prev";

                output = new VsInstallation(name, installationPath, filePath, version);
                return true;
            }

            output = null;
            return false;
        }

        private IEnumerable<(string name, string value)> EnumerateProperties(ISetupInstanceCatalog catalog)
        {
            var properties = catalog?.GetCatalogInfo();
            if (properties != null)
            {
                foreach (string name in properties.GetNames())
                {
                    object value = properties.GetValue(name);
                    yield return (name, value.ToString());
                }
            }
        }

        record VsInstallation(string Name, string InstallationPath, string FilePath, Version Version);
    }
}
