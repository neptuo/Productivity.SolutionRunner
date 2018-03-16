using Neptuo.Productivity.SolutionRunner.Services.Configuration;
using Neptuo.Productivity.SolutionRunner.Services.Execution;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Neptuo.Productivity.SolutionRunner.ViewModels
{
    public static class EventManager
    {
        public static event Action<FileViewModel> FilePinned;

        public static void RaiseFilePinned(FileViewModel viewModel)
        {
            Ensure.NotNull(viewModel, "viewModel");
            if (FilePinned != null)
                FilePinned(viewModel);
        }


        public static event Action<ConfigurationViewModel> ConfigurationSaved;

        public static void RaiseConfigurationSaved(ConfigurationViewModel viewModel)
        {
            Ensure.NotNull(viewModel, "viewModel");
            if (ConfigurationSaved != null)
                ConfigurationSaved(viewModel);
        }

        public static event Action<IApplication, IFile> ProcessStarted;

        public static void RaiseProcessStarted(IApplication application, IFile file)
        {
            Ensure.NotNull(application, "application");
            if (ProcessStarted != null)
                ProcessStarted(application, file);
        }
    }
}
