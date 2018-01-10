using Neptuo.Observables.Collections;
using Neptuo.Productivity.SolutionRunner.Services.Applications;
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
        public IApplicationBuilder Add(string name, string path, string arguments, bool isAdministratorRequired, ImageSource icon, Key hotKey, bool isMain)
        {
            MainApplicationListViewModel viewModel = new MainApplicationListViewModel()
            {
                Name = name,
                Path = path,
                Icon = icon,
                IsEnabled = true
            };
            Add(viewModel);
            return viewModel;
        }

        public IApplicationBuilder Add(string name, Version version, string path, string arguments, bool isAdministratorRequired, ImageSource icon, Key hotKey, bool isMain)
        {
            MainApplicationListViewModel viewModel = new MainApplicationListViewModel()
            {
                Name = name,
                Path = path,
                Icon = icon,
                IsEnabled = true
            };
            Add(viewModel);
            return viewModel;
        }
    }
}
