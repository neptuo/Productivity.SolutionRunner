using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Neptuo.Productivity.SolutionRunner.Views
{
    internal interface IWindowManager
    {
        MainWindow Main { get; }
        ConfigurationWindow Configuration { get; }
        StatisticsWindow Statistics { get; }
    }
}
