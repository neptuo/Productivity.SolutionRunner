using Neptuo.Observables;
using Neptuo.Productivity.SolutionRunner.Services.Applications;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;

namespace Neptuo.Productivity.SolutionRunner.ViewModels
{
    public class AdditionalApplicationListViewModel : ObservableObject
    {
        public AdditionalApplicationModel Model { get; private set; }

        private ImageSource icon;
        public ImageSource Icon
        {
            get { return icon; }
            set
            {
                if (icon != value)
                {
                    icon = value;
                    RaisePropertyChanged();
                }
            }
        }

        private string path;
        public string Path
        {
            get { return path; }
            set
            {
                if (path != value)
                {
                    path = value;
                    RaisePropertyChanged();
                }
            }
        }

        public AdditionalApplicationListViewModel(AdditionalApplicationModel model)
        {
            UpdateModel(model);
        }

        public void UpdateModel(AdditionalApplicationModel model)
        {
            Ensure.NotNull(model, "model");
            Icon = IconExtractor.Get(model.Path);
            Path = model.Path;
            Model = model;
        }
    }
}
