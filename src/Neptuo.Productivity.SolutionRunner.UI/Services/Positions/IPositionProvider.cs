using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Neptuo.Productivity.SolutionRunner.Services.Positions
{
    public interface IPositionProvider
    {
        void Apply(IPositionTarget target);
    }
}
