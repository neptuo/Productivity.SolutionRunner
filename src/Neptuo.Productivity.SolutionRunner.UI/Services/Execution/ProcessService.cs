using Neptuo.Collections.Specialized;
using Neptuo.Text.Tokens;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Neptuo.Productivity.SolutionRunner.Services.Execution
{
    public class ProcessService
    {
        public void Run(IApplication application, IFile file)
        {
            if (file == null)
            {
                Process.Start(application.Path);
            }
            else
            {
                string arguments = file.Path;
                if (!String.IsNullOrEmpty(application.Arguments))
                {
                    TokenWriter writer = new TokenWriter(application.Arguments);
                    arguments = writer.Format(new KeyValueCollection()
                        .Add("FilePath", file.Path)
                        .Add("DirectoryPath", System.IO.Path.GetDirectoryName(file.Path))
                    );
                }

                Process.Start(new ProcessStartInfo(application.Path, arguments));
            }
        }
    }
}
