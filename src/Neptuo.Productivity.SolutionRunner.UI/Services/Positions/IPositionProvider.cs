using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Neptuo.Productivity.SolutionRunner.Services.Positions
{
    /// <summary>
    /// A component for applying initial position.
    /// </summary>
    public interface IPositionProvider
    {
        /// <summary>
        /// Applies position to <paramref name="target"/>.
        /// </summary>
        /// <param name="target">A target component to apply position to.</param>
        void Apply(IPositionTarget target);

        /// <summary>
        /// Applies position to <paramref name="target"/> in horizontal axe.
        /// </summary>
        /// <param name="target">A target component to apply position to.</param>
        void ApplyHorizontal(IPositionTarget target);

        /// <summary>
        /// Applies position to <paramref name="target"/> in vertical axe.
        /// </summary>
        /// <param name="target">A target component to apply position to.</param>
        void ApplyVertical(IPositionTarget target);
    }
}
