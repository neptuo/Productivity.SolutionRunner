using Neptuo.Productivity.SolutionRunner.Services.Applications;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Neptuo.Productivity.SolutionRunner.ViewModels
{
    public interface IApplicationViewModel
    {
        string Name { get; }
        string Path { get; }
        string Arguments { get; }
        KeyViewModel HotKey { get; }

        AdditionalApplicationModel ToModel();
    }
}
