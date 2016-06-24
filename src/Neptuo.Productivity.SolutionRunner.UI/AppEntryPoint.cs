using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Neptuo.Productivity.SolutionRunner
{
    public class AppEntryPoint
    {
        [STAThread]
        public static void Main(string[] args)
        {
            AppEntryPointManager manager = new AppEntryPointManager();
            manager.Run(args);
        }
    }
}
