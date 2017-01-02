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
    public class PositionService : IPositionProvider
    {
        private readonly Settings settings;

        internal PositionService(Settings settings)
        {
            Ensure.NotNull(settings, "settings");
            this.settings = settings;
        }

        public void Apply(IPositionTarget target)
        {
            Ensure.NotNull(target, "target");

            switch (settings.PositionMode)
            {
                case PositionMode.CenterPrimaryScreen:
                    Screen screen = Screen.PrimaryScreen;
                    target.Left = (screen.WorkingArea.Width - target.ActualWidth) / 2 + screen.WorkingArea.Left;
                    target.Top = (screen.WorkingArea.Height - target.ActualHeight) / 2 + screen.WorkingArea.Top;
                    break;
                case PositionMode.UserDefined:
                    target.Left = settings.PositionLeft;
                    target.Top = settings.PositionTop;
                    break;
            }
        }
    }
}
