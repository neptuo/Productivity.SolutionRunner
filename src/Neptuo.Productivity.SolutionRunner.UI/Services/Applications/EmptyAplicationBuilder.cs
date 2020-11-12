using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace Neptuo.Productivity.SolutionRunner.Services.Applications
{
    public class EmptyAplicationBuilder : IApplicationBuilder
    {
        public IApplicationBuilder AddCommand(string name, string path, string emptyArguments, string fileArguments, bool isAdministratorRequired, bool isApplicationWindowShown, Key hotKey)
        {
            return this;
        }
    }
}
