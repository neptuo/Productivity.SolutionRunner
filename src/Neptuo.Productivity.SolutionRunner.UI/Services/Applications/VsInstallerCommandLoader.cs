using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace Neptuo.Productivity.SolutionRunner.Services.Applications
{
    public static class VsInstallerCommandLoader
    {
        private static bool? hasInstaller;
        private static string installerPath;

        public static void AddCommand(IApplicationBuilder application, string installationPath)
        {
            if (EnsureInstallerPath())
            {
                string args = $"-- modify --installPath \"{installationPath}\"";
                application.AddCommand("Installer", installerPath, args, args, true, true, Key.I);
            }
        }

        private static bool EnsureInstallerPath()
        {
            if (hasInstaller == null)
            {
                string path = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ProgramFilesX86), "Microsoft Visual Studio", "Installer", "vs_installer.exe");
                if (File.Exists(path))
                {
                    hasInstaller = true;
                    installerPath = path;
                }
                else
                {
                    hasInstaller = false;
                }
            }

            return hasInstaller.Value;
        }
    }
}
