using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace Neptuo.Productivity.SolutionRunner.Services.Applications
{
    public class VsVersionLoader : IApplicationLoader
    {
        public static void AddAdministratorCommand(IApplicationBuilder builder, string filePath)
        {
            builder.AddCommand(
                "Run as Administrator",
                filePath,
                null,
                null,
                true,
                true,
                Key.A
            );
        }

        public static void AddExperimentalCommand(IApplicationBuilder builder, string filePath)
        {
            builder.AddCommand(
                "Run experimental",
                filePath,
                "/rootsuffix Exp",
                "/rootsuffix Exp {FilePath}",
                false,
                true,
                Key.E
            );
        }

        public void Add(IApplicationCollection applications)
        {
            string rootPath = Environment.GetFolderPath(Environment.SpecialFolder.ProgramFilesX86);
            Add2015AndOlder(rootPath, applications);
        }

        private void Add2015AndOlder(string rootPath, IApplicationCollection applications)
        {
            IEnumerable<string> directories = Directory.EnumerateDirectories(rootPath, "Microsoft Visual Studio*");

            foreach (string directory in directories)
                TryAddDirectory(applications, directory);
        }

        private void TryAddDirectory(IApplicationCollection applications, string directory)
        {
            string filePath = Path.Combine(directory, @"Common7\IDE\devenv.exe");
            TryAddApplication(applications, filePath);
        }

        private void TryAddApplication(IApplicationCollection applications, string filePath)
        {
            if (File.Exists(filePath))
            {
                FileVersionInfo version = FileVersionInfo.GetVersionInfo(filePath);

                IApplicationBuilder builder = applications.Add(
                    String.Format("Visual Studio {0}", VersionFormatter.Format(version)),
                    new Version(version.FileMajorPart, version.FileMinorPart, version.FileBuildPart),
                    filePath,
                    null,
                    null,
                    false,
                    true,
                    IconExtractor.Get(filePath),
                    Key.None,
                    true
                );

                AddAdministratorCommand(builder, filePath);
                AddExperimentalCommand(builder, filePath);
            }
        }
    }
}
