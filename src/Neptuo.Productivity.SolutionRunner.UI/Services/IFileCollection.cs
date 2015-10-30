using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Neptuo.Productivity.SolutionRunner.Services
{
    public interface IFileCollection
    {
        IFileCollection Clear();
        IFileCollection Add(string fileName, string filePath, bool isPinned);
    }
}
