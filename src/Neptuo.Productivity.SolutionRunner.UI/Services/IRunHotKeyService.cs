using Neptuo.Productivity.SolutionRunner.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace Neptuo.Productivity.SolutionRunner.Services
{
    public interface IRunHotKeyService
    {
        IRunHotKeyService Bind(Key key, ModifierKeys modifier);
        IRunHotKeyService UnBind();
        KeyViewModel FindKeyViewModel();

        bool IsSet { get; }
    }
}
