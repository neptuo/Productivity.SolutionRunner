using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;

namespace Neptuo.Productivity.SolutionRunner.ViewModels
{
    public interface IPreferedApplicationViewModel
    {
        ImageSource Icon { get; }
        string Name { get; }
        string Path { get; }
    }
}
