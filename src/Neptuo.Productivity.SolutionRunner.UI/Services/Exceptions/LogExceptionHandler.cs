using Neptuo;
using Neptuo.Exceptions.Handlers;
using Neptuo.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Neptuo.Productivity.SolutionRunner.Services.Exceptions
{
    public class LogExceptionHandler : IExceptionHandler<Exception>
    {
        private readonly ILog log;

        public LogExceptionHandler(ILog log)
        {
            Ensure.NotNull(log, "log");
            this.log = log;
        }

        public void Handle(Exception exception)
        {
            log.Fatal(exception);
        }
    }
}
