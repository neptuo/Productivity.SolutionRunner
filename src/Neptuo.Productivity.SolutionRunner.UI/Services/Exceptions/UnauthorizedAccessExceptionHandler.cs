using Neptuo.Exceptions.Handlers;
using Neptuo.Productivity.SolutionRunner.Services.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace Neptuo.Productivity.SolutionRunner.Services.Exceptions
{
    public class UnauthorizedAccessExceptionHandler : IExceptionHandler<UnauthorizedAccessException>
    {
        private readonly ISettings settings;
        private readonly INavigator navigator;
        private readonly Action mainWindowCloser;

        internal UnauthorizedAccessExceptionHandler(ISettings settings, INavigator navigator, Action mainWindowCloser)
        {
            Ensure.NotNull(settings, "settings");
            Ensure.NotNull(navigator, "navigator");
            Ensure.NotNull(mainWindowCloser, "mainWindowCloser");
            this.settings = settings;
            this.navigator = navigator;
            this.mainWindowCloser = mainWindowCloser;
        }

        public void Handle(UnauthorizedAccessException exception)
        {
            MessageBox.Show(
                String.Format(
                    "The path '{0}' is not accessible. {1}We are going to reset the root directory.",
                    settings.SourceDirectoryPath,
                    Environment.NewLine
                ),
                "Unauthorized access",
                MessageBoxButton.OK
            );

            navigator.OpenConfiguration();
            mainWindowCloser();
        }
    }
}
