using Neptuo;
using Neptuo.Productivity.SolutionRunner.Services;
using Neptuo.Productivity.SolutionRunner.Services.Configuration;
using Neptuo.Productivity.SolutionRunner.Views;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Neptuo.Productivity.SolutionRunner
{
    internal partial class AppTrayIcon
    {
        private readonly App app;
        private readonly ISettings settings;
        private readonly INavigator navigator;
        private readonly IWindowManager windows;

        private NotifyIcon trayIcon;

        public AppTrayIcon(App app, ISettings settings, INavigator navigator, IWindowManager windows)
        {
            Ensure.NotNull(app, "app");
            Ensure.NotNull(settings, "settings");
            Ensure.NotNull(navigator, "navigator");
            Ensure.NotNull(windows, "windows");
            this.app = app;
            this.settings = settings;
            this.navigator = navigator;
            this.windows = windows;
        }

        public bool TryCreate()
        {
            if (settings.IsTrayIcon && trayIcon == null)
            {
                trayIcon = new NotifyIcon();
                trayIcon.Icon = Icon.ExtractAssociatedIcon(Process.GetCurrentProcess().MainModule.FileName);
                trayIcon.Text = "SolutionRunner";
                trayIcon.MouseClick += OnIconClick;
                trayIcon.Visible = true;

                trayIcon.ContextMenu = new ContextMenu();
                trayIcon.ContextMenu.MenuItems.Add("Open", (sender, e) => { navigator.OpenMain(); windows.Configuration?.Close(); windows.Statistics?.Close(); });
                trayIcon.ContextMenu.MenuItems.Add("Configuration", (sender, e) => { navigator.OpenConfiguration(); windows.Main?.Close(); windows.Statistics?.Close(); });
                trayIcon.ContextMenu.MenuItems.Add("Statistics", (sender, e) => navigator.OpenStatistics());
                trayIcon.ContextMenu.MenuItems.Add("Exit", (sender, e) => app.Shutdown());
                return true;
            }

            return false;
        }

        public bool TryDestroy()
        {
            if (trayIcon != null)
            {
                trayIcon.MouseClick -= OnIconClick;
                trayIcon.Dispose();
                trayIcon = null;
                return true;
            }

            return false;
        }

        public bool TryUpdate()
        {
            if (trayIcon != null && windows.Main != null)
            {
                string resourceName = null;
                if (windows.Main.ViewModel.IsLoading)
                    resourceName = "Neptuo.Productivity.SolutionRunner.Resources.Loading.ico";
                else
                    resourceName = "Neptuo.Productivity.SolutionRunner.Resources.SolutionRunner.ico";

                trayIcon.Icon = new Icon(Assembly.GetExecutingAssembly().GetManifestResourceStream(resourceName));
                return true;
            }

            return false;
        }

        private void OnIconClick(object sender, MouseEventArgs e)
        {
            if (e.Button != MouseButtons.Right)
                app.Activate();
        }
    }
}
