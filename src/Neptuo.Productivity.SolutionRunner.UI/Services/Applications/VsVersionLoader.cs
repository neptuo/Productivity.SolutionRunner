using Neptuo.FileSystems;
using Neptuo.FileSystems.Features;
using Neptuo.FileSystems.Features.Searching;
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
        public void Add(IApplicationCollection applications)
        {
            IDirectoryNameSearch search = new LocalSearchProvider(Environment.GetFolderPath(Environment.SpecialFolder.ProgramFilesX86));
            Add2015AndOlder(search, applications);
            //Add2017AndNewer(search, applications);
        }

        private void Add2017AndNewer(IDirectoryNameSearch search, IApplicationCollection applications)
        {
            IDirectory root = search
                .FindDirectories(TextSearch.CreateMatched("Microsoft Visual Studio"))
                .FirstOrDefault();

            if (root == null)
                return;

            foreach (IDirectory version in root.WithDirectoryEnumerator())
            {
                if (version.Name.Length == 4)
                {
                    foreach (IDirectory edition in version.WithDirectoryEnumerator())
                        TryAdd(applications, edition);
                }
            }
        }

        private void Add2015AndOlder(IDirectoryNameSearch search, IApplicationCollection applications)
        {
            IEnumerable<IDirectory> directories = search.FindDirectories(TextSearch.CreatePrefixed("Microsoft Visual Studio"));

            foreach (IDirectory directory in directories)
                TryAdd(applications, directory);
        }

        private void TryAdd(IApplicationCollection applications, IDirectory directory)
        {
            string filePath = Path.Combine(directory.WithAbsolutePath().AbsolutePath, @"Common7\IDE\devenv.exe");
            TryAdd(applications, filePath);
        }

        private void TryAdd(IApplicationCollection applications, string filePath)
        {
            if (File.Exists(filePath))
            {
                FileVersionInfo version = FileVersionInfo.GetVersionInfo(filePath);

                IApplicationBuilder builder = applications.Add(
                    String.Format("Visual Studio {0}", VersionFormatter.Format(version)),
                    new Version(version.FileMajorPart, version.FileMinorPart, version.FileBuildPart),
                    filePath,
                    null,
                    false,
                    IconExtractor.Get(filePath),
                    Key.None,
                    true
                );

                builder.AddCommand(
                    "Run as Administrator",
                    filePath,
                    null,
                    true,
                    Key.A
                );
            }
        }
    }
}
