using Neptuo.Productivity.SolutionRunner.Services.Positions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Markup;

namespace Neptuo.Productivity.SolutionRunner.Views.DataSources
{
    public class PositionModes : MarkupExtension
    {
        public override object ProvideValue(IServiceProvider serviceProvider)
        {
            return new List<PositionMode>()
            {
                PositionMode.CenterPrimaryScreen,
                PositionMode.UserDefined
            };
        }
    }
}
