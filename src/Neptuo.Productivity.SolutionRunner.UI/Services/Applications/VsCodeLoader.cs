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
            IDirectoryNameSearch search = new LocalSearchProvider(Environment.GetFolderPath(Environment.SpecialFolder.ProgramFilesX86));

            IDirectory directory = search
                .FindDirectories(TextSearch.CreateMatched("Microsoft VS Code"))
                .FirstOrDefault();

            if (directory == null)
                return;

            //IFile file = directory
            //    .WithFileNameSearch()
            //    .FindFiles(TextSearch.CreateMatched("Code", false), TextSearch.CreateMatched(".exe"))
            //    .FirstOrDefault();

            IFile file = directory
                .WithFileEnumerator()
                .FirstOrDefault(f => f.Name == "Code" && f.Extension == "exe");

            if (file == null)
                return;

            string filePath = file.WithAbsolutePath().AbsolutePath;
            FileVersionInfo version = FileVersionInfo.GetVersionInfo(filePath);

            IApplicationBuilder builder = applications.Add(
                String.Format("VS Code {0}.{1}.{2}", version.FileMajorPart, version.FileMinorPart, version.FileBuildPart),
                new Version(version.FileMajorPart, version.FileMinorPart),
                filePath,
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
                true,
                Key.A
            );
        }
    }
}
