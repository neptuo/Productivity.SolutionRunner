using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using Windows.ApplicationModel;

namespace Neptuo.Productivity.SolutionRunner.Services
{
    public class ApplicationVersion
    {
        public string GetDisplayString()
        {
            if (Uwp.Is())
                return GetVersionFromPackage();

            return GetVersionFromAssemblyAttribute();
        }

        private string GetVersionFromPackage()
        {
            var version = Package.Current.Id.Version;
            string versionText = $"v{version.Major}.{version.Minor}.{version.Revision}";
            if (version.Build > 0)
                versionText += $".{version.Build}";

            return versionText;
        }

        private static string GetVersionFromAssemblyAttribute()
        {
            string version = Assembly.GetExecutingAssembly().GetCustomAttribute<AssemblyInformationalVersionAttribute>().InformationalVersion;
            return String.Format("v{0}", version);
        }
    }
}
