using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace Neptuo.Productivity.SolutionRunner.Services.Positions
{
    /// <summary>
    /// A target component to apply position to.
    /// </summary>
    public interface IPositionTarget
    {
        /// <summary>
        /// Gets or sets left offset.
        /// </summary>
        double Left { get; set; }

        /// <summary>
        /// Gets or sets top offset.
        /// </summary>
        double Top { get; set; }

        /// <summary>
        /// Gets actual width.
        /// </summary>
        double ActualWidth { get; }

        /// <summary>
        /// Gets actual height.
        /// </summary>
        double ActualHeight { get; }
    }
}
