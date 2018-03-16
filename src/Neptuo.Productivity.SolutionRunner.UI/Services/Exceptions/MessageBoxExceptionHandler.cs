using Neptuo.Exceptions.Handlers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace Neptuo.Productivity.SolutionRunner.Services.Exceptions
{
    public class MessageBoxExceptionHandler : IExceptionHandler<Exception>
    {
        private readonly Application application;

        public MessageBoxExceptionHandler(Application application)
        {
            Ensure.NotNull(application, "application");
            this.application = application;
        }

        public void Handle(Exception exception)
        {
            StringBuilder message = new StringBuilder();

            string exceptionMessage = exception.ToString();
            if (exceptionMessage.Length > 800)
                exceptionMessage = exceptionMessage.Substring(0, 800);

            message.AppendLine(exceptionMessage);

            MessageBoxResult result = MessageBox.Show(message.ToString(), "Do you want to kill the aplication?", MessageBoxButton.YesNo);

            if (result == MessageBoxResult.Yes)
                application.Shutdown();
        }
    }
}
