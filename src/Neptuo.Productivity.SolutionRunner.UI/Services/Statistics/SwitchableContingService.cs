using Neptuo.Productivity.SolutionRunner.Services.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Neptuo.Productivity.SolutionRunner.Services.Statistics
{
    /// <summary>
    /// An implementation of <see cref="ICountingAppender"/> which can be turned off by <see cref="Settings.IsStatisticsCounted"/>.
    /// </summary>
    public class SwitchableContingService : ICountingAppender, ICountingReader
    {
        private readonly ISettings settings;
        private readonly ICountingAppender appender;
        private readonly ICountingReader reader;

        internal SwitchableContingService(ISettings settings, ICountingAppender appender, ICountingReader reader)
        {
            Ensure.NotNull(settings, "settings");
            Ensure.NotNull(appender, "appender");
            Ensure.NotNull(reader, "reader");
            this.settings = settings;
            this.appender = appender;
            this.reader = reader;
        }

        public void Application(string path)
        {
            if (settings.IsStatisticsCounted)
                appender.Application(path);
        }

        public void File(string applicationPath, string filePath)
        {
            if (settings.IsStatisticsCounted)
                appender.File(applicationPath, filePath);
        }

        public void File(string applicationPath, string argumentsTemplate, string filePath)
        {
            if (settings.IsStatisticsCounted)
                appender.File(applicationPath, argumentsTemplate, filePath);
        }

        public IEnumerable<Month> Months()
        {
            if (settings.IsStatisticsCounted)
                return reader.Months();

            return Enumerable.Empty<Month>();
        }

        public IEnumerable<ApplicationCountModel> Applications(Month monthFrom, Month monthTo)
        {
            if (settings.IsStatisticsCounted)
                return reader.Applications(monthFrom, monthTo);

            return Enumerable.Empty<ApplicationCountModel>();
        }

        public IEnumerable<FileCountModel> Files(Month monthFrom, Month monthTo)
        {
            if (settings.IsStatisticsCounted)
                return reader.Files(monthFrom, monthTo);

            return Enumerable.Empty<FileCountModel>();
        }
    }
}
