using Neptuo.Exceptions.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Neptuo.Productivity.SolutionRunner.Services.Searching
{
    public static class _EnsureExceptionExtensions
    {
        public static NotSupportedException NotSupportedSearchMode(this EnsureExceptionHelper ensure, FileSearchMode mode)
        {
            return Ensure.Exception.NotSupported("Not supported file search mode '{0}'.", mode);
        }
    }
}
