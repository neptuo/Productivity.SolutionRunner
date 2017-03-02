using Neptuo.Observables.Collections;
using Neptuo.Productivity.SolutionRunner.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using System.Windows.Media;

namespace Neptuo.Productivity.SolutionRunner.ViewModels
{
    public class MainApplicationCollection : ObservableCollection<MainApplicationListViewModel>, IApplicationCollection
    {
        public IApplicationCollection Add(string name, string path, string arguments, ImageSource icon, Key hotKey, bool isMain)
        {
            Add(new MainApplicationListViewModel()
            {
                Name = name,
                Path = path,
                Icon = icon,
                IsEnabled = true
            });
            return this;
        }

        public IApplicationCollection Add(string name, Version version, string path, string arguments, ImageSource icon, Key hotKey, bool isMain)
        {
            Add(new MainApplicationListViewModel()
            {
                Name = name,
                Path = path,
                Icon = icon,
                IsEnabled = true
            });
            return this;
        }
    }
}
