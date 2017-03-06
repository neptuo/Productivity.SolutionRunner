using Neptuo.Observables.Collections;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Neptuo.Productivity.SolutionRunner.ViewModels
{
    public class ContainerCollection<T> : ObservableCollection<Container<T>>
        where T : class
    {
    }
}
