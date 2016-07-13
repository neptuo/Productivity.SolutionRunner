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
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace Neptuo.Productivity.SolutionRunner.Services.Applications
{
    public class VsVersionLoader
    {
        public void Add(IApplicationCollection applications)
        {
            IDirectoryNameSearch search = new LocalSearchProvider(Environment.GetFolderPath(Environment.SpecialFolder.ProgramFilesX86));
            IEnumerable<IDirectory> directories = search.FindDirectories(TextSearch.CreatePrefixed("Microsoft Visual Studio"));

            foreach (IDirectory directory in directories)
            {
                string filePath = Path.Combine(directory.WithAbsolutePath().AbsolutePath, @"Common7\IDE\devenv.exe");
                if (File.Exists(filePath))
                {
                    FileVersionInfo version = FileVersionInfo.GetVersionInfo(filePath);

                    applications.Add(
                        String.Format("Visual Studio {0}.{1}", version.FileMajorPart, version.FileMinorPart),
                        filePath,
                        null,
                        IconExtractor.Get(filePath),
                        true
                    );
                }
            }
        }
    }
}
