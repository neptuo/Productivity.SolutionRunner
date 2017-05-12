using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace Neptuo.Productivity.SolutionRunner.ViewModels
{
    public static class _KeyViewModelExtensions
    {
        public static Key GetKey(this KeyViewModel viewModel)
        {
            if (viewModel == null)
                return Key.None;

            return viewModel.Key;
        }
    }
}
