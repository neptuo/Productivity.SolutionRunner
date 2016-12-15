using Neptuo;
using Neptuo.Collections.Specialized;
using Neptuo.Productivity.SolutionRunner.Services.Statistics;
using Neptuo.Productivity.SolutionRunner.ViewModels;
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
        private readonly ICountingAppender countingAppender;

        public ProcessService(ICountingAppender countingAppender)
        {
            Ensure.NotNull(countingAppender, "countingAppender");
            this.countingAppender = countingAppender;
        }

        public void Run(IApplication application, IFile file)
        {
            if (file == null)
            {
                countingAppender.Application(application.Path);
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

                    countingAppender.File(application.Path, application.Arguments, file.Path);
                }
                else
                {
                    countingAppender.File(application.Path, file.Path);
                }

                Process.Start(new ProcessStartInfo(application.Path, arguments));
                EventManager.RaiseProcessStarted(application, file);
            }
        }
    }
}
