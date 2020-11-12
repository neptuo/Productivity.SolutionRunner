using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using System.Windows.Media;

namespace Neptuo.Productivity.SolutionRunner.Services.Applications
{
    public interface IApplicationCollection
    {
        IApplicationBuilder Add(string name, string path, string emptyArguments, string fileArguments, bool isAdministratorRequired, bool isApplicationWindowShown, ImageSource icon, Key hotKey, bool isMain);
        IApplicationBuilder Add(string name, Version version, string path, string emptyArguments, string fileArguments, bool isAdministratorRequired, bool isApplicationWindowShown, ImageSource icon, Key hotKey, bool isMain);
    }
}
