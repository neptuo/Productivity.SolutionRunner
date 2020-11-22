using Neptuo;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.ApplicationModel;

namespace Neptuo.Productivity.SolutionRunner.Services.StartupShortcuts
{
    public class UwpAutoStartupTask : IAutoStartup
    {
        private readonly INavigator navigator;
        private StartupTask startupTask;

        public UwpAutoStartupTask(INavigator navigator)
        {
            Ensure.NotNull(navigator, "navigator");
            this.navigator = navigator;
        }

        private bool IsEnabled(StartupTaskState state)
            => state == StartupTaskState.Enabled || state == StartupTaskState.EnabledByPolicy;

        public async Task<bool> IsEnabledAsync()
        {
            await EnsureStartupTask();
            return IsEnabled(startupTask.State);
        }

        public async Task<bool> DisableAsync()
        {
            await EnsureStartupTask();
            switch (startupTask.State)
            {
                case StartupTaskState.Disabled:
                case StartupTaskState.DisabledByUser:
                case StartupTaskState.DisabledByPolicy:
                    return true;
                case StartupTaskState.Enabled:
                    startupTask.Disable();
                    return true;
                case StartupTaskState.EnabledByPolicy:
                    navigator.Notify("Auto startup can't be disabled by group policy.");
                    return true;
            }

            return false;
        }

        public async Task<bool> EnableAsync()
        {
            await EnsureStartupTask();
            switch (startupTask.State)
            {
                case StartupTaskState.Disabled:
                    StartupTaskState newState = await startupTask.RequestEnableAsync();
                    return IsEnabled(newState);
                case StartupTaskState.DisabledByUser:
                    navigator.Notify("You must manually enable auto startup. Go to Task Manager -> Startup and enable Solution Runner to startup on your login.");
                    return false;
                case StartupTaskState.DisabledByPolicy:
                    navigator.Notify("Auto startup disabled by group policy, or not supported on this device.");
                    return false;
                case StartupTaskState.Enabled:
                case StartupTaskState.EnabledByPolicy:
                    return true;
            }

            return false;
        }

        private async Task<StartupTask> EnsureStartupTask()
        {
            if (startupTask == null)
                startupTask = await StartupTask.GetAsync("SolutionRunnerStartup");

            return startupTask;
        }
    }
}
