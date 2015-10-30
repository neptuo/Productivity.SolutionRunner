using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace Neptuo.Productivity.SolutionRunner.ViewModels.Commands
{
    public abstract class CommandBase : ICommand
    {
        public bool CanExecute(object parameter)
        {
            return CanExecute();
        }

        protected abstract bool CanExecute();

        public event EventHandler CanExecuteChanged;

        public void RaiseCanExecuteChanged()
        {
            if (CanExecuteChanged != null)
                CanExecuteChanged(this, EventArgs.Empty);
        }

        public void Execute(object parameter)
        {
            if (CanExecute())
                Execute();
        }

        protected abstract void Execute();
    }
}
