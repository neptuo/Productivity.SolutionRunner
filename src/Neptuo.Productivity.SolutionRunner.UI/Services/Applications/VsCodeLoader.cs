using Neptuo.FileSystems;
using Neptuo.FileSystems.Features;
using Neptuo.FileSystems.Features.Searching;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace Neptuo.Productivity.SolutionRunner.Services.Applications
{
    public class VsCodeLoader : IApplicationLoader
    {
        public void Add(IApplicationCollection applications)
        {
            List<(string suffix, IFile file)> files = new List<(string suffix, IFile file)>();
            foreach (var search in EnumerateProgramFolders())
            {
                IDirectory directory = search
                    .directory
                    .FindDirectories(TextSearch.CreateMatched("Microsoft VS Code"))
                    .FirstOrDefault();

                if (directory == null)
                    continue;

                IFile file = directory
                    .WithFileEnumerator()
                    .FirstOrDefault(f => f.Name == "Code" && f.Extension == "exe");

                if (file == null)
                    continue;

                files.Add((search.suffix, file));
            }

            if (files.Count == 0)
                return;

            foreach (var search in files)
            {
                //IFile file = directory
                //    .WithFileNameSearch()
                //    .FindFiles(TextSearch.CreateMatched("Code", false), TextSearch.CreateMatched(".exe"))
                //    .FirstOrDefault();

                string filePath = search.file.WithAbsolutePath().AbsolutePath;
                FileVersionInfo version = FileVersionInfo.GetVersionInfo(filePath);

                IApplicationBuilder builder = applications.Add(
                    String.Format(
                        "VS Code{0} {1}",
                        files.Count == 1
                            ? String.Empty
                            : " " + search.suffix,
                        VersionFormatter.Format(version)
                    ),
                    new Version(version.FileMajorPart, version.FileMinorPart),
                    filePath,
                    null,
                    "{DirectoryPath}",
                    false,
                    IconExtractor.Get(filePath),
                    Key.None,
                    true
                );

                builder.AddCommand(
                    "Run as Administrator",
                    filePath,
                    null,
                    null,
                    true,
                    Key.A
                );
            }
        }

        private IEnumerable<(string suffix, IDirectoryNameSearch directory)> EnumerateProgramFolders()
        {
            string x86 = Environment.GetFolderPath(Environment.SpecialFolder.ProgramFilesX86);
            string x64 = x86.Replace(" (x86)", String.Empty);

            if (Directory.Exists(x86))
                yield return ("x86", new LocalSearchProvider(x86));

            if (x64 != x86 && Directory.Exists(x64))
                yield return ("x64", new LocalSearchProvider(x64));
        }
    }
}
