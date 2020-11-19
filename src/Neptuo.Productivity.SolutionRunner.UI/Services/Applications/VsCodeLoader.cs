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
            List<(string suffix, string file)> files = new List<(string suffix, string file)>(3);
            foreach (var search in EnumerateProgramFolders())
            {
                string filePath = Path.Combine(search.directory, "Microsoft VS Code", "Code.exe");
                if (File.Exists(filePath))
                    files.Add((search.suffix, filePath));
            }

            foreach (var search in files)
            {
                FileVersionInfo version = FileVersionInfo.GetVersionInfo(search.file);

                IApplicationBuilder builder = applications.Add(
                    String.Format(
                        "VS Code{0} {1}",
                        files.Count == 1
                            ? String.Empty
                            : " " + search.suffix,
                        VersionFormatter.Format(version)
                    ),
                    new Version(version.FileMajorPart, version.FileMinorPart),
                    search.file,
                    null,
                    "{DirectoryPath}",
                    false,
                    true,
                    IconExtractor.Get(search.file),
                    Key.None,
                    true
                );

                builder.AddCommand(
                    "Run as Administrator",
                    search.file,
                    null,
                    null,
                    true,
                    true,
                    Key.A
                );
            }
        }

        private IEnumerable<(string suffix, string directory)> EnumerateProgramFolders()
        {
            string x86 = Environment.GetFolderPath(Environment.SpecialFolder.ProgramFilesX86);
            string x64 = Environment.GetFolderPath(Environment.SpecialFolder.ProgramFiles);

            if (Directory.Exists(x86))
                yield return ("x86", x86);

            if (x64 != x86 && Directory.Exists(x64))
                yield return ("x64", x64);

            string user = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "Programs");
            if (Directory.Exists(user))
                yield return ("user", user);
        }
    }
}
