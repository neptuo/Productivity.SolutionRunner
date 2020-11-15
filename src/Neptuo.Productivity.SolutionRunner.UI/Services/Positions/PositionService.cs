using Neptuo.Productivity.SolutionRunner.Services.Configuration;
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
        private readonly ISettings settings;

        /// <summary>
        /// Creates a new instance.
        /// </summary>
        /// <param name="settings">User settings.</param>
        internal PositionService(ISettings settings)
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
                    target.Left = (SystemParameters.WorkArea.Width - target.ActualWidth) / 2 + SystemParameters.WorkArea.Left;
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
                    target.Top = (SystemParameters.WorkArea.Height - target.ActualHeight) / 2 + SystemParameters.WorkArea.Top;
                    break;
                case PositionMode.UserDefined:
                    target.Top = settings.PositionTop;
                    break;
            }
        }
    }
}
