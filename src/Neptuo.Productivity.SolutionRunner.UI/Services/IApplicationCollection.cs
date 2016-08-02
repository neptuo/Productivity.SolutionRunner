using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;

namespace Neptuo.Productivity.SolutionRunner.Services
{
    public interface IApplicationCollection
    {
        IApplicationCollection Add(string name, string path, string arguments, ImageSource icon, bool isMain);
        IApplicationCollection Add(string name, Version version, string path, string arguments, ImageSource icon, bool isMain);
    }
}
