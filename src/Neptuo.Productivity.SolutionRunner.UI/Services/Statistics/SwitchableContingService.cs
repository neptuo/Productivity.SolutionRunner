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
    public class SwitchableContingService : ICountingAppender
    {
        private readonly Settings settings;
        private readonly ICountingAppender appender;

        internal SwitchableContingService(Settings settings, ICountingAppender appender)
        {
            Ensure.NotNull(settings, "settings");
            Ensure.NotNull(appender, "appender");
            this.settings = settings;
            this.appender = appender;
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
    }
}
