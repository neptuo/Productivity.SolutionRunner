using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Neptuo.Productivity.SolutionRunner.Services.Searching
{
    partial class PatternMatcherFactory
    {
        public interface IFileModel
        {
            string NameWithExtension { get; }
            string Path { get; }
        }
    }
}
