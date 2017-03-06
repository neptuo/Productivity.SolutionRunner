using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;

namespace Neptuo.Productivity.SolutionRunner.ViewModels
{
    public class NoPreferedApplicationViewModel : IPreferedApplicationViewModel
    {
        public ImageSource Icon { get; private set; }

        public string Name
        {
            get { return "---"; }
        }

        public string Path { get; private set; }
    }
}
