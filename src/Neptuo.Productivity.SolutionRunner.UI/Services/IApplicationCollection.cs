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
        IApplicationCollection Add(string name, string path, ImageSource icon);
    }
}
