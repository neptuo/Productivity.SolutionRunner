using Neptuo;
using Neptuo.Productivity.SolutionRunner.Properties;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Forms;

namespace Neptuo.Productivity.SolutionRunner.Services.Positions
{
    /// <summary>
    /// An implmentation of <see cref="IPositionProvider"/> based on saved user settings.
    /// </summary>
    public class PositionService : IPositionProvider
    {
        private readonly Settings settings;

        /// <summary>
        /// Creates a new instance.
        /// </summary>
        /// <param name="settings">User settings.</param>
        internal PositionService(Settings settings)
        {
            Ensure.NotNull(settings, "settings");
            this.settings = settings;
        }

        public void Apply(IPositionTarget target)
        {
            ApplyHorizontal(target);
            ApplyVertical(target);
        }

        public void ApplyHorizontal(IPositionTarget target)
        {
            Ensure.NotNull(target, "target");

            switch (settings.PositionMode)
            {
                case PositionMode.CenterPrimaryScreen:
                    Screen screen = Screen.PrimaryScreen;
                    target.Left = (screen.WorkingArea.Width - target.ActualWidth) / 2 + screen.WorkingArea.Left;
                    break;
                case PositionMode.UserDefined:
                    target.Left = settings.PositionLeft;
                    break;
            }
        }

        public void ApplyVertical(IPositionTarget target)
        {
            Ensure.NotNull(target, "target");

            switch (settings.PositionMode)
            {
                case PositionMode.CenterPrimaryScreen:
                    Screen screen = Screen.PrimaryScreen;
                    target.Top = (screen.WorkingArea.Height - target.ActualHeight) / 2 + screen.WorkingArea.Top;
                    break;
                case PositionMode.UserDefined:
                    target.Top = settings.PositionTop;
                    break;
            }
        }
    }
}
