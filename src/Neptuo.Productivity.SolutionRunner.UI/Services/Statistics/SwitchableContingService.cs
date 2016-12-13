using Neptuo;
using Neptuo.Productivity.SolutionRunner.Properties;
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
        private readonly Settings settings;
        private readonly ICountingAppender appender;
        private readonly ICountingReader reader;

        internal SwitchableContingService(Settings settings, ICountingAppender appender, ICountingReader reader)
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

        public IEnumerable<ApplicationCountModel> Applications()
        {
            if (settings.IsStatisticsCounted)
                reader.Applications();

            return Enumerable.Empty<ApplicationCountModel>();
        }

        public IEnumerable<FileCountModel> Files()
        {
            if (settings.IsStatisticsCounted)
                reader.Files();

            return Enumerable.Empty<FileCountModel>();
        }
    }
}
