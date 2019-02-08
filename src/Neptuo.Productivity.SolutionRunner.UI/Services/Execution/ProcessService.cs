using Neptuo.Collections.Specialized;
using Neptuo.Productivity.SolutionRunner.Services.Statistics;
using Neptuo.Productivity.SolutionRunner.ViewModels;
using Neptuo.Text.Tokens;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
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

        public void Run(IApplication application, IFile file, bool isAdministratorRequired = false)
        {
            string arguments = null;

            if (file == null)
            {
                if (!String.IsNullOrEmpty(application.EmptyArguments))
                {
                    arguments = application.EmptyArguments;
                    countingAppender.Application(application.Path, arguments);
                }
                else
                {
                    countingAppender.Application(application.Path);
                }
            }
            else
            {
                if (!String.IsNullOrEmpty(application.FileArguments))
                {
                    TokenWriter writer = new TokenWriter(application.FileArguments);
                    arguments = writer.Format(new KeyValueCollection()
                        .Add("FilePath", file.Path)
                        .Add("DirectoryPath", Path.GetDirectoryName(file.Path))
                    );

                    countingAppender.File(application.Path, application.FileArguments, file.Path);
                }
                else
                {
                    arguments = file.Path;
                    countingAppender.File(application.Path, file.Path);
                }
            }

            ProcessStartInfo info = arguments == null ? new ProcessStartInfo(application.Path) : new ProcessStartInfo(application.Path, arguments);
            if (isAdministratorRequired || application.IsAdministratorRequired)
                info.Verb = "runas";

            try
            {
                Process.Start(info);
                EventManager.RaiseProcessStarted(application, file);
            }
            catch (Win32Exception e)
            {
                // "The operation was canceled by the user".
                if (e.NativeErrorCode != 1223)
                    throw;
            }
        }
    }
}
