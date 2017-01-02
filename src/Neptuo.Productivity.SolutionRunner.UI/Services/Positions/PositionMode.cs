using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Neptuo.Productivity.SolutionRunner.Services.Positions
{
    /// <summary>
    /// Enumeration of supported position modes.
    /// </summary>
    [Serializable]
    public enum PositionMode
    {
        /// <summary>
        /// A target component is centered on primary screen.
        /// </summary>
        CenterPrimaryScreen,

        /// <summary>
        /// A target component is placed on user defined position.
        /// </summary>
        UserDefined
    }
}
